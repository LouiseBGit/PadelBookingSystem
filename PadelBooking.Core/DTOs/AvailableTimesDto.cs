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
        //vilken bana det gäller
        public int CourtNumber { get; set; }
        //lista med de lediga timmarna
        public List<int> AvailableHours { get; set; } = new List<int>();
    }
}
