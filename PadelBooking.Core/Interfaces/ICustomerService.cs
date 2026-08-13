using PadelBooking.Core.DTOs;
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
        Task<List<CustomerDto>> GetAllCustomerAsync();
        Task<CustomerDto?> GetCustomerByIdAsync(int id);
        Task<CustomerDto?> CreateCustomerAsync(CreateCustomerDto dto);
        Task<CustomerDto?> UpdateCustomerAsync(int id, UpdateCustomerDto dto);
        Task<bool> DeleteCustomerAsync(int id);


    }
}
