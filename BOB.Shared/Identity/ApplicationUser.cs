using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace BOB.Shared.Identity
{
    public class ApplicationUser : IdentityUser
    {
        public string Company { get; set; } = string.Empty;
        public string Branch { get; set; } = string.Empty;


    }
}
