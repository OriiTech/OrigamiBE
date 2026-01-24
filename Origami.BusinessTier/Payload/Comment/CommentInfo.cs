using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Origami.BusinessTier.Payload.Comment
{
    public class CommentInfo
    {
        public int GuideId { get; set; }
        public string? Content { get; set; }
        public int? ParentId { get; set; }
    }
}
