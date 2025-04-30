using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace authService.Models
{
    public class User
    {
    public int Id { get; set; }

    [Required]
    [EmailAddress]
    public string Email { get; set; }

    [Required]
    [RegularExpression("^[a-zA-Z0-9]{4,20}$", ErrorMessage = "Username must be alphanumeric and 4-20 characters long.")]
    public string Username { get; set; }

    [Required]
    public string PasswordHash { get; set; }

    [Required]
    [RegularExpression("^(User|Admin)$", ErrorMessage = "Role must be either 'User' or 'Admin'.")]
    public string Role { get; set; }

    public DateTime CreatedAt { get; set; }
    }
}