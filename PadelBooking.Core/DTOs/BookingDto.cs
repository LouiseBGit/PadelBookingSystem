using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PadelBooking.Core.DTOs
{
    /// <summary>
    /// DTO för att visa bokningsinfo i API-svar
    /// </summary>
    public class BookingDto
    {
        //bokningsId
        public int Id { get; set; }
        //vilken bana som bokats
        public int CourtNumber { get; set; }
        //starttiden för bokningen
        public DateTime StartTime { get; set; }
    }
}
