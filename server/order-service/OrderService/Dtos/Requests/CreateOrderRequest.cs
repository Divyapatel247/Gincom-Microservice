using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace OrderService.Dtos.Requests
{
    public class CreateOrderRequest
    {
        [Required(ErrorMessage = "User email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        [StringLength(254, ErrorMessage = "Email cannot exceed 254 characters.")]
        public string UserEmail { get; set; } = string.Empty;
        
    }
}