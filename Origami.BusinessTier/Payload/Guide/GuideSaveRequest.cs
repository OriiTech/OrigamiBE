using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Origami.BusinessTier.Payload.Guide
{
    public class GuideSaveRequest
    {
        public string Title { get; set; } = null!;
        public string? Subtitle { get; set; }
        public string? Description { get; set; }


        public string? Level { get; set; }

        public List<string> Category { get; set; } = new();

        public PriceDto Price { get; set; } = null!;
        public ProductPreviewDto? ProductPreview { get; set; }

        public List<StepSaveDto> Steps { get; set; } = new();

        public RequirementDto? Requirements { get; set; }
    }
    public class StepSaveDto
    {
        public int Order { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }

        public List<string> Tips { get; set; } = new();

        public List<StepMediaDto> Medias { get; set; } = new();
    }

    public class StepMediaDto
    {
        public int Order { get; set; }
        public string Url { get; set; } = null!;
        public string? Note { get; set; }
        public int Type { get; set; }
    }

}
