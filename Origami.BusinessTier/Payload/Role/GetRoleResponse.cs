using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Origami.BusinessTier.Payload.Role
{
    public class GetRoleResponse
    {
        public int RoleId { get; set; }
        public string RoleName { get; set; } = null!;
    }
}