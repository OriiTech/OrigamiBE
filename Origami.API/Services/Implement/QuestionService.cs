using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Origami.API.Services.Implement;
using Origami.BusinessTier.Payload;
using Origami.BusinessTier.Payload.Question;
using Origami.DataTier.Models;
using Origami.DataTier.Paginate;
using Origami.DataTier.Repository.Interfaces;

namespace Origami.API.Services.Interfaces
{
    public class QuestionService : BaseService<QuestionService>, IQuestionService
    {
        private readonly IConfiguration _configuration;

        public QuestionService(IUnitOfWork<OrigamiDbContext> unitOfWork, ILogger<QuestionService> logger, IMapper mapper,
            IHttpContextAccessor httpContextAccessor, IConfiguration configuration) : base(unitOfWork, logger, mapper, httpContextAccessor)
        {
            _configuration = configuration;
        }

        public async Task<int> CreateNewQuestion(QuestionInfo request)
        {
            var repo = _unitOfWork.GetRepository<Question>();

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

            var newQuestion = _mapper.Map<Question>(request);
            newQuestion.CreatedAt = DateTime.UtcNow;

            await repo.InsertAsync(newQuestion);

            var isSuccessful = await _unitOfWork.CommitAsync() > 0;
            if (!isSuccessful)
                throw new BadHttpRequestException("CreateFailed");

            return newQuestion.QuestionId;
        }

        public async Task<GetQuestionResponse> GetQuestionById(int id)
        {
            Question question = await _unitOfWork.GetRepository<Question>().GetFirstOrDefaultAsync(
                predicate: x => x.QuestionId == id,
                include: q => q.Include(q => q.Course).Include(q => q.User),
                asNoTracking: true
            ) ?? throw new BadHttpRequestException("QuestionNotFound");

            return _mapper.Map<GetQuestionResponse>(question);
        }

        public async Task<IPaginate<GetQuestionResponse>> ViewAllQuestions(QuestionFilter filter, PagingModel pagingModel)
        {
            IPaginate<GetQuestionResponse> response = await _unitOfWork.GetRepository<Question>().GetPagingListAsync(
                selector: x => _mapper.Map<GetQuestionResponse>(x),
                filter: filter,
                orderBy: x => x.OrderByDescending(q => q.CreatedAt),
                include: q => q.Include(q => q.Course).Include(q => q.User),
                page: pagingModel.page,
                size: pagingModel.size
            );

            return response;
        }

        public async Task<bool> UpdateQuestionInfo(int id, QuestionInfo request)
        {
            var repo = _unitOfWork.GetRepository<Question>();
            var question = await repo.GetFirstOrDefaultAsync(
                predicate: x => x.QuestionId == id,
                asNoTracking: false
            ) ?? throw new BadHttpRequestException("QuestionNotFound");

            if (request.CourseId.HasValue && request.CourseId != question.CourseId)
            {
                var courseRepo = _unitOfWork.GetRepository<Course>();
                var course = await courseRepo.GetFirstOrDefaultAsync(
                    predicate: x => x.CourseId == request.CourseId.Value,
                    asNoTracking: true
                );
                if (course == null)
                    throw new BadHttpRequestException("CourseNotFound");

                question.CourseId = request.CourseId;
            }

            if (request.UserId.HasValue && request.UserId != question.UserId)
            {
                var userRepo = _unitOfWork.GetRepository<User>();
                var user = await userRepo.GetFirstOrDefaultAsync(
                    predicate: x => x.UserId == request.UserId.Value,
                    asNoTracking: true
                );
                if (user == null)
                    throw new BadHttpRequestException("UserNotFound");

                question.UserId = request.UserId.Value;
            }

            if (!string.IsNullOrEmpty(request.Content))
                question.Content = request.Content;

            bool isSuccessful = await _unitOfWork.CommitAsync() > 0;
            return isSuccessful;
        }

        public async Task<bool> DeleteQuestion(int id)
        {
            var repo = _unitOfWork.GetRepository<Question>();
            var question = await repo.GetFirstOrDefaultAsync(
                predicate: x => x.QuestionId == id,
                include: q => q.Include(q => q.Answers),
                asNoTracking: false
            ) ?? throw new BadHttpRequestException("QuestionNotFound");

            if (question.Answers != null && question.Answers.Any())
                throw new BadHttpRequestException("QuestionHasAnswersCannotDelete");

            repo.Delete(question);
            bool isSuccessful = await _unitOfWork.CommitAsync() > 0;
            return isSuccessful;
        }
    }
}