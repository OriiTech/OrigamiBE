using System.Collections.Generic;

namespace Origami.BusinessTier.Payload.Course
{
    public class LectureSaveRequest
    {
        public int? Id { get; set; } // Nếu có thì update, không có thì create
        public int LessonId { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? Type { get; set; } // video/text/quiz/exam/...
        public int? DurationMinutes { get; set; }
        public bool? PreviewAvailable { get; set; }
        public string? ContentUrl { get; set; } // URL của file video/content
        public string? Note { get; set; }
        public List<ResourceSaveRequest>? Resources { get; set; }
    }
}

