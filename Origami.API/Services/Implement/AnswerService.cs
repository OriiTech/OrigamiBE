
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Origami.API.Services.Implement;
using Origami.BusinessTier.Payload;
using Origami.BusinessTier.Payload.Answer;
using Origami.DataTier.Models;
using Origami.DataTier.Paginate;
using Origami.DataTier.Repository.Interfaces;

namespace Origami.API.Services.Interfaces
{
    public class AnswerService : BaseService<AnswerService>, IAnswerService
    {
        private readonly IConfiguration _configuration;

        public AnswerService(IUnitOfWork<OrigamiDbContext> unitOfWork, ILogger<AnswerService> logger, IMapper mapper,
            IHttpContextAccessor httpContextAccessor, IConfiguration configuration) : base(unitOfWork, logger, mapper, httpContextAccessor)
        {
            _configuration = configuration;
        }

        public async Task<int> CreateNewAnswer(AnswerInfo request)
        {
            var repo = _unitOfWork.GetRepository<Answer>();

            if (request.QuestionId.HasValue)
            {
                var questionRepo = _unitOfWork.GetRepository<Question>();
                var question = await questionRepo.GetFirstOrDefaultAsync(
                    predicate: x => x.QuestionId == request.QuestionId.Value,
                    asNoTracking: true
                );
                if (question == null)
                    throw new BadHttpRequestException("QuestionNotFound");
            }

            if (request.UserId.HasValue)
            {
                var userRepo = _unitOfWork.GetRepository<User>();
                var user = await userRepo.GetFirstOrDefaultAsync(
                    predicate: x => x.UserId == request.UserId.Value,
                    asNoTracking: true
                );
                if (user == null)
                    throw new BadHttpRequestException("UserNotFound");
            }

            var newAnswer = _mapper.Map<Answer>(request);
            newAnswer.CreatedAt = DateTime.UtcNow;

            await repo.InsertAsync(newAnswer);

            var isSuccessful = await _unitOfWork.CommitAsync() > 0;
            if (!isSuccessful)
                throw new BadHttpRequestException("CreateFailed");

            return newAnswer.AnswerId;
        }

        public async Task<GetAnswerResponse> GetAnswerById(int id)
        {
            Answer answer = await _unitOfWork.GetRepository<Answer>().GetFirstOrDefaultAsync(
                predicate: x => x.AnswerId == id,
                include: q => q.Include(a => a.Question).Include(a => a.User),
                asNoTracking: true
            ) ?? throw new BadHttpRequestException("AnswerNotFound");

            return _mapper.Map<GetAnswerResponse>(answer);
        }

        public async Task<IPaginate<GetAnswerResponse>> ViewAllAnswers(AnswerFilter filter, PagingModel pagingModel)
        {
            IPaginate<GetAnswerResponse> response = await _unitOfWork.GetRepository<Answer>().GetPagingListAsync(
                selector: x => _mapper.Map<GetAnswerResponse>(x),
                filter: filter,
                orderBy: x => x.OrderByDescending(a => a.CreatedAt),
                include: q => q.Include(a => a.Question).Include(a => a.User),
                page: pagingModel.page,
                size: pagingModel.size
            );

            return response;
        }

        public async Task<bool> UpdateAnswerInfo(int id, AnswerInfo request)
        {
            var repo = _unitOfWork.GetRepository<Answer>();
            var answer = await repo.GetFirstOrDefaultAsync(
                predicate: x => x.AnswerId == id,
                asNoTracking: false
            ) ?? throw new BadHttpRequestException("AnswerNotFound");

            if (request.QuestionId.HasValue && request.QuestionId != answer.QuestionId)
            {
                var questionRepo = _unitOfWork.GetRepository<Question>();
                var question = await questionRepo.GetFirstOrDefaultAsync(
                    predicate: x => x.QuestionId == request.QuestionId.Value,
                    asNoTracking: true
                );
                if (question == null)
                    throw new BadHttpRequestException("QuestionNotFound");

                answer.QuestionId = request.QuestionId;
            }

            if (request.UserId.HasValue && request.UserId != answer.UserId)
            {
                var userRepo = _unitOfWork.GetRepository<User>();
                var user = await userRepo.GetFirstOrDefaultAsync(
                    predicate: x => x.UserId == request.UserId.Value,
                    asNoTracking: true
                );
                if (user == null)
                    throw new BadHttpRequestException("UserNotFound");

                answer.UserId = request.UserId;
            }

            if (!string.IsNullOrEmpty(request.Content))
                answer.Content = request.Content;

            bool isSuccessful = await _unitOfWork.CommitAsync() > 0;
            return isSuccessful;
        }

        public async Task<bool> DeleteAnswer(int id)
        {
            var repo = _unitOfWork.GetRepository<Answer>();
            var answer = await repo.GetFirstOrDefaultAsync(
                predicate: x => x.AnswerId == id,
                asNoTracking: false
            ) ?? throw new BadHttpRequestException("AnswerNotFound");

            repo.Delete(answer);
            bool isSuccessful = await _unitOfWork.CommitAsync() > 0;
            return isSuccessful;
        }
    }
}