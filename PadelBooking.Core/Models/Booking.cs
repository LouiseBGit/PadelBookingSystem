using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PadelBooking.Core.Models
{
    internal class Booking
    {
        public int Id { get; set; }
        public DateTime StartTime { get; set; }
        public int CourtNumber { get; set; }
        public int CustomerId { get; set; }
        public Customer Customer { get; set; } = null!;
    }
}
