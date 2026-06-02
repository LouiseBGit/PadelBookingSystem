using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PadelBooking.Core.Interfaces;
using PadelBooking.Core.Models;
using PadelBooking.Core.DTOs;

namespace PadelBooking.API.Controllers
{
    /// <summary>
    /// controller som tar hand om HTTP-anrop för bokningar 
    /// tar emot requests och skickar vidare till service 
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class BookingsController : ControllerBase
    {
        //dependency injection
        private readonly IBookingService _bookingService;

        /// <summary>
        /// konstruktor
        /// </summary>
        /// <param name="bookingService"></param>
        public BookingsController(IBookingService bookingService)
        {
            _bookingService = bookingService;
        }


        //ActionResult = controllern returnerar HTTP-svar
        /// <summary>
        /// hämtar alla bokningar
        /// </summary>
        /// <returns>lista med bokningar - DTO-objekt</returns>
        [HttpGet] 
        public async Task<ActionResult<List<BookingDto>>> GetAll()
        {
            //hämtar alla bokningar från service
            var bookings = await _bookingService.GetAllBookingsAsync();

            //mapping -entity till DTO
            var bookingDtos = bookings.Select(b => new BookingDto
            {
                Id = b.Id,
                CourtNumber = b.CourtNumber,
                StartTime = b.StartTime
            }).ToList();

            //returnerar JSON-data
            return Ok(bookingDtos);
        }
        /// <summary>
        /// hämtar specifik bokning baserat på ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns>returnerar bokning eller 404 om den inte finns</returns>
        [HttpGet("{id}")] 
        public async Task<ActionResult<BookingDto>> GetBookingById(int id)
        {
            var booking = await _bookingService.GetBookingByIdAsync(id);

            if(booking == null)
            {
                return NotFound();
            }

            //mapping -entity till DTO
            var bookingDto = new BookingDto
            {
                Id = booking.Id,
                CourtNumber = booking.CourtNumber,
                StartTime = booking.StartTime
            };

            return Ok(bookingDto);
        }
        /// <summary>
        /// skapar ny bokning 
        /// validering sker i service
        /// </summary>
        /// <param name="booking"></param>
        [HttpPost] 
        public async Task<ActionResult> CreateBooking(CreateBookingDto dto)
        {
            //mapping - DTO till Entity
            var booking = new Booking
            {
                CourtNumber = dto.CourtNumber,
                StartTime = dto.StartTime
            };

            //service hanterar reglerna
            var result = await _bookingService.CreateBookingAsync(booking);

            if (!result)
            {
                return BadRequest("Bokning bryter mot reglerna");
            }

            //200 OK
            return Ok("Bokning skapad");
        }

        /// <summary>
        /// Tar bort en bokning baserat på ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBooking(int id)
        {
            //koll om bokning finns 
            var booking = await _bookingService.GetBookingByIdAsync(id);

            if (booking == null)
            {
                return NotFound("Bokningen hittades inte");
            }

            var result = await _bookingService.DeleteBookingAsync(id);

            if (!result)
            {
                return BadRequest("Kunde inte ta bort bokningen");
            }

            return Ok();

        }
        /// <summary>
        /// Uppdaterar en befintlig bokning 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="booking"></param>
        /// <returns>uppdaterad bokning</returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateBooking(int id, UpdateBookingDto dto)
        {
            //mapping - DTO till entity 
            var booking = new Booking
            {
                Id = id,
                CourtNumber = dto.CourtNumber,
                StartTime = dto.StartTime
            };

            //service kontrollerar regler innan uppdatering görs
            var result = await _bookingService.UpdateBookingAsync(booking);

            if (!result)
            {
                return BadRequest("Uppdateringen misslyckades pga bruten regel"); 
            }
            //200 OK och uppdaterar bokning
            return Ok(booking);
        }
    }
}
