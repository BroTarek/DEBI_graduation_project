using Microsoft.AspNetCore.Identity;
using System;
using Makanak.Domain.EnumsHelper.User;

namespace Makanak.Domain.Models.Identity
{
    public class ApplicationUser : IdentityUser
    {
        public UserStatus UserStatus { get; set; }
        public DateTime DateOfBirth { get; set; }
        public UserTypes UserType { get; set; }
        public string Name { get; set; }
    }
}
