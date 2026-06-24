using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PadelBooking.Core.DTOs
{
    /// <summary>
    /// DTO för att uppdatera en bokning
    /// </summary>
    public class UpdateBookingDto
    {
        //det nya numret på banan
        [Range(1, 3, ErrorMessage = "Numret på banan måste vara mellan 1-3")]
        public int CourtNumber { get; set; }
        //den nya starttiden för bokningen
        [Required(ErrorMessage = "Starttid måste anges")]
        public DateTime StartTime { get; set; }
        public int CustomerId { get; set; }
    }
}
