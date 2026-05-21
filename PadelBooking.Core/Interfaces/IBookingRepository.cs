using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PadelBooking.Core.Models;

namespace PadelBooking.Core.Interfaces
{
    /// <summary>
    /// interface -för hantrering av bokningar i databasen
    /// </summary>
    
    //interface -vad som ska göras, men inte hur det görs (som ett kontrakt)
    //metoderna som måste finnas för att hantera bokning
    //Task = asynkron metod --> annat kan göras undertiden
    public interface IBookingRepository
    {
        /// <summary>
        /// hämta alla bokningar från databasen
        /// </summary>
        /// <returns> returnerar en lista med alla bokningar</returns>
        Task<List<Booking>> GetAllBookingsAsync();
        /// <summary>
        /// hämta en bokning med ID, asyncront
        /// </summary>
        /// <param name="id"></param>
        /// <returns> returnerar bokning på specifikt ID, eller null om ingen bokning finns (<-- ?) </returns>
        Task<Booking?> GetBookingByIdAsync(int id);
        /// <summary>
        /// lägg till en ny bokning i databasen, asynkront
        /// </summary>
        /// <param name="booking"> Bokningen som ska läggas till</param>
        Task AddAsync(Booking booking);
        /// <summary>
        /// uppdaterar bokning asynktont 
        /// </summary>
        /// <param name="booking"></param>
        Task UpdateAsync(Booking booking);
        /// <summary>
        /// ta bort bokning på ID asynkront
        /// </summary>
        /// <param name="id"></param>
        Task DeleteAsync(int id);

    }
}
