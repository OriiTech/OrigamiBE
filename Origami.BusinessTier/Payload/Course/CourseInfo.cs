using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Origami.BusinessTier.Payload.Course
{
    public class CourseInfo
    {
        public string? Title { get; set; }
        public string? Description { get; set; }
        public decimal? Price { get; set; }
        public int? TeacherId { get; set; }
        public string? Language { get; set; } 
        public string? ThumbnailUrl { get; set; }
        public string? Subtitle { get; set; }
        public List<string>? Objectives { get; set; }
        public bool? PaidOnly { get; set; }
        public bool? Trending { get; set; }
        public string? PreviewVideoUrl { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public List<int>? CategoryIds { get; set; }
        public List<int>? TargetLevelIds { get; set; }
    }
}
