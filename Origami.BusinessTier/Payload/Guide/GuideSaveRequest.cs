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

        /// <summary>Chọn Origami có sẵn. Dùng cái này hoặc OrigamiNew.</summary>
        public int? OrigamiId { get; set; }

        /// <summary>Tạo Origami mới nếu chưa có trong DB. Dùng khi OrigamiId null.</summary>
        public OrigamiNewDto? OrigamiNew { get; set; }

        public PriceDto Price { get; set; } = null!;
        public ProductPreviewDto? ProductPreview { get; set; }
        public List<StepSaveDto> Steps { get; set; } = new();
        public RequirementDto? Requirements { get; set; }
    }

    public class OrigamiNewDto
    {
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public string? ImageUrl { get; set; }
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
