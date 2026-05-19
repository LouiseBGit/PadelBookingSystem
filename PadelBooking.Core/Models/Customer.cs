using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PadelBooking.Core.Models
{
    internal class Customer
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
    }
}
