using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Origami.BusinessTier.Payload.Guide
{
    public class AddPromoPhotoRequest
    {
        public IFormFile PhotoFile { get; set; } = null!;
        public int DisplayOrder { get; set; }
    }
}
