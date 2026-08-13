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
            return await _context.Bookings
                .Include(b => b.Customer)
                .ToListAsync();
        }
        //hämtar en bokning för det ID man skickar in
        public async Task<Booking?> GetBookingByIdAsync(int id)
        {
            //returnera första raden som stämmer överrens, annars null
            return await _context.Bookings
                .Include(b => b.Customer)
                .FirstOrDefaultAsync(b => b.Id == id);
        }
        //uppdaterar en redan existerande bokning i databasen
        public async Task UpdateAsync(Booking booking)
        {
            

            _context.Bookings.Update(booking);
            //spara uppdateringen
            await _context.SaveChangesAsync();
        }
        /// <summary>
        /// kontrollerar om det redan finns en likadan bokning
        /// </summary>
        /// <param name="courtNumber"></param>
        /// <param name="startTime"></param>
        /// <param name="excludeId"></param>
        /// <returns></returns>
        public async Task<bool> BookingExistsAsync(int courtNumber, DateTime startTime, int? excludeId = null)
        {
            return await _context.Bookings.AnyAsync(b =>
            b.CourtNumber == courtNumber &&
            b.StartTime == startTime &&
            (!excludeId.HasValue || b.Id != excludeId));
            
        }
        /// <summary>
        /// Hämtar alla bokningar för specifikt datum
        /// </summary>
        /// <param name="date"></param>
        /// <returns>lista med bokningar för specifik dag</returns>
        public async Task<List<Booking>> GetBookingsByDateAsync(DateTime date)
        {
            return await _context.Bookings
                .Include(b => b.Customer)
                .Where(b => b.StartTime.Date == date.Date)
                .ToListAsync();
        }
        /// <summary>
        /// Hämtar bokningar mellan två specifika datum 
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        public async Task<List<Booking>> GetBookingsBetweenDatesAsync(DateTime startDate, DateTime endDate)
        {
            return await _context.Bookings
                .Include(b => b.Customer)
                .Where(b => b.StartTime >= startDate &&
                b.StartTime <= endDate)
                .ToListAsync();
        }
        /// <summary>
        /// hämtar alla bokningar för en specifik dag och bana
        /// </summary>
        /// <param name="date"></param>
        /// <param name="courtNumber"></param>
        /// <returns></returns>
        public async Task<List<Booking>> GetBookingsByDateAndCourtAsync(DateTime date, int courtNumber)
        {
            return await _context.Bookings
                .Include(b => b.Customer)
                .Where(b => b.StartTime.Date == date.Date && b.CourtNumber == courtNumber)
                .ToListAsync();
        }
    }
}
