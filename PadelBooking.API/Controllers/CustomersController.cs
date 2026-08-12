using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing.Constraints;
using PadelBooking.Core.DTOs;
using PadelBooking.Core.Interfaces;
using PadelBooking.Core.Models;
using PadelBooking.Core.Services;

namespace PadelBooking.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomersController : ControllerBase
    {
        private readonly ICustomerService _customerService; 

        public CustomersController(ICustomerService customerService)
        {
            _customerService = customerService;
        }

        [HttpGet]
        public async Task<ActionResult<List<CustomerDto>>> GetAll()
        {
            var customers = await _customerService.GetAllCustomerAsync();

            return Ok(customers);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<CustomerDto>> GetAllCustomerById(int id)
        {
            var customer = await _customerService.GetCustomerByIdAsync(id);

            if(customer == null)
            {
                return NotFound();
            }

            return Ok(customer); 
        }

        /// <summary>
        /// skapar en ny kund
        /// validering sker i service
        /// </summary>
        /// <param name="dto">uppgifter för kunden som ska skapas </param>
        /// <returns>201 Created med skapad kund eller 400 Bad Request om kunden inte kan skapas</returns>
        [HttpPost]
        public async Task<ActionResult> CreateCustomer(CreateCustomerDto dto)
        {
            //service hanterar reglerna och returnerar dto om skapandet lyckas
            var result = await _customerService.CreateCustomerAsync(dto);

            //om någon regel bryts returnerar service null
            if (result == null)
            {
                return BadRequest("En kund med samma epost finns redan...");
            }


            //Returnerar 201 Created eftersom en ny kund skapats.
            //GetAllCustomerById är metoden som kan användas för att hämta den skapade kunden igen.
            //Id skickas med så rätt kund kan hittas.
            //Result är kunden som skickas tillbaka.
            return CreatedAtAction(
                nameof(GetAllCustomerById),
                new { id = result.Id },
                result);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteCustomer(int id)
        {
            
            //service kollar att kunden finns och försöker ta bort den
            var result = await _customerService.DeleteCustomerAsync(id);

            //false betyder att kunden inte fanns
            if (!result)
            {
                return NotFound("Kunden hittades inte...");
            }

            //kunden hittades och togs bort
            return Ok("Kund borttagen!");
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateCustomer(int id, UpdateCustomerDto dto)
        {
          
            var result = await _customerService.UpdateCustomerAsync(id, dto);

            if (result == null)
            {
                return NotFound("Kunde inte uppdatera kunden...");
            }

            return Ok(result);
        }

    }
}
