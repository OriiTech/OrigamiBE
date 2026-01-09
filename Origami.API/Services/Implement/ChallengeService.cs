using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Origami.API.Services.Implement;
using Origami.BusinessTier.Payload;
using Origami.BusinessTier.Payload.Challenge;
using Origami.BusinessTier.Payload.Guide;
using Origami.DataTier.Models;
using Origami.DataTier.Paginate;
using Origami.DataTier.Repository.Interfaces;
using System.Linq.Expressions;
using System.Text.Json;

namespace Origami.API.Services.Interfaces
{
    public class ChallengeService : BaseService<ChallengeService>, IChallengeService
    {
        private readonly IConfiguration _configuration;

        public ChallengeService(IUnitOfWork<OrigamiDbContext> unitOfWork, ILogger<ChallengeService> logger, IMapper mapper,
            IHttpContextAccessor httpContextAccessor, IConfiguration configuration) : base(unitOfWork, logger, mapper, httpContextAccessor)
        {
            _configuration = configuration;
        }

        public async Task<int> CreateNewChallenge(ChallengeInfo request)
        {
            var repo = _unitOfWork.GetRepository<Challenge>();

            if (request.CreatedBy.HasValue)
            {
                var userRepo = _unitOfWork.GetRepository<User>();
                var user = await userRepo.GetFirstOrDefaultAsync(
                    predicate: x => x.UserId == request.CreatedBy.Value,
                    asNoTracking: true
                );
                if (user == null)
                    throw new BadHttpRequestException("UserNotFound");
            }

            if (request.StartDate.HasValue && request.EndDate.HasValue)
            {
                if (request.EndDate.Value <= request.StartDate.Value)
                    throw new BadHttpRequestException("EndDateMustBeAfterStartDate");
            }

            var newChallenge = _mapper.Map<Challenge>(request);
            newChallenge.CreatedAt = DateTime.UtcNow;

            await repo.InsertAsync(newChallenge);

            var isSuccessful = await _unitOfWork.CommitAsync() > 0;
            if (!isSuccessful)
                throw new BadHttpRequestException("CreateFailed");

            return newChallenge.ChallengeId;
        }

        public async Task<int> CreateChallengeAsync(ChallengeCreateDto dto)
        {
            ValidateCreateChallenge(dto);

            var challengeRepo = _unitOfWork.GetRepository<Challenge>();
            var categoryRepo = _unitOfWork.GetRepository<Category>();

            await _unitOfWork.BeginTransactionAsync();

            try
            {
                var categoryNames = dto.Category
                    .Select(c => c.Trim().ToLower())
                    .ToList();

                var categories = await categoryRepo.GetAllAsync(
                    c => categoryNames.Contains(c.CategoryName.ToLower())
                );

                if (categories.Count != dto.Category.Count)
                    throw new BadHttpRequestException("CategoryNotFound");

                var challenge = new Challenge
                {
                    Title = dto.Title,
                    Description = dto.Summary,
                    PromoPhoto = dto.PromoPhoto,

                    Theme = dto.Theme,
                    Level = dto.Level,

                    PrizePool = dto.PrizePool,
                    IsFree = dto.IsFree,
                    EntryFee = dto.IsFree ? 0 : dto.EntryFee,

                    MaxTeamSize = dto.TeamSize,

                    Status = "upcoming",
                    Phase = "registration",

                    CreatedBy = GetCurrentUserId(),
                    CreatedAt = DateTime.UtcNow,

                    Categories = categories.ToList()
                };

                await challengeRepo.InsertAsync(challenge);
                await _unitOfWork.CommitAsync(); 


                challenge.ChallengeSchedule = new ChallengeSchedule
                {
                    RegistrationStart = dto.RegistrationStart,
                    SubmissionStart = dto.SubmissionStart,
                    SubmissionEnd = dto.SubmissionEnd,
                    VotingStart = dto.VotingStart,
                    VotingEnd = dto.VotingEnd,
                    ResultsDate = dto.ResultsDate
                };

                challenge.ChallengeRequirement = new ChallengeRequirement
                {
                    PaperRequirements = dto.PaperRequirements,
                    FoldingConstraints = dto.FoldingConstraints,
                    PhotographyRequirements = dto.PhotographyRequirements,
                    ModelRequirements = dto.ModelRequirements,
                    MaximumSubmissions = dto.MaximumSubmissions
                };

                if (dto.OtherRequirements.Any())
                {
                    challenge.ChallengeOtherRequirements = dto.OtherRequirements
                        .Select(r => new ChallengeOtherRequirement
                        {
                            Content = r
                        }).ToList();
                }

                if (dto.Rules.Any())
                {
                    challenge.ChallengeRules = dto.Rules.Select(r => new ChallengeRule
                    {
                        Section = r.Section,
                        ChallengeRuleItems = r.Items
                            .Split("||", StringSplitOptions.RemoveEmptyEntries)
                            .Select(i => new ChallengeRuleItem
                            {
                                Content = i.Trim()
                            }).ToList()
                    }).ToList();
                }

                await _unitOfWork.CommitAsync();
                await _unitOfWork.CommitTransactionAsync();

                return challenge.ChallengeId;
            }
            catch
            {
                await _unitOfWork.RollbackTransactionAsync();
                throw;
            }
        }
        private static void ValidateCreateChallenge(ChallengeCreateDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Title))
                throw new BadHttpRequestException("TitleRequired");

            if (!dto.IsFree && (!dto.EntryFee.HasValue || dto.EntryFee <= 0))
                throw new BadHttpRequestException("InvalidEntryFee");

            if (dto.SubmissionStart >= dto.SubmissionEnd)
                throw new BadHttpRequestException("InvalidSubmissionPeriod");

            if (dto.VotingStart.HasValue && dto.VotingEnd.HasValue &&
                dto.VotingStart >= dto.VotingEnd)
                throw new BadHttpRequestException("InvalidVotingPeriod");
        }


        public async Task<GetChallengeResponse> GetChallengeById(int id)
        {
            Challenge challenge = await _unitOfWork.GetRepository<Challenge>().GetFirstOrDefaultAsync(
                predicate: x => x.ChallengeId == id,
                include: q => q.Include(c => c.CreatedByNavigation),
                asNoTracking: true
            ) ?? throw new BadHttpRequestException("ChallengeNotFound");

            return _mapper.Map<GetChallengeResponse>(challenge);
        }

 
        private async Task<ChallengeDetailDto> LoadChallengeDetailBaseAsync(int challengeId)
        {
            var repo = _unitOfWork.GetRepository<Challenge>();

            var challenge = await repo.GetFirstOrDefaultAsync(
                predicate: c => c.ChallengeId == challengeId,
                include: q => q
                    .AsSplitQuery()
                    .Include(c => c.Categories)
                    .Include(c => c.CreatedByNavigation)
                        .ThenInclude(u => u.UserProfile)
                    .Include(c => c.Submissions)
                        .ThenInclude(s => s.Team)
                            .ThenInclude(t => t.TeamMembers)
                    .Include(c => c.Users)
                    .Include(c => c.ChallengeRules)
                        .ThenInclude(r => r.ChallengeRuleItems)
                    .Include(c => c.ChallengePrizes)
                        .ThenInclude(p => p.Badges)
                    .Include(c => c.ChallengeSchedule)
                    .Include(c => c.ChallengeRequirement)
                    .Include(c => c.ChallengeOtherRequirements)
                    .Include(c => c.Submissions)
                    
            ) ?? throw new BadHttpRequestException("ChallengeNotFound");
            var resolvedPhase = ResolveCurrentPhase(challenge.ChallengeSchedule);
            var totalParticipants = challenge.Submissions
                .SelectMany(s =>
                    s.TeamId != null
                        ? s.Team.TeamMembers.Select(tm => tm.UserId)
                        : new[] { s.SubmittedBy }
                )
                .Distinct()
                .Count();
            return new ChallengeDetailDto
            {
                Id = challenge.ChallengeId,
                Title = challenge.Title,
                Description = challenge.Description,

                PromoPhoto = new PromoPhotoDto
                {
                    Url = challenge.PromoPhoto
                },

                Organizer = new OrganizerDetailDto
                {
                    Id = challenge.CreatedByNavigation.UserId,
                    Name = challenge.CreatedByNavigation.Username,
                    Avatar = challenge.CreatedByNavigation.UserProfile?.AvatarUrl,
                    Bio = challenge.CreatedByNavigation.UserProfile?.Bio
                },

                Details = new ChallengeDetailInfoDto
                {
                    Status = challenge.Status,
                    Phase = resolvedPhase,
                    Level = challenge.Level,

                    Category = challenge.Categories.Select(c => c.CategoryName).ToList(),

                    PaperRequirements = challenge.ChallengeRequirement?.PaperRequirements,
                    FoldingConstraints = challenge.ChallengeRequirement?.FoldingConstraints,
                    PhotographyRequirements = challenge.ChallengeRequirement?.PhotographyRequirements,
                    ModelRequirements = challenge.ChallengeRequirement?.ModelRequirements,

                    OtherRequirements = challenge.ChallengeOtherRequirements.Select(r => r.Content).ToList(),

                    MaximumSubmissions = challenge.ChallengeRequirement?.MaximumSubmissions,
                    TeamSize = challenge.MaxTeamSize
                }
,

                Schedule = new ChallengeScheduleDto
                {
                    Dates = new ChallengeScheduleDatesDto
                    {
                        RegistrationStart = challenge.ChallengeSchedule?.RegistrationStart,
                        SubmissionStart = challenge.ChallengeSchedule?.SubmissionStart,
                        SubmissionEnd = challenge.ChallengeSchedule?.SubmissionEnd,
                        VotingStart = challenge.ChallengeSchedule?.VotingStart,
                        VotingEnd = challenge.ChallengeSchedule?.VotingEnd,
                        ResultsDate = challenge.ChallengeSchedule?.ResultsDate
                    }
                },

                Rules = challenge.ChallengeRules.Select(r => new ChallengeRuleDto
                {
                    Section = r.Section,
                    Items = r.ChallengeRuleItems.Select(i => i.Content).ToList()
                }).ToList(),

                Prize = new ChallengePrizeDto
                {
                    TotalPool = challenge.PrizePool,
                    Currency = true,
                    Ranks = challenge.ChallengePrizes.Select(p => new PrizeRankDto
                    {
                        Rank = p.Rank,
                        Cash = p.Cash,
                        Description = p.Description,
                        Badges = p.Badges.Select(b => new BadgeDto
                        {
                            Id = b.BadgeId,
                            Name = b.BadgeName,
                            //Icon = b.Icon
                        }).ToList()
                    }).ToList()
                },

                Judges = challenge.Users.Select(u => new JudgeDto
                {
                    Id = u.UserId,
                    Name = u.Username,
                    Role = "judge",
                    Avatar = u.UserProfile?.AvatarUrl,
                    Bio = u.UserProfile?.Bio
                }).ToList(),

                Stats = new ChallengeStatsDetailDto
                {
                    TotalParticipants = totalParticipants,
                    TotalSubmissions = challenge.Submissions.Count,
                    TotalViews = 0,
                    Rating = 0
                },

                Metadata = new ChallengeMetadataDto
                {
                    CreatedAt = challenge.CreatedAt,
                    UpdatedAt = challenge.UpdatedAt,
                    PublishedAt = challenge.CreatedAt
                }
            };
        }

        private async Task<ChallengeUserContextDto> BuildUserContextAsync(int challengeId,int? currentUserId)
        {
            if (currentUserId == null)
                return new ChallengeUserContextDto();

            var repo = _unitOfWork.GetRepository<Challenge>();

            var challenge = await repo.GetFirstOrDefaultAsync(
                predicate: c => c.ChallengeId == challengeId,
                include: q => q
                    .Include(c => c.Users)
                    .Include(c => c.Submissions)
                        .ThenInclude(s => s.Team)
                            .ThenInclude(t => t.TeamMembers)
            );

            if (challenge == null)
                return new ChallengeUserContextDto();

            return new ChallengeUserContextDto
            {
                IsOrganizer = challenge.CreatedBy == currentUserId,

                IsJudge = challenge.Users.Any(u => u.UserId == currentUserId),

                HasSubmissions = challenge.Submissions.Any(s =>
                    s.SubmittedBy == currentUserId
                    || (s.Team != null && s.Team.TeamMembers.Any(tm => tm.UserId == currentUserId))
                ),

                CanSubmit = challenge.Status == "active"
                            && challenge.Phase == "submission",

                CanVote = challenge.Phase == "voting"
            };
        }

        public async Task<ChallengeDetailDto> GetChallengeDetailAsync(int challengeId)
        {
            var baseDetail = await LoadChallengeDetailBaseAsync(challengeId);

            var currentUserId = GetCurrentUserId();

            baseDetail.UserContext =
                await BuildUserContextAsync(challengeId, currentUserId);

            return baseDetail;
        }

        public async Task AddJudgeToChallengeAsync(ChallengeJudgeCommandDto dto)
        {
            var challengeRepo = _unitOfWork.GetRepository<Challenge>();
            var userRepo = _unitOfWork.GetRepository<User>();

            var challenge = await challengeRepo.GetFirstOrDefaultAsync(
                predicate: c => c.ChallengeId == dto.ChallengeId,
                include: q => q.Include(c => c.Users),asNoTracking: false
            ) ?? throw new BadHttpRequestException("ChallengeNotFound");

            var user = await userRepo.GetFirstOrDefaultAsync(
                predicate: u => u.UserId == dto.UserId
            ) ?? throw new BadHttpRequestException("UserNotFound");

            if (challenge.Users.Any(u => u.UserId == dto.UserId))
                throw new BadHttpRequestException("UserAlreadyJudge");
            challenge.Users.Add(user);

            await _unitOfWork.CommitAsync();
        }

        public async Task RemoveJudgeFromChallengeAsync(ChallengeJudgeCommandDto dto)
        {
            var challengeRepo = _unitOfWork.GetRepository<Challenge>();

            await _unitOfWork.BeginTransactionAsync();

                var challenge = await challengeRepo.GetFirstOrDefaultAsync(
                    predicate: c => c.ChallengeId == dto.ChallengeId,
                    include: q => q.Include(c => c.Users),asNoTracking: false
                ) ?? throw new BadHttpRequestException("ChallengeNotFound");

                var judge = challenge.Users
                    .FirstOrDefault(u => u.UserId == dto.UserId);

                if (judge == null)
                    throw new BadHttpRequestException("JudgeNotFound");

                challenge.Users.Remove(judge);

                await _unitOfWork.CommitAsync();

        }


        public async Task<IPaginate<GetChallengeResponse>> ViewAllChallenges(ChallengeFilter filter, PagingModel pagingModel)
        {
            IPaginate<GetChallengeResponse> response = await _unitOfWork.GetRepository<Challenge>().GetPagingListAsync(
                selector: x => _mapper.Map<GetChallengeResponse>(x),
                filter: filter,
                orderBy: x => x.OrderByDescending(c => c.CreatedAt),
                include: q => q.Include(c => c.CreatedByNavigation),
                page: pagingModel.page,
                size: pagingModel.size
            );

            return response;
        }
        public async Task<IPaginate<ChallengeListItemDto>> GetChallengeListAsync(ChallengeListFilter filter,
            PagingModel pagingModel)
        {
            var repo = _unitOfWork.GetRepository<Challenge>();

            Expression<Func<Challenge, bool>> predicate = c =>
                (string.IsNullOrEmpty(filter.Keyword)
                    || c.Title.Contains(filter.Keyword)
                    || c.Description.Contains(filter.Keyword))
                && (string.IsNullOrEmpty(filter.Status) || c.Status == filter.Status)
                && (string.IsNullOrEmpty(filter.Phase) || c.Phase == filter.Phase)
                && (string.IsNullOrEmpty(filter.Level) || c.Level == filter.Level)
                && (!filter.IsFeatured.HasValue || c.IsFeatured == filter.IsFeatured.Value)
                && (!filter.IsFree.HasValue || c.IsFree == filter.IsFree.Value);

            var result = await repo.GetPagingListAsync(
                predicate: predicate,
                orderBy: q => q.OrderByDescending(c => c.CreatedAt),
                include: q => q
                    .Include(c => c.Categories)
                    .Include(c => c.CreatedByNavigation)
                        .ThenInclude(u => u.UserProfile)
                    .Include(c => c.Submissions),
                selector: c => new ChallengeListItemDto
                {
                    Id = c.ChallengeId,
                    Title = c.Title,
                    Summary = c.Description,

                    Status = c.Status,
                    Phase = c.Phase,

                    PromoPhoto = c.PromoPhoto,
                    Theme = c.Theme,
                    Level = c.Level,

                    Category = c.Categories
                        .Select(cat => cat.CategoryName)
                        .ToList(),

                    Organizer = new OrganizerDto
                    {
                        Id = c.CreatedByNavigation.UserId,
                        Name = c.CreatedByNavigation.Username,
                        AvatarUrl = c.CreatedByNavigation.UserProfile.AvatarUrl
                    },

                    Stats = new ChallengeStatsDto
                    {
                        ParticipantsCount = c.Submissions
                            .Select(s => s.TeamId)
                            .Distinct()
                            .Count(),

                        SubmissionsCount = c.Submissions.Count,

                        PrizePool = c.PrizePool
                    },

                    IsParticipating = c.Submissions.Any(s =>s.Team.TeamMembers.Any(tm => tm.UserId == GetCurrentUserId())),

                    IsFeatured = c.IsFeatured,
                    IsFree = c.IsFree,
                    EntryFee = c.EntryFee,

                    CreatedAt = c.CreatedAt,
                    UpdatedAt = c.UpdatedAt
                },
                page: pagingModel.page,
                size: pagingModel.size
            );

            return result;
        }

        public async Task<bool> UpdateChallengeInfo(int id, ChallengeInfo request)
        {
            var repo = _unitOfWork.GetRepository<Challenge>();
            var challenge = await repo.GetFirstOrDefaultAsync(
                predicate: x => x.ChallengeId == id,
                asNoTracking: false
            ) ?? throw new BadHttpRequestException("ChallengeNotFound");

            if (request.CreatedBy.HasValue && request.CreatedBy != challenge.CreatedBy)
            {
                var userRepo = _unitOfWork.GetRepository<User>();
                var user = await userRepo.GetFirstOrDefaultAsync(
                    predicate: x => x.UserId == request.CreatedBy.Value,
                    asNoTracking: true
                );
                if (user == null)
                    throw new BadHttpRequestException("UserNotFound");

                challenge.CreatedBy = request.CreatedBy;
            }

            var startDate = request.StartDate ?? challenge.StartDate;
            var endDate = request.EndDate ?? challenge.EndDate;
            if (startDate.HasValue && endDate.HasValue)
            {
                if (endDate.Value <= startDate.Value)
                    throw new BadHttpRequestException("EndDateMustBeAfterStartDate");
            }

            if (!string.IsNullOrEmpty(request.Title))
                challenge.Title = request.Title;
            if (!string.IsNullOrEmpty(request.Description))
                challenge.Description = request.Description;
            if (!string.IsNullOrEmpty(request.ChallengeType))
                challenge.ChallengeType = request.ChallengeType;
            if (request.StartDate.HasValue)
                challenge.StartDate = request.StartDate;
            if (request.EndDate.HasValue)
                challenge.EndDate = request.EndDate;
            if (request.MaxTeamSize.HasValue)
                challenge.MaxTeamSize = request.MaxTeamSize;
            challenge.IsTeamBased = request.IsTeamBased;

            bool isSuccessful = await _unitOfWork.CommitAsync() > 0;
            return isSuccessful;
        }

        public async Task<bool> DeleteChallenge(int id)
        {
            var repo = _unitOfWork.GetRepository<Challenge>();
            var challenge = await repo.GetFirstOrDefaultAsync(
                predicate: x => x.ChallengeId == id,
                include: q => q.Include(c => c.Leaderboards)
                              .Include(c => c.Submissions)
                              .Include(c => c.Teams),
                asNoTracking: false
            ) ?? throw new BadHttpRequestException("ChallengeNotFound");

            if (challenge.Leaderboards != null && challenge.Leaderboards.Any())
                throw new BadHttpRequestException("ChallengeHasLeaderboardsCannotDelete");
            if (challenge.Submissions != null && challenge.Submissions.Any())
                throw new BadHttpRequestException("ChallengeHasSubmissionsCannotDelete");
            if (challenge.Teams != null && challenge.Teams.Any())
                throw new BadHttpRequestException("ChallengeHasTeamsCannotDelete");

            repo.Delete(challenge);
            bool isSuccessful = await _unitOfWork.CommitAsync() > 0;
            return isSuccessful;
        }

        
        public async Task ApplyPrizePoolAsync(int challengeId)
        {
            var challengeRepo = _unitOfWork.GetRepository<Challenge>();
            var snapshotRepo = _unitOfWork.GetRepository<SubmissionSnapshot>();
            var prizeRepo = _unitOfWork.GetRepository<ChallengePrize>();
            var userRepo = _unitOfWork.GetRepository<User>();
            var balanceRepo = _unitOfWork.GetRepository<Wallet>();
            var userBadgeRepo = _unitOfWork.GetRepository<UserBadge>();

            var challenge = await challengeRepo.GetFirstOrDefaultAsync(
                predicate: c => c.ChallengeId == challengeId
            ) ?? throw new BadHttpRequestException("ChallengeNotFound");

            var snapshots = await snapshotRepo.GetListAsync(
                predicate: s => s.ChallengeId == challengeId
            );

            var prizes = await prizeRepo.GetListAsync(
                predicate: p => p.ChallengeId == challengeId
            );

            await _unitOfWork.BeginTransactionAsync();

            try
            {
                foreach (var prize in prizes)
                {
                    var winnerSnapshot = snapshots
                        .FirstOrDefault(s => s.Rank == prize.Rank);

                    if (winnerSnapshot == null)
                        continue;

                    var userId = winnerSnapshot.UserId;

                    // 1. Apply cash
                    if (prize.Cash > 0)
                    {
                        var balance = await balanceRepo.GetFirstOrDefaultAsync(
                            predicate: b => b.UserId == userId,
                            asNoTracking: false
                        );

                        balance.Balance += prize.Cash;
                    }

                }
                challenge.Status = "completed";

                await _unitOfWork.CommitAsync();
                await _unitOfWork.CommitTransactionAsync();
            }
            catch
            {
                await _unitOfWork.RollbackTransactionAsync();
                throw;
            }
        }

        private decimal CalculateCommunityScore(
            int votes,
            int likes,
            int comments,
            int maxVotes,
            int maxLikes,
            int maxComments)
        {
            decimal voteScore = maxVotes == 0 ? 0 : (decimal)votes / maxVotes;
            decimal likeScore = maxLikes == 0 ? 0 : (decimal)likes / maxLikes;
            decimal commentScore = maxComments == 0 ? 0 : (decimal)comments / maxComments;

            return (voteScore * 0.5m +
                    likeScore * 0.3m +
                    commentScore * 0.2m) * 100;
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

    }

}