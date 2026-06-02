using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PadelBooking.Core.Interfaces;
using PadelBooking.Core.Models;

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

        //konstruktor
        public BookingsController(IBookingService bookingService)
        {
            _bookingService = bookingService;
        }

        //ActionResult = controllern returnerar HTTP-svar

        [HttpGet] 
        public async Task<ActionResult<List<Booking>>> GetAll()
        {
            var bookings = await _bookingService.GetAllBookingsAsync();

            //returnerar JSON-data
            return Ok(bookings);
        }
        /// <summary>
        /// hämtar specifik bokning baserat på ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns>returnerar bokning eller 404 om den inte finns</returns>
        [HttpGet("{id}")] 
        public async Task<ActionResult<Booking>> GetBookingById(int id)
        {
            var bookings = await _bookingService.GetBookingByIdAsync(id);

            if(bookings == null)
            {
                return NotFound();
            }

            return Ok(bookings);
        }
        /// <summary>
        /// skapar ny bokning 
        /// validering sker i service
        /// </summary>
        /// <param name="booking"></param>
        [HttpPost] 
        public async Task<ActionResult<Booking>> CreateBooking(Booking booking)
        {
            //koll om det är tomt i request-bodyn
            if (booking == null)
            {
                return BadRequest("Tom bokning");
            }
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
            var bookings = await _bookingService.GetBookingByIdAsync(id);

            if (bookings == null)
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
        /// <returns></returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateBooking(int id, Booking booking)
        {
            //koll om request är tom
            if (booking == null)
            {
                return BadRequest("Bokningen är tom");
            }

            //ser till att rätt ID uppdateras
            booking.Id = id;

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
