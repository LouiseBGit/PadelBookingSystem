using System;
using System.Collections.Generic;
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
        public int CourtNumber { get; set; }
        //Starttid för bokningen
        public DateTime StartTime { get; set; }
    }
}
