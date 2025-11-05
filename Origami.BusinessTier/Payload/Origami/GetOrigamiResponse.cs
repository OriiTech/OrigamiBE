using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Origami.BusinessTier.Payload.Origami
{
    public class GetOrigamiResponse
    {
        public string Name { get; set; } = null!;

        public string? Description { get; set; }

        public string? ImageUrl { get; set; }

        public DateTime? CreatedAt { get; set; }

        public int CreatedBy { get; set; }
    }
}
