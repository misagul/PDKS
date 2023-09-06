﻿namespace PDKS.Models
{
    public class User
    {
        public Guid Id { get; set; }
        public string? Username { get; set; }
        public string? Password { get; set; }
        public int IsActive { get; set; }

        public User()
        {
            IsActive = 1;
        }
    }
}
