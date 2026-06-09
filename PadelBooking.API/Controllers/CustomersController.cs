using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing.Constraints;
using PadelBooking.Core.DTOs;
using PadelBooking.Core.Interfaces;
using PadelBooking.Core.Models;

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
            var customer = await _customerService.GetAllCustomerAsync();

            var customerDtos = customer.Select(c => new CustomerDto
            {
                Id = c.Id,
                FirstName = c.FirstName,
                LastName = c.LastName,
                Email = c.Email,
                PhoneNumber = c.PhoneNumber
            }).ToList(); 

            return Ok(customerDtos);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<CustomerDto>> GetAllCustomerById(int id)
        {
            var customer = await _customerService.GetAllCustomerById(id);

            if(customer == null)
            {
                return NotFound();
            }

            var customerDto = new CustomerDto
            {
                Id = customer.Id,
                FirstName = customer.FirstName,
                LastName = customer.LastName,
                Email = customer.Email,
                PhoneNumber = customer.PhoneNumber
            };

            return Ok(customerDto); 
        }

        [HttpPost]
        public async Task<ActionResult> CreateCustomer(CreateCustomerDto dto)
        {
            var customer = new Customer
            {
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Email = dto.Email,
                PhoneNumber = dto.PhoneNumber
            };

            var result = await _customerService.CreateCustomerAsync(customer);

            if(!result)
            {
                return BadRequest("Kunde inte skapa kunden..");
            }

            return Ok("Kund skapad!");
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteCustomer(int id)
        {
            var customer = await _customerService.GetAllCustomerById(id);

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
            //hämtar kund från databasen
            var customer = await _customerService.GetCustomerByIdAsync(id);

            if(customer == null)
            {
                return NotFound("Kunden hittades inte...");
            }

            //mapping DTO till Entity
            customer.FirstName = dto.FirstName;
            customer.LastName = dto.LastName;
            customer.Email = dto.Email;
            customer.PhoneNumber = dto.PhoneNumber;

            var result = await _customerService.UpdateCustomerAsync(customer);

            if(!result)
            {
                return BadRequest("Kunde inte uppdatera kunden...");
            }

            return Ok("Kund uppdaterad.");
        }

    }
}
