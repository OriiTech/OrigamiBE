using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Newtonsoft.Json;
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
            if (!string.IsNullOrEmpty(request.Language))
                course.Language = request.Language;
            if (!string.IsNullOrEmpty(request.ThumbnailUrl))
                course.ThumbnailUrl = request.ThumbnailUrl;
            if (!string.IsNullOrEmpty(request.Subtitle))
                course.Subtitle = request.Subtitle;
            if (request.Objectives != null)
                course.Objectives = JsonConvert.SerializeObject(request.Objectives);
            if (request.PaidOnly.HasValue)
                course.PaidOnly = request.PaidOnly;
            if (request.Trending.HasValue)
                course.Trending = request.Trending;
            if (!string.IsNullOrEmpty(request.PreviewVideoUrl))
                course.PreviewVideoUrl = request.PreviewVideoUrl;
            if (request.UpdatedAt.HasValue)
                course.UpdatedAt = request.UpdatedAt;
            else
                course.UpdatedAt = DateTime.UtcNow;

            // Update Categories
            if (request.CategoryIds != null && request.CategoryIds.Any())
            {
                var categoryRepo = _unitOfWork.GetRepository<Category>();
                var categories = await categoryRepo.GetListAsync(
                    predicate: c => request.CategoryIds.Contains(c.CategoryId),
                    asNoTracking: false
                );
                course.Categories = categories.ToList();
            }

            // Update TargetLevels
            if (request.TargetLevelIds != null && request.TargetLevelIds.Any())
            {
                var levelRepo = _unitOfWork.GetRepository<TargetLevel>();
                var levels = await levelRepo.GetListAsync(
                    predicate: l => request.TargetLevelIds.Contains(l.LevelId),
                    asNoTracking: false
                );
                course.Levels = levels.ToList();
            }

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
                                Image = x.Teacher.UserProfile != null ? x.Teacher.UserProfile.AvatarUrl : null
                            }
                        }
                        : new List<SimpleUserDto>(),
                    TargetLevel = x.Levels.Select(t => new IdNameDto
                    {
                        Id = t.LevelId,
                        Name = t.Name
                    }).ToList(),
                    PaidOnly = x.PaidOnly,
                    Trending = x.Trending
                },
                predicate: pricePredicate,
                orderBy: q => q.OrderByDescending(c => c.CreatedAt),
                include: q => q
                    .Include(c => c.Teacher).ThenInclude(t => t.UserProfile)
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
                    .Include(c => c.Teacher).ThenInclude(t => t.UserProfile)
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
                Subtitle = course.Subtitle,
                Description = course.Description,
                Objectives = string.IsNullOrEmpty(course.Objectives)
                    ? new List<string>()
                    : JsonConvert.DeserializeObject<List<string>>(course.Objectives ?? "[]") ?? new List<string>(),
                Language = course.Language,
                PublishedDate = course.PublishedAt,
                Instructors = course.Teacher != null
                    ? new List<CourseInstructorDto>
                    {
                        new CourseInstructorDto
                        {
                            Id = course.TeacherId ?? 0,
                            Name = course.Teacher.Username,
                            Image = course.Teacher.UserProfile?.AvatarUrl,
                            Rating = course.Teacher.CourseReviews.Any() 
                                ? course.Teacher.CourseReviews.Average(r => (double?)r.Rating) 
                                : null,
                            Reviews = course.Teacher.CourseReviews.Count,
                            Courses = course.Teacher.Courses.Count
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
                    RecentStudents = course.CourseAccesses
                        .Where(ca => ca.PurchasedAt.HasValue && ca.PurchasedAt.Value >= DateTime.UtcNow.AddDays(-30))
                        .Select(ca => ca.LearnerId)
                        .Distinct()
                        .Count()
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
                    PaidOnly = course.PaidOnly
                },
                Preview = new PreviewDto
                {
                    VideoUrl = course.PreviewVideoUrl,
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
                    UpdatedAt = course.UpdatedAt,
                    Bestseller = course.Bestseller,
                    Trending = course.Trending
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
                LikeCount = r.LikeCount ?? 0,
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

        public async Task<List<LessonWithLecturesResponse>> GetLessonsWithLecturesAsync(int courseId, int? userId = null)
        {
            var lessonRepo = _unitOfWork.GetRepository<Lesson>();
            var lessons = await lessonRepo.GetListAsync(
                predicate: l => l.CourseId == courseId,
                include: q => q.Include(l => l.Lectures),
                orderBy: q => q.OrderBy(l => l.LessonId),
                asNoTracking: true
            );

            // Load lecture progress if userId is provided
            List<LectureProgress>? lectureProgresses = null;
            if (userId.HasValue)
            {
                var progressRepo = _unitOfWork.GetRepository<LectureProgress>();
                var lectureIds = lessons.SelectMany(l => l.Lectures).Select(le => le.LectureId).ToList();
                var progresses = await progressRepo.GetListAsync(
                    predicate: p => p.UserId == userId.Value && lectureIds.Contains(p.LectureId),
                    asNoTracking: true
                );
                lectureProgresses = progresses.ToList();
            }

            return lessons.Select(l =>
            {
                var lLectures = l.Lectures.ToList();
                var totalMinutes = lLectures.Sum(le => le.DurationMinutes ?? 0);
                
                // Calculate completed lectures if userId is provided
                int totalCompleted = 0;
                if (userId.HasValue && lectureProgresses != null)
                {
                    var lessonLectureIds = lLectures.Select(le => le.LectureId).ToList();
                    totalCompleted = lectureProgresses
                        .Count(p => lessonLectureIds.Contains(p.LectureId) && p.IsCompleted);
                }

                return new LessonWithLecturesResponse
                {
                    Id = l.LessonId,
                    Title = l.Title,
                    Lectures = lLectures.Select(le => new LectureItemResponse
                    {
                        Id = le.LectureId,
                        Title = le.Title,
                        Type = le.Type,
                        DurationMinutes = le.DurationMinutes,
                        PreviewAvailable = le.PreviewAvailable,
                        IsCompleted = userId.HasValue && lectureProgresses != null
                            ? lectureProgresses.Any(p => p.LectureId == le.LectureId && p.IsCompleted)
                            : null
                    }).ToList(),
                    TotalLectures = lLectures.Count,
                    TotalLecturesCompleted = totalCompleted,
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
                Note = lecture.Note,
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

        public async Task<int> CreateOrUpdateCourse(CourseSaveRequest request)
        {
            var repo = _unitOfWork.GetRepository<Course>();
            Course course;

            if (request.InstructorIds != null && request.InstructorIds.Any())
            {
                var userRepo = _unitOfWork.GetRepository<User>();
                var teacherId = request.InstructorIds.First(); // Hiện tại chỉ hỗ trợ 1 instructor
                var teacher = await userRepo.GetFirstOrDefaultAsync(
                    predicate: x => x.UserId == teacherId,
                    asNoTracking: true
                );
                if (teacher == null)
                    throw new BadHttpRequestException("TeacherNotFound");
            }

            course = new Course
            {
                Title = request.Title,
                Subtitle = request.Subtitle,
                Description = request.Description,
                Language = request.Language,
                Objectives = request.Objectives != null ? JsonConvert.SerializeObject(request.Objectives) : null,
                Price = request.Price?.Amount,
                PaidOnly = request.Price?.PaidOnly,
                PreviewVideoUrl = request.Preview?.VideoUrl,
                ThumbnailUrl = request.Preview?.ThumbnailUrl,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                TeacherId = request.InstructorIds != null && request.InstructorIds.Any() ? request.InstructorIds.First() : null
            };

            await repo.InsertAsync(course);

            // Add Categories
            if (request.CategoryIds != null && request.CategoryIds.Any())
            {
                var categoryRepo = _unitOfWork.GetRepository<Category>();
                var categories = await categoryRepo.GetListAsync(
                    predicate: c => request.CategoryIds.Contains(c.CategoryId),
                    asNoTracking: false
                );
                foreach (var category in categories)
                {
                    course.Categories.Add(category);
                }
            }

            // Add TargetLevels
            if (request.TargetLevelIds != null && request.TargetLevelIds.Any())
            {
                var levelRepo = _unitOfWork.GetRepository<TargetLevel>();
                var levels = await levelRepo.GetListAsync(
                    predicate: l => request.TargetLevelIds.Contains(l.LevelId),
                    asNoTracking: false
                );
                foreach (var level in levels)
                {
                    course.Levels.Add(level);
                }
            }

            var isSuccessful = await _unitOfWork.CommitAsync() > 0;
            if (!isSuccessful)
                throw new BadHttpRequestException("CreateFailed");

            return course.CourseId;
        }

        public async Task<int> CreateOrUpdateLesson(LessonSaveRequest request)
        {
            var repo = _unitOfWork.GetRepository<Lesson>();
            Lesson lesson;

            // Verify course exists
            var courseRepo = _unitOfWork.GetRepository<Course>();
            var course = await courseRepo.GetFirstOrDefaultAsync(
                predicate: x => x.CourseId == request.CourseId,
                asNoTracking: true
            );
            if (course == null)
                throw new BadHttpRequestException("CourseNotFound");

            if (request.Id.HasValue)
            {
                // Update existing lesson
                lesson = await repo.GetFirstOrDefaultAsync(
                    predicate: x => x.LessonId == request.Id.Value,
                    asNoTracking: false
                ) ?? throw new BadHttpRequestException("LessonNotFound");

                if (!string.IsNullOrEmpty(request.Title))
                    lesson.Title = request.Title;
            }
            else
            {
                // Create new lesson
                lesson = new Lesson
                {
                    CourseId = request.CourseId,
                    Title = request.Title,
                    CreatedAt = DateTime.UtcNow
                };
                await repo.InsertAsync(lesson);
            }

            // Handle lectures if provided
            if (request.Lectures != null && request.Lectures.Any())
            {
                var lectureRepo = _unitOfWork.GetRepository<Lecture>();
                foreach (var lectureReq in request.Lectures)
                {
                    Lecture lecture;
                    if (lectureReq.Id.HasValue)
                    {
                        lecture = await lectureRepo.GetFirstOrDefaultAsync(
                            predicate: x => x.LectureId == lectureReq.Id.Value,
                            asNoTracking: false
                        );
                        if (lecture != null)
                        {
                            lecture.Title = lectureReq.Title ?? lecture.Title;
                            lecture.Description = lectureReq.Description;
                            lecture.Type = lectureReq.Type ?? lecture.Type;
                            lecture.DurationMinutes = lectureReq.DurationMinutes;
                            lecture.PreviewAvailable = lectureReq.PreviewAvailable ?? lecture.PreviewAvailable;
                            lecture.ContentUrl = lectureReq.ContentUrl;
                            lecture.Note = lectureReq.Note;
                        }
                    }
                    else
                    {
                        lecture = new Lecture
                        {
                            LessonId = lesson.LessonId,
                            Title = lectureReq.Title ?? "",
                            Description = lectureReq.Description,
                            Type = lectureReq.Type ?? "video",
                            DurationMinutes = lectureReq.DurationMinutes,
                            PreviewAvailable = lectureReq.PreviewAvailable ?? false,
                            ContentUrl = lectureReq.ContentUrl,
                            Note = lectureReq.Note,
                            CreatedAt = DateTime.UtcNow
                        };
                        await lectureRepo.InsertAsync(lecture);

                        // Handle resources if provided
                        if (lectureReq.Resources != null && lectureReq.Resources.Any())
                        {
                            var resourceRepo = _unitOfWork.GetRepository<Resource>();
                            foreach (var resourceReq in lectureReq.Resources)
                            {
                                Resource resource;
                                if (resourceReq.Id.HasValue)
                                {
                                    resource = await resourceRepo.GetFirstOrDefaultAsync(
                                        predicate: x => x.ResourceId == resourceReq.Id.Value,
                                        asNoTracking: false
                                    );
                                    if (resource != null)
                                    {
                                        resource.Title = resourceReq.Title ?? resource.Title;
                                        resource.Url = resourceReq.Url ?? resource.Url;
                                        resource.Type = resourceReq.Type ?? resource.Type;
                                    }
                                }
                                else
                                {
                                    resource = new Resource
                                    {
                                        LectureId = lecture.LectureId,
                                        Title = resourceReq.Title ?? "",
                                        Url = resourceReq.Url ?? "",
                                        Type = resourceReq.Type ?? "pdf",
                                        CreatedAt = DateTime.UtcNow
                                    };
                                    await resourceRepo.InsertAsync(resource);
                                }
                            }
                        }
                    }
                }
            }

            var isSuccessful = await _unitOfWork.CommitAsync() > 0;
            if (!isSuccessful)
                throw new BadHttpRequestException("SaveFailed");

            return lesson.LessonId;
        }

        public async Task<bool> DeleteLesson(int lessonId)
        {
            var repo = _unitOfWork.GetRepository<Lesson>();
            var lesson = await repo.GetFirstOrDefaultAsync(
                predicate: x => x.LessonId == lessonId,
                include: q => q.Include(l => l.Lectures),
                asNoTracking: false
            ) ?? throw new BadHttpRequestException("LessonNotFound");

            if (lesson.Lectures != null && lesson.Lectures.Any())
                throw new BadHttpRequestException("LessonHasLecturesCannotDelete");

            repo.Delete(lesson);
            bool isSuccessful = await _unitOfWork.CommitAsync() > 0;
            return isSuccessful;
        }

        public async Task<int> CreateOrUpdateLecture(LectureSaveRequest request)
        {
            var repo = _unitOfWork.GetRepository<Lecture>();
            Lecture lecture;

            // Verify lesson exists
            var lessonRepo = _unitOfWork.GetRepository<Lesson>();
            var lesson = await lessonRepo.GetFirstOrDefaultAsync(
                predicate: x => x.LessonId == request.LessonId,
                asNoTracking: true
            );
            if (lesson == null)
                throw new BadHttpRequestException("LessonNotFound");

            if (request.Id.HasValue)
            {
                // Update existing lecture
                lecture = await repo.GetFirstOrDefaultAsync(
                    predicate: x => x.LectureId == request.Id.Value,
                    asNoTracking: false
                ) ?? throw new BadHttpRequestException("LectureNotFound");

                if (!string.IsNullOrEmpty(request.Title))
                    lecture.Title = request.Title;
                if (!string.IsNullOrEmpty(request.Description))
                    lecture.Description = request.Description;
                if (!string.IsNullOrEmpty(request.Type))
                    lecture.Type = request.Type;
                lecture.DurationMinutes = request.DurationMinutes;
                if (request.PreviewAvailable.HasValue)
                    lecture.PreviewAvailable = request.PreviewAvailable.Value;
                if (!string.IsNullOrEmpty(request.ContentUrl))
                    lecture.ContentUrl = request.ContentUrl;
                lecture.Note = request.Note;
            }
            else
            {
                // Create new lecture
                lecture = new Lecture
                {
                    LessonId = request.LessonId,
                    Title = request.Title ?? "",
                    Description = request.Description,
                    Type = request.Type ?? "video",
                    DurationMinutes = request.DurationMinutes,
                    PreviewAvailable = request.PreviewAvailable ?? false,
                    ContentUrl = request.ContentUrl,
                    Note = request.Note,
                    CreatedAt = DateTime.UtcNow
                };
                await repo.InsertAsync(lecture);
            }

            // Handle resources if provided
            if (request.Resources != null && request.Resources.Any())
            {
                var resourceRepo = _unitOfWork.GetRepository<Resource>();
                foreach (var resourceReq in request.Resources)
                {
                    Resource resource;
                    if (resourceReq.Id.HasValue)
                    {
                        resource = await resourceRepo.GetFirstOrDefaultAsync(
                            predicate: x => x.ResourceId == resourceReq.Id.Value,
                            asNoTracking: false
                        );
                        if (resource != null)
                        {
                            resource.Title = resourceReq.Title ?? resource.Title;
                            resource.Url = resourceReq.Url ?? resource.Url;
                            resource.Type = resourceReq.Type ?? resource.Type;
                        }
                    }
                    else
                    {
                        resource = new Resource
                        {
                            LectureId = lecture.LectureId,
                            Title = resourceReq.Title ?? "",
                            Url = resourceReq.Url ?? "",
                            Type = resourceReq.Type ?? "pdf",
                            CreatedAt = DateTime.UtcNow
                        };
                        await resourceRepo.InsertAsync(resource);
                    }
                }
            }

            var isSuccessful = await _unitOfWork.CommitAsync() > 0;
            if (!isSuccessful)
                throw new BadHttpRequestException("SaveFailed");

            return lecture.LectureId;
        }

        public async Task<bool> DeleteLecture(int lectureId)
        {
            var repo = _unitOfWork.GetRepository<Lecture>();
            var lecture = await repo.GetFirstOrDefaultAsync(
                predicate: x => x.LectureId == lectureId,
                include: q => q.Include(l => l.Resources).Include(l => l.Questions),
                asNoTracking: false
            ) ?? throw new BadHttpRequestException("LectureNotFound");

            if (lecture.Resources != null && lecture.Resources.Any())
            {
                var resourceRepo = _unitOfWork.GetRepository<Resource>();
                foreach (var resource in lecture.Resources)
                {
                    resourceRepo.Delete(resource);
                }
            }

            repo.Delete(lecture);
            bool isSuccessful = await _unitOfWork.CommitAsync() > 0;
            return isSuccessful;
        }

        public async Task<bool> DeleteResource(int resourceId)
        {
            var repo = _unitOfWork.GetRepository<Resource>();
            var resource = await repo.GetFirstOrDefaultAsync(
                predicate: x => x.ResourceId == resourceId,
                asNoTracking: false
            ) ?? throw new BadHttpRequestException("ResourceNotFound");

            repo.Delete(resource);
            bool isSuccessful = await _unitOfWork.CommitAsync() > 0;
            return isSuccessful;
        }

        public async Task<bool> MarkCourseTrending(int id, bool trending)
        {
            var repo = _unitOfWork.GetRepository<Course>();
            var course = await repo.GetFirstOrDefaultAsync(
                predicate: x => x.CourseId == id,
                asNoTracking: false
            ) ?? throw new BadHttpRequestException("CourseNotFound");

            course.Trending = trending;
            course.UpdatedAt = DateTime.UtcNow;

            bool isSuccessful = await _unitOfWork.CommitAsync() > 0;
            return isSuccessful;
        }

        public async Task<bool> MarkCourseBestseller(int id, bool bestseller)
        {
            var repo = _unitOfWork.GetRepository<Course>();
            var course = await repo.GetFirstOrDefaultAsync(
                predicate: x => x.CourseId == id,
                asNoTracking: false
            ) ?? throw new BadHttpRequestException("CourseNotFound");

            course.Bestseller = bestseller;
            course.UpdatedAt = DateTime.UtcNow;

            bool isSuccessful = await _unitOfWork.CommitAsync() > 0;
            return isSuccessful;
        }
    }
}