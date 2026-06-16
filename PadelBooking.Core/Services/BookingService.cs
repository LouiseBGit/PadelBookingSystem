using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;
using PadelBooking.Core.Interfaces;
using PadelBooking.Core.Models;

namespace PadelBooking.Core.Services
{
    /// <summary>
    /// klass för regler och logik
    /// </summary>
    public class BookingService : IBookingService
    {
        private readonly IBookingRepository _repository;

        public BookingService(IBookingRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<Booking>> GetAllBookingsAsync()
        {
            return await _repository.GetAllBookingsAsync();
        }

        public async Task<Booking?> GetBookingByIdAsync(int id)
        {
            return await _repository.GetBookingByIdAsync(id);
        }

        public async Task<bool> CreateBookingAsync(Booking booking)
        {
            //regel om tid
            if (booking.StartTime.Hour < 7 || booking.StartTime.Hour > 22)
            {
                return false;
            }
            //regel om hel timma
            if (booking.StartTime.Minute != 0)
            {
                return false;
            }
            //regel om vilken bana
            if (booking.CourtNumber < 1 || booking.CourtNumber > 3)
            {
                return false;
            }
            //regel om dubbelbokning (samma tid, samma bana)
            //var bookings = await _repository.GetAllBookingsAsync();
            //bool doubleBooking = bookings.Any(b =>
            //b.CourtNumber == booking.CourtNumber &&
            //b.StartTime == booking.StartTime
            //);

            var doubleBooking = await _repository.BookingExistsAsync(
                booking.CourtNumber,
                booking.StartTime);

            if (doubleBooking)
            {
                return false;
            }

            //spara om alla regler är ok
            await _repository.AddAsync(booking);
            return true;
        }
        public async Task<bool> UpdateBookingAsync(Booking booking)
        {
            //regel om tid
            if (booking.StartTime.Hour < 7 || booking.StartTime.Hour > 22)
            {
                return false;
            }

            //regel om hel timma
            if (booking.StartTime.Minute != 0)
            {
                return false;
            }
            //regel om vilken bana
            if (booking.CourtNumber < 1 || booking.CourtNumber > 3)
            {
                return false;
            }
            //regel om dubbelbokning (ignorerar den bokningen som ska ändras)
            var bookings = await _repository.GetAllBookingsAsync();
            //bool doubleBooking = bookings.Any(b =>
            //b.Id == booking.Id &&
            //b.CourtNumber == booking.CourtNumber &&
            //b.StartTime == booking.StartTime
            //);
            bool doubleBooking = bookings.Any(b =>
            b.Id != booking.Id &&
            b.CourtNumber == booking.CourtNumber &&
            b.StartTime == booking.StartTime
            ); 

            if (doubleBooking)
            {
                return false;
            }

            await _repository.UpdateAsync(booking);
            return true;
        }
        public async Task<bool> DeleteBookingAsync(int id)
        {
            await _repository.DeleteAsync(id);
            return true;
        }

        public async Task<List<Booking>> GetBookingsByDatesAsync(DateTime date)
        {
            return await _repository.GetBookingsByDateAsync(date);
        }
        //visar lediga tider att kunna boka
        public async Task<List<int>> GetAvailableTimesAsync(DateTime date)
        {
            var bookings = await _repository.GetBookingsByDateAsync(date);

            var bookedHours = bookings
                .Select(b => b.StartTime.Hour)
                .ToList();

            var availableHours = new List<int>();

            for (int hour = 7; hour <= 22; hour++)
            {
                if (!bookedHours.Contains(hour))
                {
                    availableHours.Add(hour);
                }
            }
            return availableHours;
        }

        /// <summary>
        /// ger bokningar mellan två specifika datum
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        public async Task<List<Booking>> GetBookingsBetweenDatesAsync(DateTime startDate, DateTime endDate)
        {
            return await _repository.GetBookingsBetweenDatesAsync(startDate, endDate);
        }
        
    } 
}
