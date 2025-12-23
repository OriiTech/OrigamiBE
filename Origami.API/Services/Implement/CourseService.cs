using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Origami.API.Services.Implement;
using Origami.BusinessTier.Payload;
using Origami.BusinessTier.Payload.Course;
using Origami.DataTier.Models;
using Origami.DataTier.Paginate;
using Origami.DataTier.Repository.Interfaces;
using System.Linq;
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

        public async Task<IPaginate<CourseListItemResponse>> GetCoursesExtendedAsync(CourseFilter filter, PagingModel pagingModel)
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

            var repo = _unitOfWork.GetRepository<Course>();

            var response = await repo.GetPagingListAsync(
                selector: x => new CourseListItemResponse
                {
                    Id = x.CourseId,
                    Title = x.Title,
                    Language = x.Language,
                    PublishedDate = x.PublishedAt,
                    Price = x.Price,
                    ThumbnailUrl = x.ThumbnailUrl,
                    CreatedAt = x.CreatedAt,
                    Bestseller = x.Bestseller,
                    TotalStudents = x.CourseAccesses.Count,
                    Rating = x.CourseReviews.Any() ? x.CourseReviews.Average(r => (double?)r.Rating ?? 0) : null,
                    Instructors = x.Teacher != null
                        ? new List<SimpleUserDto>
                        {
                            new SimpleUserDto
                            {
                                Id = x.TeacherId ?? 0,
                                Name = x.Teacher.Username,
                                Image = null // NOTE: avatar chưa có trường
                            }
                        }
                        : new List<SimpleUserDto>(),
                    TargetLevel = x.Levels.Select(t => new IdNameDto
                    {
                        Id = t.LevelId,
                        Name = t.Name
                    }).ToList(),
                    PaidOnly = null,   // NOTE: DB chưa có cột paid_only
                    Trending = null    // NOTE: DB chưa có cột trending
                },
                predicate: pricePredicate,
                orderBy: q => q.OrderByDescending(c => c.CreatedAt),
                include: q => q
                    .Include(c => c.Teacher)
                    .Include(c => c.Levels)
                    .Include(c => c.CourseReviews)
                    .Include(c => c.CourseAccesses),
                page: pagingModel.page,
                size: pagingModel.size,
                filter: filter
            );

            return response;
        }

        public async Task<CourseDetailResponse> GetCourseDetailAsync(int id)
        {
            var course = await _unitOfWork.GetRepository<Course>().GetFirstOrDefaultAsync(
                predicate: x => x.CourseId == id,
                include: q => q
                    .Include(c => c.Teacher)
                    .Include(c => c.Categories)
                    .Include(c => c.Levels)
                    .Include(c => c.CourseReviews).ThenInclude(r => r.User)
                    .Include(c => c.CourseReviews).ThenInclude(r => r.ReviewResponses).ThenInclude(rr => rr.User)
                    .Include(c => c.CourseAccesses)
                    .Include(c => c.Lessons).ThenInclude(l => l.Lectures).ThenInclude(le => le.Resources),
                asNoTracking: true
            ) ?? throw new BadHttpRequestException("CourseNotFound");

            var lectures = course.Lessons.SelectMany(l => l.Lectures).ToList();
            var resources = lectures.SelectMany(l => l.Resources).ToList();

            var ratingCount = course.CourseReviews.Count;
            double? ratingAverage = ratingCount > 0
                ? course.CourseReviews.Average(r => (double?)r.Rating)
                : null;

            var distribution = new CourseRatingDistribution
            {
                FiveStars = course.CourseReviews.Count(r => r.Rating == 5),
                FourStars = course.CourseReviews.Count(r => r.Rating == 4),
                ThreeStars = course.CourseReviews.Count(r => r.Rating == 3),
                TwoStars = course.CourseReviews.Count(r => r.Rating == 2),
                OneStar = course.CourseReviews.Count(r => r.Rating == 1),
            };

            var detail = new CourseDetailResponse
            {
                Id = course.CourseId,
                Title = course.Title,
                Subtitle = null, // NOTE: DB chưa có cột subtitle
                Description = course.Description,
                Objectives = new List<string>(), // NOTE: DB chưa có cột objectives
                Language = course.Language,
                PublishedDate = course.PublishedAt,
                Instructors = course.Teacher != null
                    ? new List<CourseInstructorDto>
                    {
                        new CourseInstructorDto
                        {
                            Id = course.TeacherId ?? 0,
                            Name = course.Teacher.Username,
                            Image = null,
                            Rating = null, // chưa có dữ liệu tổng hợp
                            Reviews = null,
                            Courses = null
                        }
                    }
                    : new List<CourseInstructorDto>(),
                    TargetLevel = course.Levels.Select(t => new IdNameDto
                {
                    Id = t.LevelId,
                    Name = t.Name
                }).ToList(),
                Category = course.Categories.Select(c => new IdNameDto
                {
                    Id = c.CategoryId,
                    Name = c.CategoryName
                }).ToList(),
                CourseLength = new CourseLengthDto
                {
                    TotalLectures = lectures.Count,
                    TotalResources = resources.Count,
                    TotalVideoHours = lectures.Any() ? lectures.Sum(l => l.DurationMinutes ?? 0) / 60.0 : null
                },
                Enrollment = new EnrollmentDto
                {
                    TotalStudents = course.CourseAccesses.Count,
                    RecentStudents = null // NOTE: chưa có trường riêng, để null
                },
                Rating = new CourseRatingDto
                {
                    Average = ratingAverage,
                    Count = ratingCount,
                    Distribution = distribution
                },
                Price = new CoursePriceDto
                {
                    Amount = course.Price,
                    PaidOnly = null // NOTE: DB chưa có cột paid_only
                },
                Preview = new PreviewDto
                {
                    VideoUrl = null, // NOTE: DB chưa có preview video url
                    ThumbnailUrl = course.ThumbnailUrl
                },
                Content = new CourseContentDto
                {
                    Lessons = course.Lessons.Select(l =>
                    {
                        var lLectures = l.Lectures.ToList();
                        return new CourseLessonSummaryDto
                        {
                            Id = l.LessonId,
                            Title = l.Title,
                            TotalLectures = lLectures.Count,
                            VideoCount = lLectures.Count(le => string.Equals(le.Type, "video", StringComparison.OrdinalIgnoreCase)),
                            TextCount = lLectures.Count(le => string.Equals(le.Type, "text", StringComparison.OrdinalIgnoreCase)),
                            ResourceCount = lLectures.Sum(le => le.Resources.Count),
                            TotalMinutes = lLectures.Sum(le => le.DurationMinutes ?? 0)
                        };
                    }).ToList(),
                    TotalLessons = course.Lessons.Count,
                    TotalLectures = lectures.Count,
                    TotalHours = lectures.Sum(le => le.DurationMinutes ?? 0) / 60.0
                },
                Metadata = new MetadataDto
                {
                    CreatedAt = course.CreatedAt,
                    UpdatedAt = null, // NOTE: DB chưa có cột updated_at
                    Bestseller = course.Bestseller,
                    Trending = null // NOTE: DB chưa có cột trending
                }
            };

            return detail;
        }

        public async Task<CourseReviewSummaryResponse> GetCourseReviewsAsync(int courseId, int recentCount = 5)
        {
            var reviewRepo = _unitOfWork.GetRepository<CourseReview>();
            var reviews = await reviewRepo.GetListAsync(
                predicate: r => r.CourseId == courseId,
                include: q => q.Include(r => r.User)
                               .Include(r => r.ReviewResponses).ThenInclude(rr => rr.User),
                orderBy: q => q.OrderByDescending(r => r.CreatedAt),
                asNoTracking: true
            );

            var recent = reviews.Take(recentCount).Select(r => new CourseReviewItemResponse
            {
                Id = r.ReviewId,
                StudentName = r.User?.Username,
                Rating = r.Rating,
                Date = r.CreatedAt?.ToString("yyyy-MM-dd"),
                Comment = r.Comment,
                LikeCount = null, // NOTE: DB chưa có cột like_count
                Response = r.ReviewResponses.Any()
                    ? new CourseReviewResponseItem
                    {
                        SenseiName = r.ReviewResponses.First().User?.Username,
                        Date = r.ReviewResponses.First().CreatedAt.ToString("yyyy-MM-dd"),
                        Comment = r.ReviewResponses.First().Comment
                    }
                    : null
            }).ToList();

            return new CourseReviewSummaryResponse
            {
                Total = reviews.Count,
                Recent = recent
            };
        }

        public async Task<CourseQuestionSummaryResponse> GetCourseQuestionsAsync(int courseId, int recentCount = 5)
        {
            var questionRepo = _unitOfWork.GetRepository<Question>();
            var questions = await questionRepo.GetListAsync(
                predicate: q => q.CourseId == courseId,
                include: q => q.Include(x => x.User)
                               .Include(x => x.Answers).ThenInclude(a => a.User),
                orderBy: q => q.OrderByDescending(x => x.CreatedAt),
                asNoTracking: true
            );

            var recent = questions.Take(recentCount).Select(q => new CourseQuestionItemResponse
            {
                Id = q.QuestionId,
                Question = q.Content,
                StudentName = q.User?.Username,
                Date = q.CreatedAt?.ToString("yyyy-MM-dd"),
                Answers = q.Answers.Select(a => new CourseAnswerItemResponse
                {
                    ResponseName = a.User?.Username,
                    Date = a.CreatedAt?.ToString("yyyy-MM-dd"),
                    Comment = a.Content
                }).ToList()
            }).ToList();

            return new CourseQuestionSummaryResponse
            {
                TotalQuestions = questions.Count,
                RecentQuestions = recent
            };
        }

        public async Task<List<LessonWithLecturesResponse>> GetLessonsWithLecturesAsync(int courseId)
        {
            var lessonRepo = _unitOfWork.GetRepository<Lesson>();
            var lessons = await lessonRepo.GetListAsync(
                predicate: l => l.CourseId == courseId,
                include: q => q.Include(l => l.Lectures),
                orderBy: q => q.OrderBy(l => l.LessonId),
                asNoTracking: true
            );

            return lessons.Select(l =>
            {
                var totalMinutes = l.Lectures.Sum(le => le.DurationMinutes ?? 0);
                return new LessonWithLecturesResponse
                {
                    Id = l.LessonId,
                    Title = l.Title,
                    Lectures = l.Lectures.Select(le => new LectureItemResponse
                    {
                        Id = le.LectureId,
                        Title = le.Title,
                        Type = le.Type,
                        DurationMinutes = le.DurationMinutes,
                        PreviewAvailable = le.PreviewAvailable,
                        IsCompleted = null // NOTE: cần user context + Lecture_progress để xác định
                    }).ToList(),
                    TotalLectures = l.Lectures.Count,
                    TotalLecturesCompleted = 0, // NOTE: chưa tính được vì thiếu user context
                    TotalMinutes = totalMinutes
                };
            }).ToList();
        }

        public async Task<LectureDetailResponse> GetLectureDetailAsync(int lectureId)
        {
            var lectureRepo = _unitOfWork.GetRepository<Lecture>();
            var lecture = await lectureRepo.GetFirstOrDefaultAsync(
                predicate: l => l.LectureId == lectureId,
                include: q => q
                    .Include(l => l.Resources)
                    .Include(l => l.Questions).ThenInclude(q => q.User)
                    .Include(l => l.Questions).ThenInclude(q => q.Answers).ThenInclude(a => a.User),
                asNoTracking: true
            ) ?? throw new BadHttpRequestException("LectureNotFound");

            return new LectureDetailResponse
            {
                Id = lecture.LectureId,
                Title = lecture.Title,
                Description = lecture.Description,
                Url = lecture.ContentUrl,
                Type = lecture.Type,
                DurationMinutes = lecture.DurationMinutes,
                PreviewAvailable = lecture.PreviewAvailable,
                Note = null, // NOTE: DB chưa có cột note
                Resources = lecture.Resources.Select(r => new LectureResourceResponse
                {
                    Id = r.ResourceId,
                    Title = r.Title,
                    Url = r.Url,
                    Type = r.Type
                }).ToList(),
                Discussions = lecture.Questions.Select(q => new LectureDiscussionResponse
                {
                    Id = q.QuestionId,
                    Question = q.Content,
                    StudentName = q.User?.Username,
                    Date = q.CreatedAt?.ToString("yyyy-MM-dd"),
                    Answers = q.Answers.Select(a => new LectureAnswerResponse
                    {
                        ResponseName = a.User?.Username,
                        Date = a.CreatedAt?.ToString("yyyy-MM-dd"),
                        Comment = a.Content
                    }).ToList()
                }).ToList()
            };
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
    }
}