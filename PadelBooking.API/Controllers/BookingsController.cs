using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PadelBooking.Core.Interfaces;
using PadelBooking.Core.Models;
using PadelBooking.Core.DTOs;
using Microsoft.Identity.Client;

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

            return Ok(bookings);
            ////mapping -entity till DTO
            //var bookingDtos = bookings.Select(b => new BookingDto
            //{
            //    Id = b.Id,
            //    CourtNumber = b.CourtNumber,
            //    StartTime = b.StartTime
            //}).ToList();

            ////returnerar JSON-data
            //return Ok(bookingDtos);
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

            return Ok(booking);
        }
        /// <summary>
        /// skapar ny bokning
        /// validering sker i service
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPost] 
        public async Task<ActionResult> CreateBooking(CreateBookingDto dto)
        {
            

            //service hanterar reglerna och returnerar BookingDto om bokning lyckas
            var result = await _bookingService.CreateBookingAsync(dto);

            //om någon regel bryts returnerar service null
            if (result == null)
            {
                return BadRequest("Bokning bryter mot reglerna");
            }


            //Returnerar 201 Created eftersom en ny bokning skapats.
            //GetBookingById är metoden som kan användas för att hämta bokningen igen.
            //Id skickas med så rätt bokning kan hittas.
            //Result är bokningen som skickas tillbaka.
            return CreatedAtAction(
                nameof(GetBookingById),
                new { id = result.Id },
                result);
           
        }

        /// <summary>
        /// Tar bort en bokning baserat på ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteBooking(int id)
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

            return Ok("Bokning borttagen");

        }
        /// <summary>
        /// Uppdaterar en befintlig bokning 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="booking"></param>
        /// <returns>uppdaterad bokning</returns>
        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateBooking(int id, UpdateBookingDto dto)
        {
            
            //service kontrollerar regler innan uppdatering görs
            var result = await _bookingService.UpdateBookingAsync(id, dto);

            if (result == null)
            {
                return BadRequest("Misslyckad uppdatering...");
            }
            
            return Ok(result);
        }
        /// <summary>
        /// Hämtar alla bokningar för ett specifikt datum
        /// </summary>
        /// <param name="date"></param>
        /// <returns>Lista med alla bokningar för vald dag</returns>
        [HttpGet("date")]
        public async Task<ActionResult<List<BookingDto>>> GetBookingsByDate(DateTime date, int? courtNumber)
        {
            var bookings = await _bookingService.GetBookingsByDatesAsync(date, courtNumber);

            
            if (!bookings.Any())
            {
                return NotFound("Inga bokningar...");
            }
            return Ok(bookings);
        }

        /// <summary>
        /// hämtar lediga tider för specifik dag
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        [HttpGet("available")]
        public async Task<ActionResult<List<int>>> GetAvailableTimes([FromQuery] DateTime date, [FromQuery] int courtNumber)
        {
            var result = await _bookingService.GetAvailableTimesAsync(date, courtNumber);
            return Ok(result);
        }

        /// <summary>
        /// hämtar lediga tider för vald bana mellan två datum
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <param name="courtNumber"></param>
        [HttpGet("available/between")]
        public async Task<ActionResult<List<AvailableTimesDto>>> GetAvailableTimesBetweenDates([FromQuery] DateTime startDate, [FromQuery] DateTime endDate, [FromQuery] int? courtNumber)
        {
            //skickar datumen och banan till service
            var result = await _bookingService.GetAvailableTimesBetweenDatesAsync(startDate, endDate, courtNumber);

            //returnerar restultatet 
            return Ok(result);
        }

        /// <summary>
        /// Alla bokningar mellan specifika datum
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns>Lista med bokningar mellan specifika datum</returns>
        [HttpGet("between")]
        public async Task<ActionResult<BookingSummaryDto>> GetBookingsBetweenDates(DateTime startDate, DateTime endDate)
        {
            var bookings = await _bookingService
                .GetBookingsBetweenDatesAsync(startDate, endDate);

            if (bookings.TotalBookings == 0)
            {
                return NotFound("Inga bokningar...");
            }
            return Ok(bookings);
        }

        [HttpGet("date/court")]
        public async Task<ActionResult<List<BookingDto>>> GetByDateAndCourt([FromQuery] DateTime date, [FromQuery] int courtNumber)
        {
            var bookings = await _bookingService.GetBookingsByDateAndCourtAsync(date, courtNumber);
            
            if (!bookings.Any())
            {
                return NotFound("Inga bokningar...");
            }

            return Ok(bookings);
        }
    }
}
