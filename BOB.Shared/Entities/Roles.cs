using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BOB.Shared.Entities
{
    public class Roles
    {
      
        public int Id { get; set; }  //Primary Key
        public string RoleName { get; set; } = string.Empty;
        public string RoleDescription { get; set; } = string.Empty;

        public string Company { get; set; } = string.Empty;

        public string Branch { get; set; } = string.Empty;

        // Navigation property to link to UserRoles
        public ICollection<UserRoles> UserRoles { get; set; } = new List<UserRoles>();

    }
}
