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
    public class BookingRepository : IBookingRepository
    {
        //databaskopplingen
        private readonly AppDbContext _context;

        //konstruktor
        //AppDbContext skickas in via Dependency Injection
        public BookingRepository(AppDbContext context)
        {
            _context = context;
        }

        public Task AddAsync(Booking booking)
        {
            throw new NotImplementedException();
        }

        public Task DeleteAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<List<Booking>> GetAllBookingsAsync()
        {
            throw new NotImplementedException();
        }

        public Task<Booking?> GetBookingByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task UpdateAsync(Booking booking)
        {
            throw new NotImplementedException();
        }
    }
}
