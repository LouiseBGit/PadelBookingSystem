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
            //hämtar bokningen som ska uppdateras 
            var booking = await _repository.GetBookingByIdAsync(id);
            
            //om bokning inte finns kan den inte uppdateras
            if (booking == null)
            {
                return null;
            }

            //kontrollera att kunden som skickas med finns
            var customer = await _customerRepository.GetCustomerByIdAsync(dto.CustomerId);

            if (customer == null)
            {
                return null;
            }

            //var booking = new Booking
            //{
            //    Id = id,
            //    CourtNumber = dto.CourtNumber,
            //    StartTime = dto.StartTime,
            //    CustomerId = dto.CustomerId
            //};

            //regel om tid
            if (dto.StartTime.Hour < 7 || dto.StartTime.Hour >= 22)
            {
                return null;
            }

            //regel om hel timma
            if (dto.StartTime.Minute != 0)
            {
                return null;
            }
            //regel om vilken bana
            if (dto.CourtNumber < 1 || dto.CourtNumber > 3)
            {
                return null;
            }

            //ignorera den bokning som uppdateras, vid kontroll av dubbelbokning
            var doubleBooking = await _repository.BookingExistsAsync(
                dto.CourtNumber,
                dto.StartTime,
                id);


            if (doubleBooking)
            {
                return null;
            }

            //lägger in de nya värdena i den befintliga bokningen
            booking.CourtNumber = dto.CourtNumber;
            booking.StartTime = dto.StartTime;
            booking.CustomerId = dto.CustomerId;

            //sparar den uppdaterade bokningen
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
            //hämta bokningen för att kolla att den finns
            var booking = await _repository.GetBookingByIdAsync(id);
            
            //om bokning inte finns kan den inte tas bort
            if (booking == null)
            {
                return false;
            }

            //tar bort bokning om den finns
            await _repository.DeleteAsync(id);

            //returnera true när borttagning lyckades
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
        /// hämtar lediga tider mellan två datum
        /// om en bana valts visas tider för den
        /// om ingen bana valts visas tider för alla tre
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <param name="courtNumber"></param>
        /// <returns>Lista med datum, bana och lediga timmar </returns>
        public async Task<List<AvailableTimesDto>> GetAvailableTimesBetweenDatesAsync(DateTime startDate, DateTime endDate, int? courtNumber)
        {
            //skapar en tom lista där svaret ska sparas
            var result = new List<AvailableTimesDto>();

            //går igenom alla datum från startdatum till slutdatum
            for (DateTime date = startDate; date <= endDate; date = date.AddDays(1))
            {
                if (courtNumber.HasValue)
                {
                    //hämtar lediga timmar för aktuellt datum och den valda banan
                    var availableHours = await GetAvailableTimesAsync(date, courtNumber.Value);

                    //skapar ett DTO-objekt med datum, bana och lediga timmar
                    var availableTimes = new AvailableTimesDto
                    {
                        Date = date,
                        CourtNumber = courtNumber.Value,
                        AvailableHours = availableHours
                    };

                    //lägger till resultatet för den här dagen i listan
                    result.Add(availableTimes);
                }
                else
                {
                    //går igenom alla tre banor
                    for (int court = 1; court <= 3; court++)
                    {
                        //hämtar lediga timmar för aktuellt datum och aktuell bana
                        var availableHours = await GetAvailableTimesAsync(date, court);

                        //skapar ett DTO-objekt för aktuell dag och bana
                        var availableTimes = new AvailableTimesDto
                        {
                            Date = date,
                            CourtNumber = court,
                            AvailableHours = availableHours
                        };

                        //lägger till resultatet för aktuell dag och bana i listan
                        result.Add(availableTimes);
                    }
                }


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
        public async Task<BookingSummaryDto> GetBookingsBetweenDatesAsync(DateTime startDate, DateTime endDate)
        {
            var bookings = await _repository.GetBookingsBetweenDatesAsync(startDate, endDate);

            var bookingDtos = bookings.Select(b => new BookingDto
            {
                Id = b.Id,
                CourtNumber = b.CourtNumber,
                StartTime = b.StartTime,
                CustomerName = b.Customer.FirstName + " " + b.Customer.LastName
            }).ToList();

            //skapar en sammanfattning av bokningarna
            var summary = new BookingSummaryDto
            {
                TotalBookings = bookings.Count,

                //räknar hur många bokningar som finns på varje bana
                Court1Total = bookings.Count(b => b.CourtNumber == 1),
                Court2Total = bookings.Count(b => b.CourtNumber == 2),
                Court3Total = bookings.Count(b => b.CourtNumber == 3),
                //lägger till själva bokningarna i sammanfattningen
                Bookings = bookingDtos
            };

            //returnerar sammanfattningen
            return summary;
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
