using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PadelBooking.Core.DTOs
{
    public class AvailableTimesDto
    {
        public DateTime Date { get; set; }
        public List<int> AvailableHours { get; set; } = new List<int>();
    }
}
