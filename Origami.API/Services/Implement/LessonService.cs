using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Origami.API.Services.Implement;
using Origami.BusinessTier.Payload;
using Origami.BusinessTier.Payload.Lesson;
using Origami.DataTier.Models;
using Origami.DataTier.Paginate;
using Origami.DataTier.Repository.Interfaces;

namespace Origami.API.Services.Interfaces
{
    public class LessonService : BaseService<LessonService>, ILessonService
    {
        private readonly IConfiguration _configuration;

        public LessonService(IUnitOfWork<OrigamiDbContext> unitOfWork, ILogger<LessonService> logger, IMapper mapper,
            IHttpContextAccessor httpContextAccessor, IConfiguration configuration) : base(unitOfWork, logger, mapper, httpContextAccessor)
        {
            _configuration = configuration;
        }

        public async Task<int> CreateNewLesson(LessonInfo request)
        {
            var repo = _unitOfWork.GetRepository<Lesson>();

            if (request.CourseId.HasValue)
            {
                var courseRepo = _unitOfWork.GetRepository<Course>();
                var course = await courseRepo.GetFirstOrDefaultAsync(
                    predicate: x => x.CourseId == request.CourseId.Value,
                    asNoTracking: true
                );
                if (course == null)
                    throw new BadHttpRequestException("CourseNotFound");
            }

            var newLesson = _mapper.Map<Lesson>(request);
            newLesson.CreatedAt = DateTime.UtcNow;

            await repo.InsertAsync(newLesson);

            var isSuccessful = await _unitOfWork.CommitAsync() > 0;
            if (!isSuccessful)
                throw new BadHttpRequestException("CreateFailed");

            return newLesson.LessonId;
        }

        public async Task<GetLessonResponse> GetLessonById(int id)
        {
            Lesson lesson = await _unitOfWork.GetRepository<Lesson>().GetFirstOrDefaultAsync(
                predicate: x => x.LessonId == id,
                include: q => q.Include(l => l.Course),
                asNoTracking: true
            ) ?? throw new BadHttpRequestException("LessonNotFound");

            return _mapper.Map<GetLessonResponse>(lesson);
        }

        public async Task<IPaginate<GetLessonResponse>> ViewAllLessons(LessonFilter filter, PagingModel pagingModel)
        {
            IPaginate<GetLessonResponse> response = await _unitOfWork.GetRepository<Lesson>().GetPagingListAsync(
                selector: x => _mapper.Map<GetLessonResponse>(x),
                filter: filter,
                orderBy: x => x.OrderBy(l => l.Title),
                include: q => q.Include(l => l.Course),
                page: pagingModel.page,
                size: pagingModel.size
            );

            return response;
        }

        public async Task<bool> UpdateLessonInfo(int id, LessonInfo request)
        {
            var repo = _unitOfWork.GetRepository<Lesson>();
            var lesson = await repo.GetFirstOrDefaultAsync(
                predicate: x => x.LessonId == id,
                asNoTracking: false
            ) ?? throw new BadHttpRequestException("LessonNotFound");

            if (request.CourseId.HasValue && request.CourseId != lesson.CourseId)
            {
                var courseRepo = _unitOfWork.GetRepository<Course>();
                var course = await courseRepo.GetFirstOrDefaultAsync(
                    predicate: x => x.CourseId == request.CourseId.Value,
                    asNoTracking: true
                );
                if (course == null)
                    throw new BadHttpRequestException("CourseNotFound");

                lesson.CourseId = request.CourseId;
            }

            if (!string.IsNullOrEmpty(request.Title))
                lesson.Title = request.Title;
            if (!string.IsNullOrEmpty(request.Description))
                lesson.Description = request.Description;
            if (request.Price.HasValue)
                lesson.Price = request.Price;

            bool isSuccessful = await _unitOfWork.CommitAsync() > 0;
            return isSuccessful;
        }

        public async Task<bool> DeleteLesson(int id)
        {
            var repo = _unitOfWork.GetRepository<Lesson>();
            var lesson = await repo.GetFirstOrDefaultAsync(
                predicate: x => x.LessonId == id,
                asNoTracking: false
            ) ?? throw new BadHttpRequestException("LessonNotFound");

            repo.Delete(lesson);
            bool isSuccessful = await _unitOfWork.CommitAsync() > 0;
            return isSuccessful;
        }
    }
}