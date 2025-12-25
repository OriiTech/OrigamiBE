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

        public async Task<GetChallengeResponse> GetChallengeById(int id)
        {
            Challenge challenge = await _unitOfWork.GetRepository<Challenge>().GetFirstOrDefaultAsync(
                predicate: x => x.ChallengeId == id,
                include: q => q.Include(c => c.CreatedByNavigation),
                asNoTracking: true
            ) ?? throw new BadHttpRequestException("ChallengeNotFound");

            return _mapper.Map<GetChallengeResponse>(challenge);
        }

        //public async Task<ChallengeDetailDto> GetChallengeDetailAsync(int challengeId)
        //{
        //    var repo = _unitOfWork.GetRepository<Challenge>();

        //    var challenge = await repo.GetFirstOrDefaultAsync(
        //        predicate: c => c.ChallengeId == challengeId,
        //        include: q => q
        //            .Include(c => c.Categories)
        //            .Include(c => c.CreatedByNavigation)
        //                .ThenInclude(u => u.UserProfile)
        //            .Include(c => c.Submissions)
        //                .ThenInclude(s => s.Team)
        //                    .ThenInclude(t => t.TeamMembers)
        //            .Include(c => c.Users)
        //                .ThenInclude(u => u.UserProfile)
        //            .Include(c => c.ChallengeRules)
        //                .ThenInclude(r => r.ChallengeRuleItems)
        //            .Include(c => c.ChallengePrizes)
        //                .ThenInclude(p => p.Badges)
        //            .Include(c => c.ChallengeSchedule)
        //    ) ?? throw new BadHttpRequestException("ChallengeNotFound");

        //    var phase = ResolveCurrentPhase(challenge.ChallengeSchedule);

        //    // Update runtime phase
        //    challenge.Phase = phase;

        //    var totalParticipants = challenge.Submissions
        //        .SelectMany(s =>
        //            s.Team != null
        //                ? s.Team.TeamMembers.Select(tm => tm.UserId)
        //                : new[] { s.SubmittedBy }
        //        )
        //        .Distinct()
        //        .Count();

        //    return new ChallengeDetailDto
        //    {
        //        Id = challenge.ChallengeId,
        //        Title = challenge.Title,
        //        Description = challenge.Description,

        //        PromoPhoto = new PromoPhotoDto
        //        {
        //            Url = challenge.PromoPhoto
        //        },

        //        Organizer = new OrganizerDetailDto
        //        {
        //            Id = challenge.CreatedByNavigation.UserId,
        //            Name = challenge.CreatedByNavigation.Username,
        //            Avatar = challenge.CreatedByNavigation.UserProfile?.AvatarUrl,
        //            Bio = challenge.CreatedByNavigation.UserProfile?.Bio
        //        },

        //        Details = new ChallengeDetailInfoDto
        //        {
        //            Status = challenge.Status,
        //            Phase = phase,
        //            Level = challenge.Level,
        //            Category = challenge.Categories
        //                .Select(c => c.CategoryName)
        //                .ToList(),

        //            PaperRequirements = null,
        //            FoldingConstraints = null,
        //            PhotographyRequirements = null,
        //            ModelRequirements = null,
        //            OtherRequirements = new(),

        //            MaximumSubmissions = null,
        //            TeamSize = challenge.MaxTeamSize
        //        },

        //        Schedule = challenge.ChallengeSchedule == null
        //            ? null
        //            : new ChallengeScheduleDto
        //            {
        //                Dates = new ChallengeScheduleDatesDto
        //                {
        //                    RegistrationStart = challenge.ChallengeSchedule.RegistrationStart,
        //                    SubmissionStart = challenge.ChallengeSchedule.SubmissionStart,
        //                    SubmissionEnd = challenge.ChallengeSchedule.SubmissionEnd,
        //                    VotingStart = challenge.ChallengeSchedule.VotingStart,
        //                    VotingEnd = challenge.ChallengeSchedule.VotingEnd,
        //                    ResultsDate = challenge.ChallengeSchedule.ResultsDate
        //                },
        //                //Timeline = null // DB chưa support
        //            },

        //        Rules = challenge.ChallengeRules.Select(r => new ChallengeRuleDto
        //        {
        //            Section = r.Section,
        //            Items = r.ChallengeRuleItems
        //                .Select(i => i.Content)
        //                .ToList()
        //        }).ToList(),

        //        Prize = new ChallengePrizeDto
        //        {
        //            TotalPool = challenge.PrizePool,
        //            Currency = true,
        //            Ranks = challenge.ChallengePrizes
        //                .OrderBy(p => p.Rank)
        //                .Select(p => new PrizeRankDto
        //                {
        //                    Rank = p.Rank,
        //                    Cash = p.Cash,
        //                    Description = p.Description,
        //                    Badges = p.Badges.Select(b => new BadgeDto
        //                    {
        //                        Id = b.BadgeId,
        //                        Name = b.BadgeName,
        //                        //Icon = b.Icon
        //                    }).ToList()
        //                }).ToList()
        //        },

        //        Judges = challenge.Users.Select(u => new JudgeDto
        //        {
        //            Id = u.UserId,
        //            Name = u.Username,
        //            Role = "judge",
        //            Avatar = u.UserProfile?.AvatarUrl,
        //            Bio = u.UserProfile?.Bio
        //        }).ToList(),

        //        UserContext = new ChallengeUserContextDto
        //        {
        //            CanSubmit = phase == "submission",
        //            HasSubmissions = challenge.Submissions.Any(),
        //            CanVote = phase == "voting",
        //            IsJudge = challenge.Users.Any(u => u.UserId == GetCurrentUserId()),
        //            IsOrganizer = challenge.CreatedBy == GetCurrentUserId()
        //        },

        //        Stats = new ChallengeStatsDetailDto
        //        {
        //            TotalParticipants = totalParticipants,
        //            TotalSubmissions = challenge.Submissions.Count,
        //            TotalViews = 0,
        //            Rating = 0
        //        },

        //        Metadata = new ChallengeMetadataDto
        //        {
        //            CreatedAt = challenge.CreatedAt,
        //            UpdatedAt = challenge.UpdatedAt,
        //            PublishedAt = challenge.CreatedAt
        //        }
        //    };
        //}

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

            var currentUserId = 3; //GetCurrentUserId();

            baseDetail.UserContext =
                await BuildUserContextAsync(challengeId, currentUserId);

            return baseDetail;
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