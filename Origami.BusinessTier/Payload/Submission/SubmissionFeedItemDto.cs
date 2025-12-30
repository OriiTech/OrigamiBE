using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Origami.BusinessTier.Payload.Submission
{
    public class SubmissionFeedItemDto
    {
        public int Id { get; set; }

        public SubmissionUserDto User { get; set; } = null!;

        public string Title { get; set; } = null!;
        public string? Description { get; set; }

        public List<SubmissionImageDto> Images { get; set; } = new();

        public FoldingDetailsDto FoldingDetails { get; set; } = null!;

        public SubmissionInteractionDto Interaction { get; set; } = null!;

        public SubmissionChallengeStatsDto? ChallengeStats { get; set; }

        public SubmissionMetadataDto Metadata { get; set; } = null!;
    }
    public class SubmissionUserDto
    {
        public int Id { get; set; }
        public string Username { get; set; } = null!;
        public string? Avatar { get; set; }
    }
    public class SubmissionImageDto
    {
        public string Url { get; set; } = null!;
        public string? Thumbnail { get; set; }
        public string? Note { get; set; }
        public int? Order { get; set; }
    }
    public class FoldingDetailsDto
    {
        public string? PaperSize { get; set; }
        public string? PaperType { get; set; }
        public int? Complexity { get; set; }
        public int? FoldingTimeMinute { get; set; }

        public string Source { get; set; } = null!; // original / adapted / traditional
        public string? OriginalDesigner { get; set; }
    }
    public class SubmissionInteractionDto
    {
        public int LikesCount { get; set; }
        public int CommentsCount { get; set; }
        public int ViewsCount { get; set; }

        public bool UserLiked { get; set; }
        public bool UserVoted { get; set; }

        public string ShareUrl { get; set; } = null!;
    }
    public class SubmissionChallengeStatsDto
    {
        public decimal? AverageScore { get; set; }

        public List<JudgeScoreDto> JudgeScores { get; set; } = new();
    }
    public class JudgeScoreDto
    {
        public int JudgeId { get; set; }
        public decimal Score { get; set; }

        public JudgeScoreCriteriaDto Criteria { get; set; } = null!;
    }
    public class JudgeScoreCriteriaDto
    {
        public decimal? Creativity { get; set; }
        public decimal? Execution { get; set; }
        public decimal? Theme { get; set; }
        public decimal? Difficulty { get; set; }
    }

    public class SubmissionMetadataDto
    {
        public DateTime? SubmittedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public string Status { get; set; } = null!; // pending / approved / flagged
        public bool IsSelf { get; set; }
        public bool IsTeam { get; set; }
    }

}
