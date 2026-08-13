using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.DataCollector.InProcDataCollector;
using Moq;
using PadelBooking.Core.DTOs;
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
        /// testar att en giltig kund kan skapas
        /// </summary>
        [TestMethod]
        public async Task CreateCustomer_ShouldReturnCustomerDto_WhenValidCustomer()
        {
            //arrange
            //skapar en låtsas-repository istället för riktiga databasen
            var mock = new Mock<ICustomerRepository>();

            //låtsas att ingen kund med denna epost redan finns
            mock.Setup(x => x.GetCustomerByEmailAsync("t@test.nu"))
                .ReturnsAsync((Customer?)null);

            //skapar service-klassen med mock istället för den riktiga databasen
            var service = new CustomerService(mock.Object);

            //skapar dto med uppgifter för den nya kunden
            var customer = new CreateCustomerDto
            {
                FirstName = "Elof",
                LastName = "Björn",
                Email = "t@test.nu",
                PhoneNumber = "729292663"
            };

            //act
            //försöker skapa kunden
            var result = await service.CreateCustomerAsync(customer);

            //assert
            //kontrollerar att servuce returnerar en CustomerDto och inte null
            Assert.IsNotNull(result);

        }

        /// <summary>
        /// testar att en existerande kund kan raderas
        /// </summary>
        [TestMethod]
        public async Task DeleteCustomer_ShouldReturnTrue_WhenCustomerExists()
        {
            //arrange
            //skapar en fejkad repository
            var mock = new Mock<ICustomerRepository>();

            //skapar en kund som låtsas finnas i databasen
            var customer = new Customer
            {
                Id = 1,
                FirstName = "Elof"
            };

            //låtsas att repisotoryn hittar kund med id 1
            mock.Setup(x => x.GetCustomerByIdAsync(1))
                .ReturnsAsync(customer);

            //låtsas att borttagningen fungerade utan problem
            mock.Setup(x => x.DeleteAsync(1))
                .Returns(Task.CompletedTask);

            //skapar serviceklassen med den fejkade repositoryn
            var service = new CustomerService(mock.Object);

            //act
            //försöker radera kunden med id 1
            var result = await service.DeleteCustomerAsync(1);

            //assert
            //kollar att service returnerar true eftersom kunden fanns och kunde raderas
            Assert.IsTrue(result);
        }
        /// <summary>
        /// testar att det inte går att ta bort en kund som inte finns
        /// </summary>
        [TestMethod]
        public async Task DeleteCustomer_ShouldReturnFalse_WhenCustomerDoesNotExist()
        {
            //arrange
            //skapar fejkad repositry
            var mock = new Mock<ICustomerRepository>();
            
            //låtsas att repository inte hittar kunden med id 1. Null betyder att kunden inte finns
            mock.Setup(x => x.GetCustomerByIdAsync(1))
                .ReturnsAsync((Customer?)null);

            //skapar service-klassen med den fejkade repositoryn
            var service = new CustomerService(mock.Object);

            //act
            //försöker radera kunden med id 1
            var result = await service.DeleteCustomerAsync(1);

            //assert
            //kollar att service returnerar false eftersom kunden inte finns och därmed inte kan raderas
            Assert.IsFalse(result);
        }
        /// <summary>
        /// testar att det inte går att lägga in kunder med samma email
        /// </summary>
        [TestMethod]
        public async Task CreateCustomer_ShouldReturnNull_WhenEmailAlreadyExists()
        {
            //arrange
            //skapar en kund som låtsas redan finnas i databasen
            var existingCustomer = new Customer
            {
                Id = 1,
                FirstName = "Elof",
                LastName = "Björn",
                Email = "elbrum@testis.se"
            };

            //skapar en fejkad repository
            var mock = new Mock<ICustomerRepository>();

            //låtsar att en kund med denna epost-adressen redan finns i databasen
            mock.Setup(x => x.GetCustomerByEmailAsync("elbrum@testis.se"))
                .ReturnsAsync(existingCustomer);

            //skapar service-klassen med den fejkade repositoryn
            var service = new CustomerService(mock.Object);

            //försöker skapa en kund med samma epost
            var customer = new CreateCustomerDto
            {
                FirstName = "Frida",
                LastName = "Björk",
                Email = "elbrum@testis.se"
            };

            //act
            //anropar metoden som ska testas -försöker skapa kunden
            var result = await service.CreateCustomerAsync(customer);

            //assert
            //kollar så att service returnerar null
            Assert.IsNull(result);
        }

        [TestMethod]
        public async Task GetCustomerById_ShouldReturnCustomer_WhenCustomerExists()
        {
            //arrange
            //skapar en kund som låtsas finnas i databasen
            var customer = new Customer
            {
                Id = 1,
                FirstName = "Lisa",
                LastName = "Grön",
                Email = "lg@test.nu"
            };

            //skapar fejkad repository
            var mock = new Mock<ICustomerRepository>();

            //låtsar att repositoryn hittar kunden med id 1
            mock.Setup(x => x.GetCustomerByIdAsync(1))
                .ReturnsAsync(customer);

            //skapar serviceklassen med den fejkade repositoryn
            var service = new CustomerService(mock.Object);

            //act
            //försöker hämta kunden med id 1
            var result = await service.GetCustomerByIdAsync(1);

            //assert
            //kollar först att en kund returneras
            Assert.IsNotNull(result);
            //kollar att rätt kund returneras
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
            //skapar lista som repository ska returnera, som låtsas finnas i databasen
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

            //skapar fejkas repository
            var mock = new Mock<ICustomerRepository>();

            //låtsas att repository hämtar Anna och Lasse från databasen
            mock.Setup(x => x.GetAllCustomersAsync())
                .ReturnsAsync(customerList);

            //skapar service-klassen med fejkad repository
            var service = new CustomerService(mock.Object);

            //act
            //anropar service-metoden som hämtar alla kunder
            var result = await service.GetAllCustomerAsync();

            //assert
            //kollar att en lista kom tillbaka
            Assert.IsNotNull(result);

            //kollar att listan innehåller två kunder
            Assert.AreEqual(2, result.Count);

            //kollar att första kunden är Anna
            Assert.AreEqual("Anna", result[0].FirstName);

            //kollar att andra kunden är Lasse
            Assert.AreEqual("Lasse", result[1].FirstName);
        }

        /// <summary>
        /// testar att en kund kan uppdateras 
        /// och att service returnerar den uppdaterade kunden
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task UpdateCustomer_ShouldReturnCustomerDto_WhenCustomerIsUpdated()
        {
            //arrange
            //skapar kunden som låtsas redan finnas i databasen
            var customer = new Customer
            {
                Id = 1,
                FirstName = "Anton",
                LastName = "Andersson",
                Email = "aa@mail.nu"
            };

            //skapar en fejkad repository
            var mock = new Mock<ICustomerRepository>();

            //låtsas att repositoryn hittar kunden med id 1
            mock.Setup(x => x.GetCustomerByIdAsync(1))
                .ReturnsAsync(customer);

            //skapar service-klassen med den fejkade repositoryn
            var service = new CustomerService(mock.Object);

            //skapar DTO med de nya uppgifterna kunden ska uppdateras med
            var dto = new UpdateCustomerDto
            {
                FirstName = "Anton",
                LastName = "Andersson",
                Email = "anton@test.nu"
            };

            //act
            //försöker uppdatera kunden med id 1
            var result = await service.UpdateCustomerAsync(1, dto);

            //assert
            //kollar att service returnerar en kund och inte null
            Assert.IsNotNull(result);

            //kollar att rätt kund uppdateras
            Assert.AreEqual(1, result.Id);

            //kollar att den nya emailen returneras
            Assert.AreEqual("anton@test.nu", result.Email);
        }

        /// <summary>
        /// testar att en kund som inte finns inte kan uppdateras
        /// </summary>
        [TestMethod]
        public async Task UpdateCustomer_ShouldReturnNull_WhenCustomerDoesNotExist()
        {
            //arrange
            //skapar fejkad repository
            var mock = new Mock<ICustomerRepository>();

            //låtsas att kund med ID 1 inte finns (null)
            mock.Setup(x => x.GetCustomerByIdAsync(1))
                .ReturnsAsync((Customer?)null);

            //skapar service
            var service = new CustomerService(mock.Object);

            //uppgifterna som kunden ska försöka uppdateras med
            var dto = new UpdateCustomerDto
            {
                FirstName = "Anton",
                LastName = "Antonsen",
                Email = "aa@test.sen",
                PhoneNumber = "9872348972"
            };

            //act
            //försöker uppdatera kunden med id 1
            var result = await service.UpdateCustomerAsync(1, dto);

            //assert
            //kunden finns inte, därför ska service returnera null
            Assert.IsNull(result);
        }

        
    }

}


