using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Origami.BusinessTier.Payload.Category
{
    public class GetCategoryResponse
    {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; } = null!;
        public List<string>? Guides { get; set; }
    }
}
