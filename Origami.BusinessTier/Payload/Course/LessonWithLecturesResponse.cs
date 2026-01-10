using System.Collections.Generic;

namespace Origami.BusinessTier.Payload.Course
{
    public class LessonWithLecturesResponse
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public List<LectureItemResponse> Lectures { get; set; } = new();
        public int TotalLectures { get; set; }
        public int TotalLecturesCompleted { get; set; }
        public double? TotalMinutes { get; set; }
    }

    public class LectureItemResponse
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public string? Type { get; set; }
        public int? DurationMinutes { get; set; }
        public bool? PreviewAvailable { get; set; }
        public bool? IsCompleted { get; set; }
    }
}

