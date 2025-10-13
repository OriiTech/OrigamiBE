using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Origami.BusinessTier.Payload.User
{
    public class UserFilter
    {
        public string? Username { get; set; } = null!;

        public string? Email { get; set; } = null!;
    }
}
