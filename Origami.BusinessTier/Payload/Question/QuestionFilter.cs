using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Origami.BusinessTier.Payload.Question
{
    public class QuestionFilter
    {
        public string? Content { get; set; }
        public int? CourseId { get; set; }
        public int? UserId { get; set; }
    }
}
