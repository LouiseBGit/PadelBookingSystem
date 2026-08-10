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
        private readonly ICustomerRepository _customerRepository;

        public BookingService(IBookingRepository repository, ICustomerRepository customerRepository)
        {
            _repository = repository;
            _customerRepository = customerRepository;
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
            var customer = await _customerRepository.GetCustomerByIdAsync(dto.CustomerId);

            if (customer == null)
            {
                return null;
            }
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
                CustomerName = customer.FirstName + " " + customer.LastName
            };
        }
        public async Task<BookingDto?> UpdateBookingAsync(int id, UpdateBookingDto dto)
        {
            var customer = await _customerRepository.GetCustomerByIdAsync(dto.CustomerId);

            if (customer == null)
            {
                return null;
            }

            var booking = new Booking
            {
                Id = id,
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
            //ignorera den bokning som uppdateras, vid kontroll av dubbelbokning
            var doubleBooking = await _repository.BookingExistsAsync(
                booking.CourtNumber,
                booking.StartTime,
                booking.Id);


            if (doubleBooking)
            {
                return null;
            }

            await _repository.UpdateAsync(booking);

            return new BookingDto
            {
                Id = booking.Id,
                CourtNumber = booking.CourtNumber,
                StartTime = booking.StartTime,
                CustomerName = customer.FirstName + " " + customer.LastName
            };
        }
        public async Task<bool> DeleteBookingAsync(int id)
        {
            await _repository.DeleteAsync(id);
            return true;
        }

        public async Task<List<BookingDto>> GetBookingsByDatesAsync(DateTime date, int? courtNumber)
        {
            //skapar en variabel som ska innehålla en lista av bokningar
            List<Booking> bookings;
            //om nummer på banan skickas med
            if (courtNumber.HasValue)
            {
                //hämtar bokningar för datum och valt bannummer
                bookings = await _repository.GetBookingsByDateAndCourtAsync(date, courtNumber.Value);
            }
            else
            {
                //om ingen bana valts hämtas alla banornas bokningar
                bookings = await _repository.GetBookingsByDateAsync(date);
            }
            //gör om Booking från databas till BookingDto för Api-svar
            return bookings.Select(b => new BookingDto
            {
                Id = b.Id,
                CourtNumber = b.CourtNumber,
                StartTime = b.StartTime,
                CustomerName = b.Customer.FirstName + " " + b.Customer.LastName
            }).ToList();
        }
        //visar lediga tider att kunna boka
        public async Task<List<int>> GetAvailableTimesAsync(DateTime date, int courtNumber)
        {
            //hämta alla bokningar för valt datum och vald bana
            var bookings = await _repository.GetBookingsByDateAndCourtAsync(date, courtNumber);
            

            //lista för vilka timmar som är lediga
            var availableHours = new List<int>();

            //går igenom alla timmar som kan bokas
            for (int hour = 7; hour < 22; hour++)
            {
                //kollar om det finns en bokning på denna timman
                var isBooked = bookings
                    .Any(b => b.StartTime.Hour == hour);

                //om inte tiden är bokad läggs den till som ledig
                if (!isBooked)
                {
                    availableHours.Add(hour);
                }
            }
            return availableHours;
        }

        /// <summary>
        /// hämtar lediga tider för en vald bana mellan två datum
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <param name="courtNumber"></param>
        /// <returns>Lista med datum och lediga timmar för varje datum</returns>
        public async Task<List<AvailableTimesDto>> GetAvailableTimesBetweenDatesAsync(DateTime startDate, DateTime endDate, int courtNumber)
        {
            //skapar en tom lista där svaret ska sparas
            var result = new List<AvailableTimesDto>();

            //går igenom alla datum från startdatum till slutdatum
            for (DateTime date = startDate; date <= endDate; date = date.AddDays(1))
            {
                //hämtar lediga timmar för valt datum och bana
                var availableHours = await GetAvailableTimesAsync(date, courtNumber);

                //hämtar dto-objekt som innehåller datumet och de lediga timmarna
                var availableTimes = new AvailableTimesDto
                {
                    Date = date,
                    AvailableHours = availableHours
                };

                //lägger till objektet i resultatlistan
                result.Add(availableTimes);
            }

            //returnerar alla datum med deras lediga timmar
            return result;
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
