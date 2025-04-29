using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace OrderService.Dtos.Requests
{
public class UpdateOrderStatusRequestDto
    {
        [Required(ErrorMessage = "Status is required.")]
        [RegularExpression("^(Pending|Processing|Completed)$", ErrorMessage = "Status must be one of: Pending, Processing, Completed.")]
        public string Status { get; set; } 
    }
}