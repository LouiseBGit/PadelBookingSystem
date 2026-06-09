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
        /// <param name="customer"></param>
        /// <returns>true om kunden skapades</returns>
        public async Task<bool> CreateCustomerAsync(Customer customer)
        {
            await _repository.AddAsync(customer);

            return true;            
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
        /// <returns>alla kunder från databasen, som en lista</returns>
        public async Task<List<Customer>> GetAllCustomerAsync()
        {
            return await _repository.GetAllCustomersAsync();
        }
        /// <summary>
        /// hämtar en kund baserat på id
        /// </summary>
        /// <param name="id"></param>
        /// <returns>kunden om denne finns, annars null</returns>
        public async Task<Customer?> GetCustomerByIdAsync(int id)
        {
            return await _repository.GetCustomerByIdAsync(id);
            
        }
        /// <summary>
        /// uppdaterar en befintlig kund
        /// </summary>
        /// <param name="customer"></param>
        /// <returns>true om uppdateringen lyckades</returns>
        public async Task<bool> UpdateCustomerAsync(Customer customer)
        {
            await _repository.UpdateAsync(customer);

            return true;
        }
    }
}
