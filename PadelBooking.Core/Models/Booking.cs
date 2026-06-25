using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PadelBooking.Core.Models
{
    public class Booking
    {
        public int Id { get; set; }
        public DateTime StartTime { get; set; }
        public int CourtNumber { get; set; }
        //foreign key -pekar på kunden
        public int CustomerId { get; set; }
        //navigation property
        public Customer Customer { get; set; } = null!;
    }
}
