using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Origami.BusinessTier.Payload
{
    public class UploadRequest
    {
        public IFormFile File { get; set; } = null!;
        public string? Folder { get; set; }
    }
}
