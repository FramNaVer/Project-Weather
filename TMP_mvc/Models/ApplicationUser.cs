﻿using Microsoft.AspNetCore.Identity;

namespace TMP_mvc.Models
{
    public class ApplicationUser : IdentityUser
    {
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public ICollection<UserCity> UserCities { get; set; } = new List<UserCity>();

    }
}
