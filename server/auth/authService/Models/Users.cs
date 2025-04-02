using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace authService.Models
{
    public class Users
    {
        public int Id { get; set; } 
        public string Username { get; set; }
        public string PasswordHash { get; set; } 

        public string Role { get; set; } // "User" or "Admin"
        public DateTime CreatedAt { get; set; }
    }
}