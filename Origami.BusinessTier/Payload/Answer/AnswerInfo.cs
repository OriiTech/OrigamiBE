using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Origami.BusinessTier.Payload.Answer
{
    public class AnswerInfo
    {
        public int? QuestionId { get; set; }
        public int? UserId { get; set; }
        public string? Content { get; set; }
    }
}