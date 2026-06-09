using PadelBooking.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PadelBooking.Core.Interfaces
{
    public interface ICustomerService
    {
        Task<List<Customer>> GetAllCustomerAsync();
        Task<Customer?> GetCustomerByIdAsync(int id);
        Task<bool> CreateCustomerAsync(Customer customer);
        Task<bool> UpdateCustomerAsync(Customer customer);
        Task<bool> DeleteCustomerAsync(int id);


    }
}
