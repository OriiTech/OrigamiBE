using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Origami.BusinessTier.Payload.Comment
{
    public class CommentFilter
    {
        public int? GuideId { get; set; }
        public int? UserId { get; set; }
        public DateTime? CreatedAt { get; set; }
        public int? ParentId { get; set; } 
    }
}
