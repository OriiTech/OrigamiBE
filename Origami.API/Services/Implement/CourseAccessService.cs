using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Origami.API.Services.Implement;
using Origami.BusinessTier.Payload;
using Origami.BusinessTier.Payload.CourseAccess;
using Origami.DataTier.Models;
using Origami.DataTier.Paginate;
using Origami.DataTier.Repository.Interfaces;

namespace Origami.API.Services.Interfaces
{
    public class CourseAccessService : BaseService<CourseAccessService>, ICourseAccessService
    {
        private readonly IConfiguration _configuration;

        public CourseAccessService(IUnitOfWork<OrigamiDbContext> unitOfWork, ILogger<CourseAccessService> logger, IMapper mapper,
            IHttpContextAccessor httpContextAccessor, IConfiguration configuration) : base(unitOfWork, logger, mapper, httpContextAccessor)
        {
            _configuration = configuration;
        }

        public async Task<int> CreateNewCourseAccess(CourseAccessInfo request)
        {
            var repo = _unitOfWork.GetRepository<CourseAccess>();

            // Kiểm tra CourseId có tồn tại không
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

            // Kiểm tra LearnerId có tồn tại không
            if (request.LearnerId.HasValue)
            {
                var userRepo = _unitOfWork.GetRepository<User>();
                var learner = await userRepo.GetFirstOrDefaultAsync(
                    predicate: x => x.UserId == request.LearnerId.Value,
                    asNoTracking: true
                );
                if (learner == null)
                    throw new BadHttpRequestException("LearnerNotFound");
            }

            // Kiểm tra xem Learner đã có access vào Course này chưa
            bool alreadyExists = await repo.AnyAsync(
                x => x.CourseId == request.CourseId && x.LearnerId == request.LearnerId
            );
            if (alreadyExists)
                throw new BadHttpRequestException("CourseAccessAlreadyExists");

            var newCourseAccess = _mapper.Map<CourseAccess>(request);
            newCourseAccess.PurchasedAt = DateTime.UtcNow;

            await repo.InsertAsync(newCourseAccess);

            var isSuccessful = await _unitOfWork.CommitAsync() > 0;
            if (!isSuccessful)
                throw new BadHttpRequestException("CreateFailed");

            return newCourseAccess.AccessId;
        }

        public async Task<GetCourseAccessResponse> GetCourseAccessById(int id)
        {
            CourseAccess courseAccess = await _unitOfWork.GetRepository<CourseAccess>().GetFirstOrDefaultAsync(
                predicate: x => x.AccessId == id,
                include: q => q.Include(ca => ca.Course).Include(ca => ca.Learner),
                asNoTracking: true
            ) ?? throw new BadHttpRequestException("CourseAccessNotFound");

            return _mapper.Map<GetCourseAccessResponse>(courseAccess);
        }

        public async Task<IPaginate<GetCourseAccessResponse>> ViewAllCourseAccesses(CourseAccessFilter filter, PagingModel pagingModel)
        {
            IPaginate<GetCourseAccessResponse> response = await _unitOfWork.GetRepository<CourseAccess>().GetPagingListAsync(
                selector: x => _mapper.Map<GetCourseAccessResponse>(x),
                filter: filter,
                orderBy: x => x.OrderByDescending(ca => ca.PurchasedAt),
                include: q => q.Include(ca => ca.Course).Include(ca => ca.Learner),
                page: pagingModel.page,
                size: pagingModel.size
            );

            return response;
        }

        public async Task<bool> DeleteCourseAccess(int id)
        {
            var repo = _unitOfWork.GetRepository<CourseAccess>();
            var courseAccess = await repo.GetFirstOrDefaultAsync(
                predicate: x => x.AccessId == id,
                asNoTracking: false
            ) ?? throw new BadHttpRequestException("CourseAccessNotFound");

            repo.Delete(courseAccess);
            bool isSuccessful = await _unitOfWork.CommitAsync() > 0;
            return isSuccessful;
        }
    }
}