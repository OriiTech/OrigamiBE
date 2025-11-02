using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Origami.API.Services.Implement;
using Origami.BusinessTier.Payload;
using Origami.BusinessTier.Payload.Course;
using Origami.DataTier.Models;
using Origami.DataTier.Paginate;
using Origami.DataTier.Repository.Interfaces;

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
            IPaginate<GetCourseResponse> response = await _unitOfWork.GetRepository<Course>().GetPagingListAsync(
                selector: x => _mapper.Map<GetCourseResponse>(x),
                filter: filter,
                orderBy: x => x.OrderBy(c => c.Title),
                include: q => q.Include(c => c.Teacher),
                page: pagingModel.page,
                size: pagingModel.size
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
    }
}