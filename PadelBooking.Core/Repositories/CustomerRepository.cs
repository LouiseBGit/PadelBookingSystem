using Microsoft.EntityFrameworkCore;
using PadelBooking.Core.Data;
using PadelBooking.Core.Interfaces;
using PadelBooking.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PadelBooking.Core.Repositories
{
    //Repository = använder interface ICustomerRepository
    //här ligger koden till databasen
    public class CustomerRepository : ICustomerRepository
    {
        //databaskopplingen
        private readonly AppDbContext _context;
        //konstruktor
        //AppDbContext skickas in via Dependency Injection
        public CustomerRepository(AppDbContext context)
        {
            _context = context;
        }
        /// <summary>
        /// lägger till ny kund i databasen
        /// </summary>
        /// <param name="customer"></param>
        /// <returns></returns>
        public async Task AddAsync(Customer customer)
        {
            //lägger till kund i databasen
            await _context.Customers.AddAsync(customer);
            //sparar ändringar
            await _context.SaveChangesAsync();
        }
        /// <summary>
        /// raderar kund med specifikt id
        /// </summary>
        /// <param name="id"></param>
        public async Task DeleteAsync(int id)
        {
            //letar upp kund med specifikt id
            var customer = await _context.Customers
                .FirstOrDefaultAsync(c => c.Id == id);
            //om kunden hittas raderas den
            if(customer != null)
            {
                _context.Customers.Remove(customer);
                await _context.SaveChangesAsync();
            }
        }
        /// <summary>
        /// hämtar alla kunder
        /// </summary>
        /// <returns>lista med kunder</returns>
        public async Task<List<Customer>> GetAllCustomersAsync()
        {
            return await _context.Customers.ToListAsync();
        }
        /// <summary>
        /// hämtar kund med specifikt id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<Customer?> GetCustomerByIdAsync(int id)
        {
            return await _context.Customers
                .FirstOrDefaultAsync(c => c.Id == id);
        }
        /// <summary>
        /// hämtar en kund baserat på email
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public async Task<Customer?> GetCustomerByEmailAsync(string email)
        {
            //hämtar första kunden med den angivna email
            return await _context.Customers
                .FirstOrDefaultAsync(c => c.Email == email);
        }
        /// <summary>
        /// uppdaterar specifik kund
        /// </summary>
        /// <param name="customer"></param>
        /// <returns></returns>
        public async Task UpdateAsync(Customer customer)
        {
            //markerar objekt som uppdaterat
            _context.Customers.Update(customer);
            //sparar uppdateringen i databasen
            await _context.SaveChangesAsync();
        }
    }
}
