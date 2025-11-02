using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Origami.BusinessTier.Payload.Question
{
    public class GetQuestionResponse
    {
        public int QuestionId { get; set; }
        public int? CourseId { get; set; }
        public string? CourseTitle { get; set; }
        public int? UserId { get; set; }
        public string? UserName { get; set; }
        public string? Content { get; set; }
        public DateTime? CreatedAt { get; set; }
    }
}
