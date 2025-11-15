using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Origami.BusinessTier.Payload.User
{
    public class UpdateUserRoleRequest
    {
        public int UserId { get; set; }
        public int NewRoleId { get; set; }
    }
}
