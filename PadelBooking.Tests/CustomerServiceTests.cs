using Microsoft.VisualStudio.TestPlatform.ObjectModel.DataCollector.InProcDataCollector;
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
        /// <summary>
        /// testar att en giltig kund kan läggas till 
        /// </summary>
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

        /// <summary>
        /// testar att en existerande kund kan raderas
        /// </summary>
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
        /// <summary>
        /// testar att det inte går att ta bort en kund som inte finns
        /// </summary>
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
        /// <summary>
        /// testar att det inte går att lägga in kunder med samma email
        /// </summary>
        [TestMethod]
        public async Task CreateCustomer_ShouldReturnFalse_WhenEmailAlreadyExists()
        {
            //arrange
            //skapar en kund
            var existingCustomer = new Customer
            {
                Id = 1,
                FirstName = "Elof",
                LastName = "Björn",
                Email = "elbrum@testis.se"
            };

            var mock = new Mock<ICustomerRepository>();

            //låtsar att epost-adressen redan finns i databasen
            mock.Setup(x => x.GetCustomerByEmailAsync("elbrum@testis.se"))
                .ReturnsAsync(existingCustomer);

            var service = new CustomerService(mock.Object);

            //försöker skapa en kund med samma epost
            var customer = new Customer
            {
                FirstName = "Frida",
                LastName = "Björk",
                Email = "elbrum@testis.se"
            };

            //act
            //anropar metoden som ska testas
            var result = await service.CreateCustomerAsync(customer);

            //assert
            //kollar så att kunden inte får skapas
            Assert.IsFalse(result);
        }

        [TestMethod]
        public async Task GetCustomerById_ShouldReturnCustomer_WhenCustomerExists()
        {
            //arrange
            var customer = new Customer
            {
                Id = 1,
                FirstName = "Lisa",
                LastName = "Grön",
                Email = "lg@test.nu"
            };

            var mock = new Mock<ICustomerRepository>();

            //låtsar att kunden finns
            mock.Setup(x => x.GetCustomerByIdAsync(1))
                .ReturnsAsync(customer);

            var service = new CustomerService(mock.Object);

            //act
            var result = await service.GetCustomerByIdAsync(1);

            //assert
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Id);
            Assert.AreEqual("Lisa", result.FirstName);
        }

        /// <summary>
        /// testar att en lista med alla kunder returneras 
        /// </summary>
        [TestMethod]
        public async Task GetAllCustomers_ShouldReturnListOfCustomers()
        {
            //arrange
            //skapar lista som repository ska returnera
            var customerList = new List<Customer>
            {
                new Customer
                {
                    Id = 1,
                    FirstName = "Anna"
                },
                new Customer
                {
                    Id = 2,
                    FirstName = "Lasse"
                }
            };

            var mock = new Mock<ICustomerRepository>();

            //låtsas att databasen innehåller två kunder
            mock.Setup(x => x.GetAllCustomersAsync())
                .ReturnsAsync(customerList);

            //skapar service-klassen
            var service = new CustomerService(mock.Object);

            //act
            var result = await service.GetAllCustomerAsync();

            //assert
            //kollar att listan inte är tom
            Assert.IsNotNull(result);

            //kollar att två kunder kan returneras
            Assert.AreEqual(2, result.Count);

            //kollar första kunden
            Assert.AreEqual("Anna", result[0].FirstName);

            //kollar andra kunden
            Assert.AreEqual("Lasse", result[1].FirstName);
        }

        [TestMethod]
        public async Task UpdateCustomer_ShouldReturnTrue_WhenCustomerIsUpdated()
        {
            //arrange
            var customer = new Customer
            {
                Id = 1,
                FirstName = "Anton",
                LastName = "Andersson",
                Email = "aa@mail.nu"
            };

            //skapar en mock av repository
            var mock = new Mock<ICustomerRepository>();

            //säger att UpdateAsync fungerar med just denna kund
            mock.Setup(x => x.UpdateAsync(customer))
                .Returns(Task.CompletedTask);

            var service = new CustomerService(mock.Object);

            //act
            var result = await service.UpdateCustomerAsync(customer);

            //assert
            Assert.IsTrue(result);
        }

        
    }

}


