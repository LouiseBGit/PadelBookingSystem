using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PadelBooking.Core.Models;

namespace PadelBooking.Core.Interfaces
{
    /// <summary>
    /// interface -för hantrering av kunder i databasen
    /// </summary>
    //interface -vad som ska göras, men inte hur det görs (som ett kontrakt)
    //metoderna som måste finnas för att hantera bokning
    //Task = asynkron metod --> annat kan göras undertiden
    public interface ICustomerRepository
    {
        /// <summary>
        /// hämta alla kunder från databasen
        /// </summary>
        /// <returns>returnerar en lista</returns>
        Task<List<Customer>> GetAllCustomersAsync();
        /// <summary>
        /// hämta en kund med ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns>returnerar kund på specifikt ID, eller null om ingen kund finns</returns>
        Task<Customer?> GetCustomerByIdAsync(int id);
        /// <summary>
        /// lägg till kund
        /// </summary>
        /// <param name="customer"></param>
        Task AddAsync(Customer customer);
        /// <summary>
        /// uppdatera kund
        /// </summary>
        /// <param name="customer"></param>
        Task UpdateAsync(Customer customer);
        /// <summary>
        /// ta bort kund med ID
        /// </summary>
        /// <param name="id"></param>
        Task DeleteAsync(int id);



    }
}
