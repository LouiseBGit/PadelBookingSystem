using Moq;
using PadelBooking.Core.Interfaces;
using PadelBooking.Core.Models;
using PadelBooking.Core.Services;
using System.ComponentModel.DataAnnotations;

namespace PadelBooking.Tests
{
    [TestClass]
    public class CustomerServiceTests
    {
        [TestMethod]
        public async Task CreateCustomer_ShouldReturnTrue_WhenValidCustomer()
        {
            //arrange
            //skapar en låtsas-repository istället för riktiga databasen
            var mock = new Mock<ICustomerRepository>();

            //skapar service-klassen med mock istället för den riktiga databasen
            var service = new CustomerService(mock.Object);

            //skapar en giltig kund
            var customer = new Customer
            {
                FirstName = "Elof",
                LastName = "Björn",
                Email = "elbj@testaren.nu",
                PhoneNumber = "729292663"
            };

            //act
            var result = await service.CreateCustomerAsync(customer);

            //assert
            //kontrollerar resultatet 
            Assert.IsTrue(result);

        }
        [TestMethod]
        public async Task DeleteCustomer_ShouldReturnTrue_WhenCustomerExists()
        {
            //arrange
            var mock = new Mock<ICustomerRepository>();

            //skapar en kund som finns i databasen
            var customer = new Customer
            {
                Id = 1,
                FirstName = "Elof"
            };

            //säger att kunden finns
            mock.Setup(x => x.GetCustomerByIdAsync(1))
                .ReturnsAsync(customer);

            //berättar för mock att delete fungerar
            mock.Setup(x => x.DeleteAsync(1))
                .Returns(Task.CompletedTask);

            var service = new CustomerService(mock.Object);

            //act
            var result = await service.DeleteCustomerAsync(1);

            //assert
            //förväntar att delete funkar (true)
            Assert.IsTrue(result);
        }

        [TestMethod]
        public async Task DeleteCustomer_ShouldReturnFalse_WhenCustomerDoesNotExist()
        {
            //arrange
            var mock = new Mock<ICustomerRepository>();
            //säger till mock att specifik kund inte hittas
            mock.Setup(x => x.GetCustomerByIdAsync(1))
                .ReturnsAsync((Customer?)null);

            var service = new CustomerService(mock.Object);

            //act
            var result = await service.DeleteCustomerAsync(1);

            //assert
            //förväntas att kunden inte finns (false)
            Assert.IsFalse(result);
        }
    }

}


