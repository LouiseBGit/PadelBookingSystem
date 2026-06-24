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
        /// <summary>
        /// bokningar för en viss banan och tid
        /// </summary>
        /// <param name="courtNumber"></param>
        /// <param name="startTime"></param>
        /// <returns></returns>
        Task<bool> BookingExistsAsync(int courtNumber, DateTime startTime, int? excludeId = null);
        /// <summary>
        /// bokningar för en viss dag
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        Task<List<Booking>> GetBookingsByDateAsync(DateTime date);
        /// <summary>
        /// bokningar mellan två specifika datum
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        Task<List<Booking>> GetBookingsBetweenDatesAsync(DateTime startDate, DateTime endDate);

        Task<List<Booking>> GetBookingsByDateAndCourtAsync(DateTime date, int courtNumber);
    }
}
