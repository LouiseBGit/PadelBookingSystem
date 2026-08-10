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
            //Id skickas med så rätt bokning kan hittas.
            //Result är bokningen som skickas tillbaka.
            return CreatedAtAction(
                nameof(GetAllCustomerById),
                new { id = result.Id },
                result);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteCustomer(int id)
        {
            var customer = await _customerService.GetCustomerByIdAsync(id);

            if (customer == null)
            {
                return NotFound("Kunden hittades inte...");
            }

            var result = await _customerService.DeleteCustomerAsync(id);

            if (!result)
            {
                return BadRequest("Kunde inte ta bort kunden...");
            }

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
