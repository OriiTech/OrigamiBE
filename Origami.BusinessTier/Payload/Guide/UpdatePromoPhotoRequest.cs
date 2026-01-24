using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Origami.BusinessTier.Payload.Guide
{
    public class UpdatePromoPhotoRequest
    {
        public IFormFile? PhotoFile { get; set; }
        public int? DisplayOrder { get; set; }
    }
}
