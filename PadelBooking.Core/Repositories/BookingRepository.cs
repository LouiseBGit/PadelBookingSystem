using Microsoft.Data.SqlClient;
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
        //CRUD -create, read, update, delete

        //lägga till en ny bokning i databasen
        public async Task AddAsync(Booking booking)
        {
            await _context.Bookings.AddAsync(booking);
            //sparar i databasen
            await _context.SaveChangesAsync();

        }

        public async Task DeleteAsync(int id)
        {
            //hitta bokningen för ett visst ID
            var booking = await _context.Bookings.FirstOrDefaultAsync(b => b.Id == id);

            //om bokningen finns
            if (booking != null)
            {
                //ta bort bokning
                _context.Bookings.Remove(booking);
                //spara ändringarna
                await _context.SaveChangesAsync();
            }
        }
        //hämtar alla bokningar från databasen 
        public async Task<List<Booking>> GetAllBookingsAsync()
        {
            //tar tabellen bookings i databasen som returneras som en lista
            //await = väntar på databasen men utan att blockera programmet
            return await _context.Bookings.ToListAsync();
        }
        //hämtar en bokning för det ID man skickar in
        public async Task<Booking?> GetBookingByIdAsync(int id)
        {
            //returnera första raden som stämmer överrens, annars null
            return await _context.Bookings.FirstOrDefaultAsync(b => b.Id == id);
        }
        //uppdaterar en redan existerande bokning i databasen
        public async Task UpdateAsync(Booking booking)
        {
            _context.Bookings.Update(booking);
            //spara uppdateringen
            await _context.SaveChangesAsync();
        }
    }
}
