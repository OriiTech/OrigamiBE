using Origami.BusinessTier.Payload.Guide;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Origami.BusinessTier.Payload.Comment
{
    public class GuideCommentsResponse
    {
        public int TotalComments { get; set; }
        public List<CommentDto> Comment { get; set; } = new();
    }
    public class CommentDto
    {
        public int Id { get; set; }
        public string? Step { get; set; }   // nếu gắn với step
        public string? Comment { get; set; }
        public string AskName { get; set; } = null!;
        public DateTime? Date { get; set; }
        public List<CommentReplyDto> Reply { get; set; } = new();
    }
    public class CommentReplyDto
    {
        public string ResponseName { get; set; } = null!;
        public DateTime? Date { get; set; }
        public string? Comment { get; set; }
    }

}
