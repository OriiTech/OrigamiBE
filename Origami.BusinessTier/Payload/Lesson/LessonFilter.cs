using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Origami.BusinessTier.Payload.Lesson
{
    public class LessonFilter
    {
        public string? Title { get; set; }
        public int? CourseId { get; set; }
    }
}
