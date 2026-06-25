using Microsoft.EntityFrameworkCore;
using PadelBooking.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PadelBooking.Core.Data
{
    /// <summary>
    /// representerar databasen med tabellerna
    /// </summary>
    public class AppDbContext : DbContext 
    {
        /// <summary>
        /// konstruktor -inställningar för databaskoppling
        /// </summary>
        /// <param name="options"></param>
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }

        /// <summary>
        /// tabell för kunder
        /// </summary>
        public DbSet<Customer> Customers { get; set; }
        /// <summary>
        /// tabell för bokningar
        /// </summary>
        public DbSet<Booking> Bookings { get; set; }
    }
}
