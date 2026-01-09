using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Origami.BusinessTier.Payload.Submission
{
    public class SubmissionSaveDto
    {
        public int ChallengeId { get; set; }

        public string Title { get; set; } = null!;

        public string? Description { get; set; }


        public List<SubmissionImageSaveDto> Images { get; set; } = new();


        public FoldingDetailsSaveDto FoldingDetails { get; set; } = null!;


        public bool IsTeam { get; set; }

        public int? TeamId { get; set; }
    }
    public class SubmissionImageSaveDto
    {
        public string Url { get; set; } = null!;

        public string? Thumbnail { get; set; }

        public string? Note { get; set; }

        public int Order { get; set; }
    }
    public class FoldingDetailsSaveDto
    {
        public string? PaperSize { get; set; }

        public string? PaperType { get; set; }

        public int? Complexity { get; set; }

        public int? FoldingTimeMinute { get; set; }

        public string Source { get; set; } = "original";

        public string? OriginalDesigner { get; set; }
    }

}
