using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Origami.BusinessTier.Payload.Auth
{
    public class HashPasswordResponse
    {
        public string Algorithm { get; set; } = "SHA256";
        public string Hash { get; set; } = null!;
    }
}
