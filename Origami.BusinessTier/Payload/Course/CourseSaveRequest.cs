using System.Collections.Generic;

namespace Origami.BusinessTier.Payload.Course
{
    public class CourseSaveRequest
    {
        public string? Title { get; set; }
        public string? Subtitle { get; set; }
        public List<int>? InstructorIds { get; set; } // Nếu hỗ trợ multi-instructor, hiện tại dùng TeacherId
        public string? Description { get; set; }
        public List<string>? Objectives { get; set; }
        public List<int>? CategoryIds { get; set; }
        public List<int>? TargetLevelIds { get; set; }
        public string? Language { get; set; }
        public CoursePriceSaveDto? Price { get; set; }
        public PreviewSaveDto? Preview { get; set; }
    }

    public class CoursePriceSaveDto
    {
        public decimal? Amount { get; set; }
        public bool? PaidOnly { get; set; }
    }

    public class PreviewSaveDto
    {
        public string? VideoUrl { get; set; }
        public string? ThumbnailUrl { get; set; }
    }
}

