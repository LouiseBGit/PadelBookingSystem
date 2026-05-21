using PadelBooking.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PadelBooking.Core.Services
{
    /// <summary>
    /// klass för regler
    /// </summary>
    public class CustomerService : ICustomerService
    {
        //
        private readonly ICustomerRepository _repository;
        public CustomerService(ICustomerRepository repository)
        {
            _repository = repository;
        }
    }
}
