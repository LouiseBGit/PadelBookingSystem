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
    //Repository = använder interface IBookingRepository
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

        public Task AddAsync(Customer customer)
        {
            throw new NotImplementedException();
        }

        public Task DeleteAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<List<Customer>> GetAllCustomersAsync()
        {
            throw new NotImplementedException();
        }

        public Task<Customer?> GetCustomerByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task UpdateAsync(Customer customer)
        {
            throw new NotImplementedException();
        }
    }
}
