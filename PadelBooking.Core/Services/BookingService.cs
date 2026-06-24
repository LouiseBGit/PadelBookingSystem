using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;
using PadelBooking.Core.DTOs;
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

        public async Task<List<BookingDto>> GetAllBookingsAsync()
        {
            
            var bookings = await _repository.GetAllBookingsAsync();

            return bookings.Select(b => new BookingDto
            {
                Id = b.Id,
                CourtNumber = b.CourtNumber,
                StartTime = b.StartTime,
                CustomerName = b.Customer.FirstName + " " + b.Customer.LastName
            }).ToList();
            
        }

        public async Task<BookingDto?> GetBookingByIdAsync(int id)
        {
            var booking = await _repository.GetBookingByIdAsync(id);

            if (booking == null)
            {
                return null;
            }

            return new BookingDto
            {
                Id = booking.Id,
                CourtNumber = booking.CourtNumber,
                StartTime = booking.StartTime
            };
        }

        public async Task<bool> CreateBookingAsync(Booking booking)
        {
            //regel om tid
            if (booking.StartTime.Hour < 7 || booking.StartTime.Hour >= 22)
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
            if (booking.StartTime.Hour < 7 || booking.StartTime.Hour >= 22)
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

            var doubleBooking = await _repository.BookingExistsAsync(
                booking.CourtNumber,
                booking.StartTime,
                booking.Id);

            ////regel om dubbelbokning (ignorerar den bokningen som ska ändras)
            //var bookings = await _repository.GetAllBookingsAsync();
            ////bool doubleBooking = bookings.Any(b =>
            ////b.Id == booking.Id &&
            ////b.CourtNumber == booking.CourtNumber &&
            ////b.StartTime == booking.StartTime
            ////);
            //bool doubleBooking = bookings.Any(b =>
            //b.Id != booking.Id &&
            //b.CourtNumber == booking.CourtNumber &&
            //b.StartTime == booking.StartTime
            //); 

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

        public async Task<List<BookingDto>> GetBookingsByDatesAsync(DateTime date)
        {
            var bookings = await _repository.GetBookingsByDateAsync(date);

            return bookings.Select(b => new BookingDto
            {
                Id = b.Id,
                CourtNumber = b.CourtNumber,
                StartTime = b.StartTime
            }).ToList();
        }
        //visar lediga tider att kunna boka
        public async Task<List<int>> GetAvailableTimesAsync(DateTime date)
        {
            var bookings = await _repository.GetBookingsByDateAsync(date);

            var bookedHours = bookings
                .Select(b => b.StartTime.Hour)
                .ToList();

            var availableHours = new List<int>();

            for (int hour = 7; hour < 22; hour++)
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
        public async Task<List<BookingDto>> GetBookingsBetweenDatesAsync(DateTime startDate, DateTime endDate)
        {
            var bookings = await _repository.GetBookingsBetweenDatesAsync(startDate, endDate);

            return bookings.Select(b => new BookingDto
            {
                Id = b.Id,
                CourtNumber = b.CourtNumber,
                StartTime = b.StartTime
            }).ToList();
        }

        public async Task<List<BookingDto>> GetBookingsByDateAndCourtAsync(DateTime date, int courtNumber)
        {
            var bookings = await _repository.GetBookingsByDateAndCourtAsync(date, courtNumber);

            return bookings
                .Select(b => new BookingDto
                {
                    Id = b.Id,
                    CourtNumber = b.CourtNumber,
                    StartTime = b.StartTime
                })
                .ToList();
        }
        
    } 
}
