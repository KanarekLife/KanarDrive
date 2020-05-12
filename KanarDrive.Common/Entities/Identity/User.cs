using System;
using System.Collections.Generic;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;

namespace KanarDrive.Common.Entities.Identity
{
    public class User : IdentityUser
    {
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public int AvailableCloudSpace { get; set; }
        public IEnumerable<Guid> Roles { get; set; }
        public IEnumerable<Claim> Claims { get; set; }
    }
}