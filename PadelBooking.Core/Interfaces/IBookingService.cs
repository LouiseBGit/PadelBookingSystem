using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PadelBooking.Core.Models;
using PadelBooking.Core.DTOs;

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
        Task<List<BookingDto>> GetAllBookingsAsync();
        /// <summary>
        /// hämtar en specifik bokning, asynkront, baserat på ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns>bokning om den finns, annars null</returns>
        Task<BookingDto?> GetBookingByIdAsync(int id);
        /// <summary>
        /// skapar en ny bokning
        /// </summary>
        /// <param name="dto"></param>
        /// <returns>en BookingDto om lyckas, annars null</returns>
        Task<BookingDto?> CreateBookingAsync(CreateBookingDto dto);
        /// <summary>
        /// Uppdaterar en bokning med specifikt id
        /// </summary>
        /// <param name="dto"></param>
        /// <param name="id"></param>
        /// <returns>BookingDto</returns>
        Task<BookingDto?> UpdateBookingAsync(int id, UpdateBookingDto dto);
        /// <summary>
        /// tar bort en specifik bokning baserat på ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns>returnerar true om borttagningen lyckades</returns>
        Task<bool> DeleteBookingAsync(int id);

        Task<List<BookingDto>> GetBookingsByDatesAsync(DateTime date, int? courtNumber);

        Task<List<int>> GetAvailableTimesAsync(DateTime date, int courtNumber);
        /// <summary>
        /// Bokning mellan specifikt start- och slutdatum
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        Task<BookingSummaryDto> GetBookingsBetweenDatesAsync(DateTime startDate, DateTime endDate);

        Task<List<BookingDto>> GetBookingsByDateAndCourtAsync(DateTime date, int courtNumber);

        Task<List<AvailableTimesDto>> GetAvailableTimesBetweenDatesAsync(DateTime startDate, DateTime endDate, int? courtNumber);
    }
}
