using System;
using System.Collections.Generic;
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
        public int CourtNumber { get; set; }
        //den nya starttiden för bokningen
        public DateTime StartTime { get; set; }
    }
}
