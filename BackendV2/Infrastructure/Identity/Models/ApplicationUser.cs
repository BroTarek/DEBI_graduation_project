using System;
using Microsoft.AspNetCore.Identity;
using YouTubeClone.Domain.EnumsHelper.User;

namespace YouTubeClone.Domain.Models.Identity
{
    public class ApplicationUser : IdentityUser
    {
        public UserStatus UserStatus { get; set; }
        public DateTime DateOfBirth { get; set; }
        public UserTypes UserType { get; set; }
        public string Name { get; set; }
    }
}
