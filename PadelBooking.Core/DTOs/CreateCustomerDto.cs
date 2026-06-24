using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PadelBooking.Core.DTOs
{
    /// <summary>
    /// DTO för att skapa en ny kund
    /// </summary>
    public class CreateCustomerDto
    {
        [Required(ErrorMessage = "Ange förnamn")]
        [StringLength(50, MinimumLength = 2)]
        public string FirstName { get; set; } = string.Empty;
        [Required(ErrorMessage = "Ange efternamn")]
        [StringLength(50, MinimumLength = 2)]
        public string LastName { get; set; } = string.Empty;
        [Required]
        [Phone]
        public string PhoneNumber { get; set; } = string.Empty;
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty; 


    }
}
