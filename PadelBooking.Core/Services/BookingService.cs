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
            //regel om vilken bana
            if (booking.CourtNumber < 1 || booking.CourtNumber > 3)
            {
                return false;
            }
            //regel om dubbelbokning (samma tid, samma bana)
            var bookings = await _repository.GetAllBookingsAsync();
            //bool doubleBooking = booking.Any(b => b.CourtNumber == b.)

            //spara
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
            //regel om vilken bana
            if (booking.CourtNumber < 1 || booking.CourtNumber > 3)
            {
                return false;
            }
            //regel om dubbelbokning (samma tid, samma bana)
            var bookings = await _repository.GetAllBookingsAsync();
            //bool doubleBooking = booking.Any(b => b.CourtNumber == b.)

            await _repository.UpdateAsync(booking);
            return true;
        }
        public async Task<bool> DeleteBookingAsync(int id)
        {
            await _repository.DeleteAsync(id);
            return true;
        }

        
    } 
}
