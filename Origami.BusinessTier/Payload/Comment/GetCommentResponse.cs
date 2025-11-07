using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Origami.BusinessTier.Payload.Comment
{
    public class GetCommentResponse
    {
        public int CommentId { get; set; }
        public int GuideId { get; set; }
        public int UserId { get; set; }
        public string? Content { get; set; }
        public DateTime? CreatedAt { get; set; }
        public int? ParentId { get; set; }

        public string? UserName { get; set; }
        public string? GuideTitle { get; set; }

        public List<GetCommentResponse>? Replies { get; set; }
    }
}
