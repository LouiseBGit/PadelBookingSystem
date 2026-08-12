using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PadelBooking.Core.DTOs
{
    public class BookingSummaryDto
    {
        //totalt antal bokningar
        public int TotalBookings { get; set; }

        //antal bokningar på bana 1
        public int Court1Total { get; set; }

        //antal bokningar på bana 2
        public int Court2Total { get; set; }

        //antal bokningar på bana 3
        public int Court3Total { get; set; }

        //alla bokningar som ingår i sammanfattningen
        public List<BookingDto> Bookings { get; set; } = new List<BookingDto>();
    }
}
