using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PadelBooking.Core.Models;

namespace PadelBooking.Core.Interfaces
{
    //interface -vad som ska göras, men inte hur det görs (som ett kontrakt)
    //metoderna som måste finnas för att hantera bokning
    //Task = asynkron metod --> annat kan göras undertiden
    public interface ICustomerRepository
    {
        //hämta alla kunder från databasen
        //returnera som en lista
        Task<List<Customer>> GetAllCustomersAsync();
        //hämta en kund med ID
        //? = null om ingen kund hittas med det ID
        Task<Customer?> GetCustomerByIdAsync(int id);
        //lägg till kund
        Task AddAsync(Customer customer);
        //uppdatera kund
        Task UpdateAsync(Customer customer);
        //ta bort kund med ID
        Task DeleteAsync(int id);



    }
}
