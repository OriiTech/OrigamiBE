using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Origami.BusinessTier.Payload.Lesson
{
    public class GetLessonResponse
    {
        public int LessonId { get; set; }
        public int? CourseId { get; set; }
        public string? CourseTitle { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public decimal? Price { get; set; }
        public DateTime? CreatedAt { get; set; }
    }
}