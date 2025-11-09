using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Origami.BusinessTier.Payload.Favorite
{
    public class GetFavoriteResponse
    {
        public int FavoriteId { get; set; }
        public int UserId { get; set; }
        public int GuideId { get; set; }
        public DateTime? CreatedAt { get; set; }

        public string? GuideTitle { get; set; }
        public string? UserName { get; set; }
    }
}
