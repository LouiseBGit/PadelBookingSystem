using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PadelBooking.Core.Models;

namespace PadelBooking.Core.Interfaces
{
    //interface -vad som ska göras, men inte hur det görs (som ett kontrakt)
    //metoderna som måste finnas för att hantera bokning
    //Task = asynkron metod --> annat kan göras undertiden
    public interface IBookingRepository
    {
        //hämta alla bokningar från databasen
        Task<List<Booking>> GetAllBookingsAsync();
        //hämta en bokning med ID
        //? = returnerar null om bokning på det ID inte finns
        Task<Booking?> GetBookingByIdAsync(int id);
        //lägg till en ny bokning i databasen
        Task AddAsync(Booking booking);
        //uppdaterar bokning
        Task UpdateAsync(Booking booking);
        //ta bort bokning på ID
        Task DeleteAsync(int id);

    }
}
