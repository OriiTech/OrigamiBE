using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Origami.BusinessTier.Payload.User
{
    public class UpdateUserRoleResponse
    {
        public int UserId { get; set; }
        public string Username { get; set; } = null!;
        public string Email { get; set; } = null!;
        public int? OldRoleId { get; set; }
        public int? NewRoleId { get; set; }
        public string Message { get; set; } = null!;
    }
}