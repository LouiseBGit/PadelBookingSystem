using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PadelBooking.Core.DTOs
{
    /// <summary>
    /// DTO för att skapa en bokning
    /// </summary>
    public class CreateBookingDto
    {
        //banan som ska bokas
        [Range(1, 3, ErrorMessage = "Numret på banan måste vara mellan 1-3")]
        public int CourtNumber { get; set; }
        //Starttid för bokningen
        [Required(ErrorMessage = "Starttid måste anges")]
        public DateTime StartTime { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Id måste vara större än 0")]
        public int CustomerId { get; set; }
    }
}
