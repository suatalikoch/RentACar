﻿namespace RentACar.App.Models.Users
{
    public class UserDetailsViewModel
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PIN {  get; set; }
        public string PhoneNumber { get; set; }
        public string PasswordHash { get; set; }
    }
}