﻿using Microsoft.AspNetCore.Identity;

namespace RentACar.App.Domain
{
    public class User : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PIN { get; set; }
    }
}
