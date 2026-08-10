using PadelBooking.Core.DTOs;
using PadelBooking.Core.Interfaces;
using PadelBooking.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Schema;

namespace PadelBooking.Core.Services
{
    /// <summary>
    /// klass för regler
    /// </summary>
    public class CustomerService : ICustomerService
    {
        private readonly ICustomerRepository _repository;
        /// <summary>
        /// konstruktor -skapar en ny instans av CustomerService
        /// </summary>
        /// <param name="repository"></param>
        public CustomerService(ICustomerRepository repository)
        {
            _repository = repository;
        }
        /// <summary>
        /// skapar en ny kund
        /// </summary>
        /// <param name="dto"></param>
        /// <returns>en ny customerDto om det lyckades</returns>
        public async Task<CustomerDto?> CreateCustomerAsync(CreateCustomerDto dto)
        {
            //kolla om kund redan finns
            var exists = await _repository.GetCustomerByEmailAsync(dto.Email);

            //stop om email redan finns, så inte samma kund kan läggas till flera gånger
            if (exists != null)
            {
                return null;
            }

            var customer = new Customer
            {
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Email = dto.Email,
                PhoneNumber = dto.PhoneNumber
            };

            //..annars spara
            await _repository.AddAsync(customer);

            return new CustomerDto
            {
                Id = customer.Id,
                FirstName = customer.FirstName,
                LastName = customer.LastName,
                Email = customer.Email,
                PhoneNumber = customer.PhoneNumber
            };
        }
        /// <summary>
        /// raderar kund baserat på id
        /// </summary>
        /// <param name="id"></param>
        /// <returns>true om kunden togs bort, annars false</returns>
        public async Task<bool> DeleteCustomerAsync(int id)
        {
            var customer = await _repository.GetCustomerByIdAsync(id);

            if(customer == null)
            {
                return false;
            }

            await _repository.DeleteAsync(id);

            return true; 
        }

        /// <summary>
        /// Hämtar alla kunder
        /// </summary>
        /// <returns>returnerar en lista med alla kunder, om kunder existerar</returns>
        public async Task<List<CustomerDto>> GetAllCustomerAsync()
        {
            var customers = await _repository.GetAllCustomersAsync();

            return customers.Select(c => new CustomerDto
            {
                Id = c.Id,
                FirstName = c.FirstName,
                LastName = c.LastName,
                Email = c.Email,
                PhoneNumber = c.PhoneNumber
            }).ToList();
        }
        /// <summary>
        /// hämtar en kund baserat på id
        /// </summary>
        /// <param name="id"></param>
        /// <returns>kunden om denne finns, annars null</returns>
        public async Task<CustomerDto?> GetCustomerByIdAsync(int id)
        {
            var customer = await _repository.GetCustomerByIdAsync(id);

            if (customer == null)
            {
                return null;
            }

            return new CustomerDto
            {
                Id = customer.Id,
                FirstName = customer.FirstName,
                LastName = customer.LastName,
                Email = customer.Email,
                PhoneNumber = customer.PhoneNumber
            };
            
        }
        /// <summary>
        /// uppdaterar en befintlig kund
        /// </summary>
        /// <param name="customer"></param>
        /// <returns>true om uppdateringen lyckades</returns>
        public async Task<CustomerDto?> UpdateCustomerAsync(int id, UpdateCustomerDto dto)
        {
            var customer = await _repository.GetCustomerByIdAsync(id);

            if (customer == null)
            {
                return null;
            }

            //uppdaterar kunden
            customer.FirstName = dto.FirstName;
            customer.LastName = dto.LastName;
            customer.Email = dto.Email;
            customer.PhoneNumber = dto.PhoneNumber;

            //spara ändringen
            await _repository.UpdateAsync(customer);

            //returnera uppdaterad kund
            return new CustomerDto
            {
                Id = customer.Id,
                FirstName = customer.FirstName,
                LastName = customer.LastName,
                Email = customer.Email,
                PhoneNumber = customer.PhoneNumber
            };
        }
    }
}
