using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Origami.API.Services.Implement;
using Origami.BusinessTier.Payload;
using Origami.BusinessTier.Payload.Course;
using Origami.DataTier.Models;
using Origami.DataTier.Paginate;
using Origami.DataTier.Repository.Interfaces;
using System.Linq.Expressions;

namespace Origami.API.Services.Interfaces
{
    public class CourseService : BaseService<CourseService>, ICourseService
    {
        private readonly IConfiguration _configuration;

        public CourseService(IUnitOfWork<OrigamiDbContext> unitOfWork, ILogger<CourseService> logger, IMapper mapper,
            IHttpContextAccessor httpContextAccessor, IConfiguration configuration) : base(unitOfWork, logger, mapper, httpContextAccessor)
        {
            _configuration = configuration;
        }

        public async Task<int> CreateNewCourse(CourseInfo request)
        {
            var repo = _unitOfWork.GetRepository<Course>();

            if (request.TeacherId.HasValue)
            {
                var userRepo = _unitOfWork.GetRepository<User>();
                var teacher = await userRepo.GetFirstOrDefaultAsync(
                    predicate: x => x.UserId == request.TeacherId.Value,
                    asNoTracking: true
                );
                if (teacher == null)
                    throw new BadHttpRequestException("TeacherNotFound");
            }

            var newCourse = _mapper.Map<Course>(request);
            await repo.InsertAsync(newCourse);

            var isSuccessful = await _unitOfWork.CommitAsync() > 0;
            if (!isSuccessful)
                throw new BadHttpRequestException("CreateFailed");

            return newCourse.CourseId;
        }

        public async Task<GetCourseResponse> GetCourseById(int id)
        {
            Course course = await _unitOfWork.GetRepository<Course>().GetFirstOrDefaultAsync(
                predicate: x => x.CourseId == id,
                include: q => q.Include(c => c.Teacher),
                asNoTracking: true
            ) ?? throw new BadHttpRequestException("CourseNotFound");

            return _mapper.Map<GetCourseResponse>(course);
        }

        public async Task<IPaginate<GetCourseResponse>> ViewAllCourses(CourseFilter filter, PagingModel pagingModel)
        {
            if (filter != null && filter.MinPrice.HasValue && filter.MaxPrice.HasValue
                && filter.MinPrice.Value > filter.MaxPrice.Value)
            {
                (filter.MinPrice, filter.MaxPrice) = (filter.MaxPrice, filter.MinPrice);
            }

            Expression<Func<Course, bool>> pricePredicate = x =>
                (filter == null ||
                 (!filter.MinPrice.HasValue || (x.Price.HasValue && x.Price.Value >= filter.MinPrice.Value)) &&
                 (!filter.MaxPrice.HasValue || (x.Price.HasValue && x.Price.Value <= filter.MaxPrice.Value)));

            IPaginate<GetCourseResponse> response = await _unitOfWork.GetRepository<Course>().GetPagingListAsync(
                selector: x => _mapper.Map<GetCourseResponse>(x),
                predicate: pricePredicate,
                orderBy: x => x.OrderBy(c => c.Title),
                include: q => q.Include(c => c.Teacher),
                page: pagingModel.page,
                size: pagingModel.size,
                filter: filter
            );

            return response;
        }

        public async Task<bool> UpdateCourseInfo(int id, CourseInfo request)
        {
            var repo = _unitOfWork.GetRepository<Course>();
            var course = await repo.GetFirstOrDefaultAsync(
                predicate: x => x.CourseId == id,
                asNoTracking: false
            ) ?? throw new BadHttpRequestException("CourseNotFound");

            if (request.TeacherId.HasValue && request.TeacherId != course.TeacherId)
            {
                var userRepo = _unitOfWork.GetRepository<User>();
                var teacher = await userRepo.GetFirstOrDefaultAsync(
                    predicate: x => x.UserId == request.TeacherId.Value,
                    asNoTracking: true
                );
                if (teacher == null)
                    throw new BadHttpRequestException("TeacherNotFound");

                course.TeacherId = request.TeacherId;
            }

            if (!string.IsNullOrEmpty(request.Title))
                course.Title = request.Title;
            if (!string.IsNullOrEmpty(request.Description))
                course.Description = request.Description;
            if (request.Price.HasValue)
                course.Price = request.Price;

            bool isSuccessful = await _unitOfWork.CommitAsync() > 0;
            return isSuccessful;
        }

        public async Task<bool> DeleteCourse(int id)
        {
            var repo = _unitOfWork.GetRepository<Course>();
            var course = await repo.GetFirstOrDefaultAsync(
                predicate: x => x.CourseId == id,
                include: q => q.Include(c => c.Lessons),
                asNoTracking: false
            ) ?? throw new BadHttpRequestException("CourseNotFound");

            if (course.Lessons != null && course.Lessons.Any())
                throw new BadHttpRequestException("CourseHasLessonsCannotDelete");

            var courseAccessRepo = _unitOfWork.GetRepository<CourseAccess>();
            bool hasAccess = await courseAccessRepo.AnyAsync(x => x.CourseId == id);
            if (hasAccess)
                throw new BadHttpRequestException("CourseHasAccessesCannotDelete");

            repo.Delete(course);
            bool isSuccessful = await _unitOfWork.CommitAsync() > 0;
            return isSuccessful;
        }

        public async Task<IPaginate<GetCourseCardResponse>> GetCoursesAsync(CourseFilter filter, PagingModel pagingModel)
        {
            var courseRepo = _unitOfWork.GetRepository<Course>();

            Expression<Func<Course, bool>> searchPredicate = x =>
                    string.IsNullOrEmpty(filter.Name) ||
                    (x.Title != null && x.Title.Contains(filter.Name));

            var courses = await courseRepo.GetPagingListAsync(
                selector: c => new GetCourseCardResponse
                {
                    Id = c.CourseId.ToString(),
                    Title = c.Title,
                    Instructors = c.Teacher != null ? new List<CourseInstructorInfo>
                    {
                        new CourseInstructorInfo
                        {
                            Id = c.Teacher.UserId.ToString(),
                            Name = c.Teacher.Username,
                            Image = c.Teacher.AvatarUrl
                        }
                    } : new List<CourseInstructorInfo>(),
                    TargetLevel = c.CourseTargetLevels.Select(ctl => new CourseTargetLevelInfo
                    {
                        Id = ctl.LevelId.ToString(),
                        Name = ctl.Level.Name
                    }).ToList(),
                    Language = c.Language,
                    PublishedDate = c.PublishedAt,
                    TotalStudents = c.CourseAccesses.Count,
                    // SỬA LỖI: Thêm '?? 0.0' để xử lý trường hợp null
                    Rating = c.CourseReviews.Average(r => r.Rating) ?? 0.0,
                    Price = c.Price,
                    ThumbnailUrl = c.ThumbnailUrl,
                    CreatedAt = c.CreatedAt,
                    Bestseller = c.Bestseller
                },
                predicate: searchPredicate,
                include: query => query
                    .Include(c => c.Teacher)
                    .Include(c => c.CourseTargetLevels)
                        .ThenInclude(ctl => ctl.Level)
                    .Include(c => c.CourseAccesses)
                    .Include(c => c.CourseReviews),
                page: pagingModel.page,
                size: pagingModel.size,
                orderBy: x => x.OrderByDescending(c => c.CreatedAt)
            );
            return courses;
        }

        public async Task<GetCourseDetailResponse> GetCourseDetailAsync(int id)
        {
            var course = await _unitOfWork.GetRepository<Course>().GetFirstOrDefaultAsync(
                predicate: c => c.CourseId == id,
                include: query => query
                    .Include(c => c.Teacher)
                        .ThenInclude(t => t.Instructor) // Include Instructor details
                    .Include(c => c.CourseTargetLevels)
                        .ThenInclude(ctl => ctl.Level)
                    .Include(c => c.CourseReviews)
                    .Include(c => c.Lessons) // Include lessons (chapters)
            ) ?? throw new BadHttpRequestException("Course not found");

            // Calculate rating distribution
            var ratingDistribution = course.CourseReviews
                .GroupBy(r => r.Rating)
                .ToDictionary(g => $"{g.Key}_stars", g => g.Count());

            var response = new GetCourseDetailResponse
            {
                Id = course.CourseId.ToString(),
                Title = course.Title,
                // Subtitle is missing from the DB script, returning empty for now
                // Subtitle = course.Subtitle,
                Description = course.Description,
                Instructors = course.Teacher != null ? new List<DetailInstructorInfo>
                {
                    new DetailInstructorInfo
                    {
                        Id = course.Teacher.UserId.ToString(),
                        Name = course.Teacher.Username,
                        Image = course.Teacher.AvatarUrl,
                        Bio = course.Teacher.Instructor?.Bio,
                        Rating = course.Teacher.Instructor?.RatingAvg,
                        Reviews = course.Teacher.Instructor?.TotalReviews ?? 0,
                        Courses = course.Teacher.Instructor?.TotalCourses ?? 0,
                    }
                } : new List<DetailInstructorInfo>(),
                TargetLevel = course.CourseTargetLevels.Select(ctl => new CourseTargetLevelInfo
                {
                    Id = ctl.LevelId.ToString(),
                    Name = ctl.Level.Name
                }).ToList(),
                Language = course.Language,
                PublishedDate = course.PublishedAt,
                Rating = new CourseRatingDetail
                {
                    Count = course.CourseReviews.Count,
                    // SỬA LỖI: Thêm '?? 0.0' để xử lý trường hợp null
                    Average = course.CourseReviews.Average(r => r.Rating) ?? 0.0,
                    Distribution = ratingDistribution
                },
                Price = course.Price,
                ThumbnailUrl = course.ThumbnailUrl,
                TotalLessons = course.Lessons.Count,
                Lessons = course.Lessons.Select(l => new CourseContentLesson
                {
                    Id = l.LessonId.ToString(),
                    Title = l.Title
                    // TotalLectures and TotalMinutes are postponed
                }).ToList(),
                // These are postponed because they require DB changes
                Objectives = new List<string>(),
                Category = new List<object>()
            };

            return response;
        }
    }
}