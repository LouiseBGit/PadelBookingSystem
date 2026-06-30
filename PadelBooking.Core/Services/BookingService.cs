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
    /// klass för regler och logik för bokningar
    /// hämtar data från repository och kollar att regler följs
    /// </summary>
    public class BookingService : IBookingService
    {
        //dependency injection av repository
        private readonly IBookingRepository _repository;

        public BookingService(IBookingRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<BookingDto>> GetAllBookingsAsync()
        {
            //hämtar alla bokningar från repository
            var bookings = await _repository.GetAllBookingsAsync();
            //mappar entities till dto innan de skickas till api
            //använder navigation propertiy för att få hela kundens namn
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
                StartTime = booking.StartTime,
                CustomerName = booking.Customer.FirstName + " " + booking.Customer.LastName
            };
        }
        /// <summary>
        /// skapar en ny bokning
        /// </summary>
        /// <param name="dto"></param>
        /// <returns>BookingDto om reglerna följs</returns>
        public async Task<BookingDto?> CreateBookingAsync(CreateBookingDto dto)
        {
            //mapping - DTO till Entity
            var booking = new Booking
            {
                CourtNumber = dto.CourtNumber,
                StartTime = dto.StartTime,
                CustomerId = dto.CustomerId
            };

            //regel om tid
            if (booking.StartTime.Hour < 7 || booking.StartTime.Hour >= 22)
            {
                return null;
            }
            //regel om hel timma
            if (booking.StartTime.Minute != 0)
            {
                return null;
            }
            //regel om vilken bana
            if (booking.CourtNumber < 1 || booking.CourtNumber > 3)
            {
                return null;
            }
            //kontrollerar eventuell dubbelbokning
            var doubleBooking = await _repository.BookingExistsAsync(
                booking.CourtNumber,
                booking.StartTime);

            if (doubleBooking)
            {
                return null;
            }

            //spara om alla regler är ok
            await _repository.AddAsync(booking);
            return new BookingDto
            {
                Id = booking.Id,
                CourtNumber = booking.CourtNumber,
                StartTime = booking.StartTime,
                CustomerName = booking.Customer?.FirstName + " " + booking.Customer?.LastName
            };
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
            //ignorera den bokning som uppdateras, vid kontroll av dubbelbokning
            var doubleBooking = await _repository.BookingExistsAsync(
                booking.CourtNumber,
                booking.StartTime,
                booking.Id);


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
                StartTime = b.StartTime,
                CustomerName = b.Customer.FirstName + " " + b.Customer.LastName
            }).ToList();
        }
        //visar lediga tider att kunna boka
        public async Task<List<int>> GetAvailableTimesAsync(DateTime date)
        {
            //hämta alla bokningar för valt datum
            var bookings = await _repository.GetBookingsByDateAsync(date);
            //tre banor
            var courtCount = 3;

            ////samlar alla bokade timmar för det valda datumet i en lista
            //var bookedHours = bookings
            //    .Select(b => b.StartTime.Hour)
            //    .ToList();

            //lista för vilka timmar som är lediga
            var availableHours = new List<int>();
            //går igenom alla timmar som kan bokas
            for (int hour = 7; hour < 22; hour++)
            {
                //räknar hur många bokningar som finns i denna timman
                var bookedCounts = bookings
                    .Count(b => b.StartTime.Hour == hour);
                //om inte alla är bokade finns minst en ledig bana
                if (bookedCounts < courtCount)
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
                StartTime = b.StartTime,
                CustomerName = b.Customer.FirstName + " " + b.Customer.LastName
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
                    StartTime = b.StartTime,
                    CustomerName = b.Customer.FirstName + " " + b.Customer.LastName
                })
                .ToList();
        }
        
    } 
}
