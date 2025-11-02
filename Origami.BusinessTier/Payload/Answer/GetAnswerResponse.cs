using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Origami.BusinessTier.Payload.Answer
{
    public class GetAnswerResponse
    {
        public int AnswerId { get; set; }
        public int? QuestionId { get; set; }
        public string? QuestionContent { get; set; }
        public int? UserId { get; set; }
        public string? UserName { get; set; }
        public string? Content { get; set; }
        public DateTime? CreatedAt { get; set; }
    }
}
