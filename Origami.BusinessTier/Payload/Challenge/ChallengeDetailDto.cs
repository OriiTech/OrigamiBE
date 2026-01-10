using Origami.BusinessTier.Payload.Guide;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Origami.BusinessTier.Payload.Challenge
{
    public class ChallengeDetailDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }

        public PromoPhotoDto PromoPhoto { get; set; }
        public OrganizerDetailDto Organizer { get; set; }

        public ChallengeDetailInfoDto Details { get; set; }
        public ChallengeScheduleDto Schedule { get; set; }

        public List<ChallengeRuleDto> Rules { get; set; }

        public ChallengePrizeDto Prize { get; set; }
        public List<JudgeDto> Judges { get; set; }

        public ChallengeUserContextDto UserContext { get; set; }
        public ChallengeStatsDetailDto Stats { get; set; }

        public ChallengeMetadataDto Metadata { get; set; }
    }
    public class ChallengeScheduleDto
    {
        public ChallengeScheduleDatesDto Dates { get; set; }
        //public List<ChallengePhaseDto> Timeline { get; set; }
    }

    public class ChallengeScheduleDatesDto
    {
        public DateTime? RegistrationStart { get; set; }
        public DateTime? SubmissionStart { get; set; }
        public DateTime? SubmissionEnd { get; set; }
        public DateTime? VotingStart { get; set; }
        public DateTime? VotingEnd { get; set; }
        public DateTime? ResultsDate { get; set; }
    }
    public class ChallengePhaseDto
    {
        public string Phase { get; set; }
        public string Title { get; set; }
        public DateTime? Start { get; set; }
        public DateTime? End { get; set; }
        public bool IsCurrent { get; set; }
        public string Description { get; set; }
    }
    public class ChallengeRuleDto
    {
        public string Section { get; set; }
        public List<string> Items { get; set; }
    }
    public class ChallengePrizeDto
    {
        public decimal? TotalPool { get; set; }
        public bool Currency { get; set; }
        public List<PrizeRankDto> Ranks { get; set; }
    }

    public class PrizeRankDto
    {
        public int Rank { get; set; }
        public decimal? Cash { get; set; }
        public string Description { get; set; }
        public List<BadgeDto> Badges { get; set; }
    }

    public class BadgeDto
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Icon { get; set; }
    }
    public class JudgeDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Role { get; set; }
        public string Avatar { get; set; }
        public string Bio { get; set; }
    }
    public class ChallengeUserContextDto
    {
        public bool CanSubmit { get; set; }
        public bool HasSubmissions { get; set; }
        public bool CanVote { get; set; }

        public bool IsJudge { get; set; }
        public bool IsOrganizer { get; set; }
    }
    public class ChallengeStatsDetailDto
    {
        public int TotalParticipants { get; set; }
        public int TotalSubmissions { get; set; }
        public int TotalViews { get; set; }
        public double Rating { get; set; }
    }
    public class ChallengeMetadataDto
    {
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public DateTime? PublishedAt { get; set; }
    }
    public class RelatedContentDto
    {
        public List<RelatedGuideDto> Guides { get; set; }
        public List<RelatedChallengeDto> OtherChallenges { get; set; }
    }

    public class RelatedGuideDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string PromoPhoto { get; set; }
        public int InstructorId { get; set; }
        public string InstructorName { get; set; }
    }

    public class RelatedChallengeDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Summary { get; set; }
        public string Status { get; set; }
        public string Round { get; set; }
        public string PromoPhoto { get; set; }
        public string Theme { get; set; }
        public string Level { get; set; }
    }
    public class ChallengeDetailInfoDto
    {
        public string Status { get; set; }
        public string Phase { get; set; }
        public string Level { get; set; }

        public List<string> Category { get; set; } = new();

        public string PaperRequirements { get; set; }
        public string FoldingConstraints { get; set; }
        public string PhotographyRequirements { get; set; }
        public string ModelRequirements { get; set; }

        public List<string> OtherRequirements { get; set; } = new();

        public int? MaximumSubmissions { get; set; }
        public int? TeamSize { get; set; }
    }
    public class ChallengeDetailResponse
    {
        public ChallengeDetailDto ChallengeDetail { get; set; }
        public RelatedContentDto RelatedContent { get; set; }
    }

    public class OrganizerDetailDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Avatar { get; set; }
        public string? Bio { get; set; }
    }

}
