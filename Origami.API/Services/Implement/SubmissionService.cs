// Origami.API/Services/Implement/SubmissionService.cs
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Origami.API.Services.Implement;
using Origami.API.Services.Interfaces;
using Origami.BusinessTier.Payload;
using Origami.BusinessTier.Payload.Challenge;
using Origami.BusinessTier.Payload.Submission;
using Origami.BusinessTier.Utils;
using Origami.DataTier.Models;
using Origami.DataTier.Paginate;
using Origami.DataTier.Repository.Interfaces;

namespace Origami.API.Services.Interfaces
{
    public class SubmissionService : BaseService<SubmissionService>, ISubmissionService
    {
        public SubmissionService(IUnitOfWork<OrigamiDbContext> uow, ILogger<SubmissionService> logger, IMapper mapper, IHttpContextAccessor hca)
            : base(uow, logger, mapper, hca) { }

        public async Task<int> CreateNewSubmission(SubmissionInfo request)
        {
            await EnsureChallengeExists(request.ChallengeId);
            if (request.TeamId.HasValue) await EnsureTeamExists(request.TeamId.Value);
            await EnsureUserExists(request.SubmittedBy);

            var entity = _mapper.Map<Submission>(request);
            entity.SubmittedAt = DateTime.UtcNow;

            await _unitOfWork.GetRepository<Submission>().InsertAsync(entity);
            if (await _unitOfWork.CommitAsync() <= 0) throw new BadHttpRequestException("CreateFailed");
            return entity.SubmissionId;
        }

        public async Task<GetSubmissionResponse> GetSubmissionById(int id)
        {
            var s = await _unitOfWork.GetRepository<Submission>().GetFirstOrDefaultAsync(
                predicate: x => x.SubmissionId == id,
                include: q => q.Include(x => x.Challenge)
                               .Include(x => x.Team)
                               .Include(x => x.SubmittedByNavigation),
                asNoTracking: true
            ) ?? throw new BadHttpRequestException("SubmissionNotFound");

            return _mapper.Map<GetSubmissionResponse>(s);
        }

        public Task<IPaginate<GetSubmissionResponse>> ViewAllSubmissions(SubmissionFilter filter, PagingModel paging)
        {
            return _unitOfWork.GetRepository<Submission>().GetPagingListAsync(
                selector: x => _mapper.Map<GetSubmissionResponse>(x),
                predicate: null,
                orderBy: q => q.OrderByDescending(x => x.SubmittedAt),
                include: q => q.Include(x => x.Challenge)
                               .Include(x => x.Team)
                               .Include(x => x.SubmittedByNavigation),
                page: paging.page,
                size: paging.size,
                filter: filter
            );
        }

        public async Task<bool> UpdateSubmission(int id, SubmissionInfo request)
        {
            var repo = _unitOfWork.GetRepository<Submission>();
            var entity = await repo.GetFirstOrDefaultAsync(
                predicate: x => x.SubmissionId == id,
                asNoTracking: false
            ) ?? throw new BadHttpRequestException("SubmissionNotFound");

            if (request.ChallengeId != entity.ChallengeId) await EnsureChallengeExists(request.ChallengeId);
            if (request.TeamId != entity.TeamId && request.TeamId.HasValue) await EnsureTeamExists(request.TeamId.Value);
            if (request.SubmittedBy != entity.SubmittedBy) await EnsureUserExists(request.SubmittedBy);

            entity.ChallengeId = request.ChallengeId;
            entity.TeamId = request.TeamId;
            entity.SubmittedBy = request.SubmittedBy;
            entity.FileUrl = request.FileUrl;

            return await _unitOfWork.CommitAsync() > 0;
        }

        public async Task<bool> DeleteSubmission(int id)
        {
            var repo = _unitOfWork.GetRepository<Submission>();
            var entity = await repo.GetFirstOrDefaultAsync(
                predicate: x => x.SubmissionId == id,
                asNoTracking: false
            ) ?? throw new BadHttpRequestException("SubmissionNotFound");

            repo.Delete(entity);
            return await _unitOfWork.CommitAsync() > 0;
        }

        public async Task<SubmissionFeedDto> LoadSubmissionFeedAsync( int challengeId,PagingModel paging)
        {
            var challengeRepo = _unitOfWork.GetRepository<Challenge>();
            var submissionRepo = _unitOfWork.GetRepository<Submission>();

            var challenge = await challengeRepo.GetFirstOrDefaultAsync(
                predicate: c => c.ChallengeId == challengeId,
                include: q => q.Include(c => c.ChallengeSchedule),
                asNoTracking: true
            ) ?? throw new BadHttpRequestException("ChallengeNotFound");

            var currentPhase = ResolveCurrentPhase(challenge.ChallengeSchedule);
            var currentUserId = GetCurrentUserId();

            var pagedSubmissions = await submissionRepo.GetPagingListAsync(
                predicate: s =>
                    s.ChallengeId == challengeId,
                    //&& s.Status == "approved",
                orderBy: q => q.OrderByDescending(s => s.SubmittedAt),
                include: q => q
                    .Include(s => s.SubmittedByNavigation)
                        .ThenInclude(u => u.UserProfile)
                    .Include(s => s.SubmissionImages)
                    .Include(s => s.SubmissionFoldingDetail)
                    .Include(s => s.SubmissionLikes)
                    .Include(s => s.SubmissionComments)
                    .Include(s => s.SubmissionViews)
                    .Include(s => s.Scores)
                        .ThenInclude(sc => sc.ScoreCriterion)
                    .Include(s => s.Votes),
                selector: s => new SubmissionFeedItemDto
                {
                    Id = s.SubmissionId,

                    User = new SubmissionUserDto
                    {
                        Id = s.SubmittedBy,
                        Username = s.SubmittedByNavigation.Username,
                        Avatar = s.SubmittedByNavigation.UserProfile.AvatarUrl
                    },

                    Title = s.Title,
                    Description = s.Description,

                    Images = s.SubmissionImages
                        .OrderBy(i => i.DisplayOrder)
                        .Select(i => new SubmissionImageDto
                        {
                            Url = i.Url,
                            Thumbnail = i.Thumbnail,
                            Note = i.Note,
                            Order = i.DisplayOrder
                        }).ToList(),

                    FoldingDetails = new FoldingDetailsDto
                    {
                        PaperSize = s.SubmissionFoldingDetail.PaperSize,
                        PaperType = s.SubmissionFoldingDetail.PaperType,
                        Complexity = s.SubmissionFoldingDetail.Complexity,
                        FoldingTimeMinute = s.SubmissionFoldingDetail.FoldingTimeMinute,
                        Source = s.SubmissionFoldingDetail.Source,
                        OriginalDesigner =
                            s.SubmissionFoldingDetail.Source != "original"
                                ? s.SubmissionFoldingDetail.OriginalDesigner
                                : null
                    },

                    Interaction = new SubmissionInteractionDto
                    {
                        LikesCount = s.SubmissionLikes.Count,
                        CommentsCount = s.SubmissionComments.Count,
                        ViewsCount = s.SubmissionViews.Count,

                        UserLiked = s.SubmissionLikes.Any(l => l.UserId == currentUserId),
                        UserVoted = s.Votes.Any(v => v.UserId == currentUserId),

                        ShareUrl = $"/challenges/{challengeId}/submissions/{s.SubmissionId}"
                    },

                    ChallengeStats = currentPhase != "submission"
                        ? new SubmissionChallengeStatsDto
                        {
                            AverageScore = s.Scores.Any()
                                ? s.Scores.Average(sc => sc.Score1)
                                : null,

                            JudgeScores = s.Scores.Select(sc => new JudgeScoreDto
                            {
                                JudgeId = sc.ScoreBy,
                                Score = sc.Score1,
                                Criteria = new JudgeScoreCriteriaDto
                                {
                                    Creativity = sc.ScoreCriterion.Creativity,
                                    Execution = sc.ScoreCriterion.Execution,
                                    Theme = sc.ScoreCriterion.Theme,
                                    Difficulty = sc.ScoreCriterion.Difficulty
                                }
                            }).ToList()
                        }
                        : null,

                    Metadata = new SubmissionMetadataDto
                    {
                        SubmittedAt = s.SubmittedAt,
                        UpdatedAt = s.UpdatedAt,
                        Status = s.Status,
                        IsSelf = s.SubmittedBy == currentUserId,
                        IsTeam = s.TeamId != null
                    }
                },
                page: paging.page,
                size: paging.size
            );

            var userContext = await BuildSubmissionFeedUserContextAsync(
                challenge,
                currentUserId
            );

            return new SubmissionFeedDto
            {
                ChallengeId = challenge.ChallengeId,
                ChallengeTitle = challenge.Title,
                CurrentPhase = currentPhase,
                VotingEndsIn =
                    currentPhase == "voting" && challenge.ChallengeSchedule?.VotingEnd != null
                        ? TimeFormatHelper.FormatTimeRemaining(
                            challenge.ChallengeSchedule.VotingEnd.Value
                        )
                        : null,
                Submissions = pagedSubmissions,
                UserContext = userContext
            };
        }

        public async Task<int> SaveSubmissionAsync(SubmissionSaveDto dto,bool isSubmit)
        {
            var submissionRepo = _unitOfWork.GetRepository<Submission>();
            var challengeRepo = _unitOfWork.GetRepository<Challenge>();

            var currentUserId = GetCurrentUserId();

            //Validate challenge 
            var challenge = await challengeRepo.GetFirstOrDefaultAsync(
                predicate: c => c.ChallengeId == dto.ChallengeId,
                include: q => q.Include(c => c.ChallengeSchedule),
                asNoTracking: true
            ) ?? throw new BadHttpRequestException("ChallengeNotFound");

            var currentPhase = ResolveCurrentPhase(challenge.ChallengeSchedule);

            if (isSubmit && currentPhase != "submission")
                throw new BadHttpRequestException("ChallengeNotInSubmissionPhase");

            //Load existing submission (draft or submitted)
            var submission = await submissionRepo.GetFirstOrDefaultAsync(
                predicate: s =>
                    s.ChallengeId == dto.ChallengeId &&
                    s.SubmittedBy == currentUserId,
                include: q => q
                    .Include(s => s.SubmissionImages)
                    .Include(s => s.SubmissionFoldingDetail),
                asNoTracking: false
            );

            var isNew = submission == null;

            if (isNew)
            {
                submission = new Submission
                {
                    ChallengeId = dto.ChallengeId,
                    SubmittedBy = (int)currentUserId,
                    Status = "draft"
                };

                await submissionRepo.InsertAsync(submission);
                await _unitOfWork.CommitAsync();
            }

            submission.Title = dto.Title;
            submission.Description = dto.Description;
            submission.UpdatedAt = DateTime.UtcNow;

            submission.TeamId = dto.IsTeam ? dto.TeamId : null;

            if (submission.SubmissionFoldingDetail == null)
            {
                submission.SubmissionFoldingDetail = new SubmissionFoldingDetail();
            }

            submission.SubmissionFoldingDetail.PaperSize = dto.FoldingDetails.PaperSize;
            submission.SubmissionFoldingDetail.PaperType = dto.FoldingDetails.PaperType;
            submission.SubmissionFoldingDetail.Complexity = dto.FoldingDetails.Complexity;
            submission.SubmissionFoldingDetail.FoldingTimeMinute =
                dto.FoldingDetails.FoldingTimeMinute;
            submission.SubmissionFoldingDetail.Source = dto.FoldingDetails.Source;
            submission.SubmissionFoldingDetail.OriginalDesigner =
                dto.FoldingDetails.Source != "original"
                    ? dto.FoldingDetails.OriginalDesigner
                    : null;

            submission.SubmissionImages.Clear();

            foreach (var img in dto.Images.OrderBy(i => i.Order))
            {
                submission.SubmissionImages.Add(new SubmissionImage
                {
                    Url = img.Url,
                    Thumbnail = img.Thumbnail,
                    Note = img.Note,
                    DisplayOrder = img.Order
                });
            }

            //Submit validation 
            if (isSubmit)
            {
                ValidateSubmissionForSubmit(submission);
                submission.Status = "submitted";
                submission.SubmittedAt = DateTime.UtcNow;
            }

            await _unitOfWork.CommitAsync();
            return submission.SubmissionId;
        }

        public async Task<PersonalRankingDto> GetPersonalRankingAsync(int challengeId)
        {
            var currentUserId = GetCurrentUserId();

            var submissionRepo = _unitOfWork.GetRepository<Submission>();
            var voteRepo = _unitOfWork.GetRepository<Vote>();

            var submission = await submissionRepo.GetFirstOrDefaultAsync(
                predicate: s =>
                    s.ChallengeId == challengeId &&
                    s.SubmittedBy == currentUserId &&
                    s.Status == "approved",
                include: q => q
                    .Include(s => s.SubmissionComments)
                    .Include(s => s.SubmissionViews),
                asNoTracking: true
            );

            if (submission == null)
            {
                return new PersonalRankingDto
                {
                    ChallengeId = challengeId,
                    HasSubmission = false,
                    Rank = null,
                    Score = null,
                    VotesReceived = 0,
                    CommentsReceived = 0,
                    SubmissionViews = 0
                };
            }

            var votesReceived = await voteRepo.CountAsync(
                v => v.SubmissionId == submission.SubmissionId
            );

            return new PersonalRankingDto
            {
                ChallengeId = challengeId,
                HasSubmission = true,

                // sau khi challenge finalize mới có
                Rank = null,
                Score = null,

                VotesReceived = votesReceived,
                CommentsReceived = submission.SubmissionComments.Count,
                SubmissionViews = submission.SubmissionViews.Count
            };
        }



        private void ValidateSubmissionForSubmit(Submission submission)
        {
            if (string.IsNullOrWhiteSpace(submission.Title))
                throw new BadHttpRequestException("TitleRequired");

            if (!submission.SubmissionImages.Any())
                throw new BadHttpRequestException("AtLeastOneImageRequired");

            var folding = submission.SubmissionFoldingDetail;

            if (folding == null)
                throw new BadHttpRequestException("FoldingDetailsRequired");

            if (folding.Source != "original"
                && string.IsNullOrWhiteSpace(folding.OriginalDesigner))
                throw new BadHttpRequestException("OriginalDesignerRequired");
        }

        private string ResolveCurrentPhase(ChallengeSchedule s)
        {
            var now = DateTime.UtcNow;

            if (s.RegistrationStart <= now && now < s.SubmissionStart)
                return "registration";

            if (s.SubmissionStart <= now && now < s.SubmissionEnd)
                return "submission";

            if (s.VotingStart <= now && now < s.VotingEnd)
                return "voting";

            if (s.ResultsDate <= now)
                return "results";

            return "upcoming";
        }
        private async Task<SubmissionFeedUserContextDto> BuildSubmissionFeedUserContextAsync(Challenge challenge, int? currentUserId)
        {
            if (currentUserId <= 0)
            {
                return new SubmissionFeedUserContextDto
                {
                    CanSubmit = false,
                    HasSubmissions = false,
                    CanVote = false,
                    IsJudge = false,
                    IsOrganizer = false
                };
            }

            var submissionRepo = _unitOfWork.GetRepository<Submission>();

            var hasSubmission = await submissionRepo.AnyAsync(
                s => s.ChallengeId == challenge.ChallengeId &&
                     (
                         s.SubmittedBy == currentUserId ||
                         (s.Team != null && s.Team.TeamMembers.Any(tm => tm.UserId == currentUserId))
                     )
            );

            var isOrganizer = challenge.CreatedBy == currentUserId;

            var isJudge = challenge.Users.Any(u => u.UserId == currentUserId);

            var currentPhase = ResolveCurrentPhase(challenge.ChallengeSchedule);

            return new SubmissionFeedUserContextDto
            {
                IsOrganizer = isOrganizer,
                IsJudge = isJudge,

                HasSubmissions = hasSubmission,

                CanSubmit =
                    currentPhase == "submission" &&
                    !hasSubmission &&
                    !isJudge,

                CanVote =
                    currentPhase == "voting" &&
                    !isOrganizer &&
                    !isJudge
            };
        }

        private async Task EnsureChallengeExists(int challengeId)
        {
            var ok = await _unitOfWork.GetRepository<Challenge>().AnyAsync(x => x.ChallengeId == challengeId);
            if (!ok) throw new BadHttpRequestException("ChallengeNotFound");
        }
        private async Task EnsureTeamExists(int teamId)
        {
            var ok = await _unitOfWork.GetRepository<Team>().AnyAsync(x => x.TeamId == teamId);
            if (!ok) throw new BadHttpRequestException("TeamNotFound");
        }
        private async Task EnsureUserExists(int userId)
        {
            var ok = await _unitOfWork.GetRepository<User>().AnyAsync(x => x.UserId == userId);
            if (!ok) throw new BadHttpRequestException("UserNotFound");
        }
    }
}