using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PadelBooking.Core.Models;

namespace PadelBooking.Core.Interfaces
{
    /// <summary>
    /// Interface för bokningslogik
    /// VAD systemet får göra, inte HUR det görs
    /// </summary>
    public interface IBookingService
    {
        /// <summary>
        /// hämtar alla bokningar asynkront
        /// </summary>
        /// <returns>lista med bokningar</returns>
        Task<List<Booking>> GetAllBookingsAsync();
        /// <summary>
        /// hämtar en specifik bokning, asynkront, baserat på ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns>bokning om den finns, annars null</returns>
        Task<Booking?> GetBookingByIdAsync(int id);
        /// <summary>
        /// skapar en ny bokning
        /// </summary>
        /// <param name="booking"></param>
        /// <returns>returnerar true om bokningen skapades, annars false om regel bröts</returns>
        Task<bool> CreateBookingAsync(Booking booking);
        /// <summary>
        /// uppdaterar en befintlig bokning
        /// </summary>
        /// <param name="booking"></param>
        /// <returns>returnerar true om uppdateringen lyckades, annars false om regel bröts</returns>
        Task<bool> UpdateBookingAsync(Booking booking);
        /// <summary>
        /// tar bort en specifik bokning baserat på ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns>returnerar true om borttagningen lyckades</returns>
        Task<bool> DeleteBookingAsync(int id);


    }
}
