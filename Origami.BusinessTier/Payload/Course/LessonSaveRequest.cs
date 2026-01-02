using System.Collections.Generic;

namespace Origami.BusinessTier.Payload.Course
{
    public class LessonSaveRequest
    {
        public int? Id { get; set; } // Nếu có thì update, không có thì create
        public int CourseId { get; set; }
        public string? Title { get; set; }
        public List<LectureSaveRequest>? Lectures { get; set; }
    }
}

