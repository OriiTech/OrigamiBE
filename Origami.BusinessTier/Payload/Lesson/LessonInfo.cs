using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Origami.BusinessTier.Payload.Lesson
{
    public class LessonInfo
    {
        public int? CourseId { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
    }
}