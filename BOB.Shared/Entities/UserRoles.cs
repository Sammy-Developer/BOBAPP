using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BOB.Shared.Entities
{
    public class UserRoles
    {
        public int UserId { get; set; } //FK to Users
        public Users User { get; set; } = null!;
        public int RoleId { get; set; } //FK to Users
        public Roles Role { get; set; } = null!;
        public string Company { get; set; } = string.Empty;
        public string Branch { get; set; } = string.Empty;
    }

}
