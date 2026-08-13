using Microsoft.IdentityModel.Protocols;
using Moq;
using PadelBooking.Core.Interfaces;
using PadelBooking.Core.Models;
using PadelBooking.Core.Services;
using PadelBooking.Core.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.XPath;

namespace PadelBooking.Tests
{
    [TestClass]
    public class BookingServiceTests
    {
        /// <summary>
        /// testar att giltig bokning godkäns
        /// </summary>
        [TestMethod]
        public async Task CreateBooking_ShouldReturnBookingDto_WhenValidBooking()
        {
            //arrange

            //fejkad repository för bokningar
            var mock = new Mock<IBookingRepository>();

            //fejkad repository för kunder
            var customerMock = new Mock<ICustomerRepository>();

            //låtsas att kunden med id 1 finns
            customerMock.Setup(x => x.GetCustomerByIdAsync(1))
                .ReturnsAsync(new Customer
                {
                    Id = 1,
                    FirstName = "Test",
                    LastName = "Kund"
                });

            //skapar service-klassen och skickar in mockad repository (object är fejk-versionen)
            var service = new BookingService(mock.Object, customerMock.Object);

            //skapar en giltig bokning -med alla rätta regler
            var booking = new CreateBookingDto
            {
                StartTime = new DateTime(2026, 1, 1, 10, 0, 0),
                CourtNumber = 1,
                CustomerId = 1
            };

            //act
            //anropar metoden som ska testas
            var result = await service.CreateBookingAsync(booking);

            //assert
            //kontrollerar att bokning godkäns
            Assert.IsNotNull(result);
        }
        /// <summary>
        /// testar att det inte går att göra bokning utanför öppettiderna
        /// </summary>
        [TestMethod]
        public async Task CreateBooking_ShouldReturnNull_WhenTimeIsInvalid()
        {
            //arrange
            //skapar en mock av repositoryn
            var mock = new Mock<IBookingRepository>();
            var customerMock = new Mock<ICustomerRepository>();
            //låtsas att kunden med id 1 finns
            customerMock.Setup(x => x.GetCustomerByIdAsync(1))
                .ReturnsAsync(new Customer
                {
                    Id = 1,
                    FirstName = "Test",
                    LastName = "Kund"
                });
            
            //skapar service-klassen
            var service = new BookingService(mock.Object, customerMock.Object);

            //skapar en ogiltig bokning
            var booking = new CreateBookingDto
            {
                StartTime = new DateTime(2026, 1, 1, 6, 0, 0),
                CourtNumber = 1,
                CustomerId = 1
            };

            //act
            var result = await service.CreateBookingAsync(booking);

            //assert
            //förväntat svar är null eftersom tiden är ogiltig
            Assert.IsNull(result);

        }
        /// <summary>
        /// testar att bokningar utanför hela timmar nekas
        /// </summary>
        [TestMethod] 
        public async Task CreateBooking_ShouldReturnNull_WhenNotFullHour()
        {
            //arrange
            //skapar en mock av repositoryn
            var mock = new Mock<IBookingRepository>();
            var customerMock = new Mock<ICustomerRepository>();

            //låtsas att kunden med id 1 finns
            customerMock.Setup(x => x.GetCustomerByIdAsync(1))
                .ReturnsAsync(new Customer
                {
                    Id = 1,
                    FirstName = "Test",
                    LastName = "Kund"
                });

            //skapar service-klassen
            var service = new BookingService(mock.Object, customerMock.Object);

            //skapar en ogiltig bokning
            var booking = new CreateBookingDto
            {
                StartTime = new DateTime(2026, 1, 1, 10, 30, 0),
                CourtNumber = 1,
                CustomerId = 1
            };
            //act
            var result = await service.CreateBookingAsync(booking);

            //assert
            //förväntat svar är null eftersom tiden inte är en hel timme
            Assert.IsNull(result);
        }
        /// <summary>
        /// testar att man inte kan göra en dubbelbokning
        /// </summary>
        [TestMethod]
        public async Task CreateBooking_ShouldReturnNull_WhenDoubleBooking()
        {
            //arrange
            var mock = new Mock<IBookingRepository>();
            //skapar en fejkad repository för kunder
            var customerMock = new Mock<ICustomerRepository>();

            //låtsas att kunden med id 1 finns
            //så att det är dubbelbokningen som gör bokningen ogiltig
            customerMock.Setup(x => x.GetCustomerByIdAsync(1))
                .ReturnsAsync(new Customer
                {
                    Id = 1,
                    FirstName = "Test",
                    LastName = "Kund"
                });

            //säger att bokningen redan finns
            mock.Setup(x => x.BookingExistsAsync(1, new DateTime(2026, 1, 1, 10, 0, 0), It.IsAny<int?>()))
                .ReturnsAsync(true);

            var service = new BookingService(mock.Object, customerMock.Object);

            //försöker lägga in samma bokning igen
            var booking = new CreateBookingDto
            {
                StartTime = new DateTime(2026, 1, 1, 10, 0, 0),
                CourtNumber = 1,
                CustomerId = 1
            };

            //act
            var result = await service.CreateBookingAsync(booking);

            //assert
            //förväntar oss null eftersom dubbelbokning inte får göras
            Assert.IsNull(result);

        }
        /// <summary>
        /// testar att en bokning inte kan uppdateras
        /// om den nya tiden och banan redan är bokad
        /// </summary>
        [TestMethod] 
        public async Task UppdateBooking_ShouldReturnNull_WhenDoubleBookingExists()
        {
            //arrange
            //mockar repository så vi inte använder riktig data
            var mock = new Mock<IBookingRepository>();
            //skapar en fejkad repository för kunder
            var customerMock = new Mock<ICustomerRepository>();

            //låtsas att bokningen redan finns med id 1, så testet kommer till koll för dubbelbokning
            mock.Setup(x => x.GetBookingByIdAsync(1))
                .ReturnsAsync(new Booking
                {
                    Id = 1,
                    CourtNumber = 2,
                    StartTime = new DateTime(2026, 1, 1, 12, 0, 0),
                    CustomerId = 1
                });

            //låtsas att kunden med id 1 finns
            //så att det är dubbelbokningen som gör uppdateringen ogiltig
            customerMock.Setup(x => x.GetCustomerByIdAsync(1))
                .ReturnsAsync(new Customer
                {
                    Id = 1,
                    FirstName = "Test",
                    LastName = "Kund"
                });

            //låtsas att det redan finns en annan bokning
            //på bana 1 klockan 10:00
            //id 1 skickas med för att ignorera bokningen som själv ska uppdateras
            mock.Setup(x => x.BookingExistsAsync(
                    1,
                    new DateTime(2026, 1, 1, 10, 0, 0),
                    1))
                .ReturnsAsync(true);


            //skapar BookingService med båda fejkade repositories
            var service = new BookingService(mock.Object, customerMock.Object);

            //skapar DTO:n med de nya värden som bokningen ska uppdateras till
            var bookingUpdate = new UpdateBookingDto
            {
                CourtNumber = 1,
                StartTime = new DateTime(2026, 1, 1, 10, 0, 0),
                CustomerId = 1
            };

            //act
            //försöker uppdatera bokningen med id 1
            var result = await service.UpdateBookingAsync(1, bookingUpdate);

            //assert
            //förväntar null eftersom den nya tiden och banan redan är bokad
            Assert.IsNull(result);
        }
        /// <summary>
        /// testar att man inte kan boka en bana på fel ban-nummer
        /// </summary>
        [TestMethod]
        public async Task CreateBooking_ShouldReturnNull_WhenCourtNumberIsInvalid()
        {
            //arrange
            //skapar en mock av repository så vi slipper använda riktig databas
            var mock = new Mock<IBookingRepository>();

            //skapar en fejk-repository för kunder
            var customerMock = new Mock<ICustomerRepository>();

            //låtsas att kunden med id 1 finns
            //så att det är bannumret som gör bokningen ogiltig
            customerMock.Setup(x => x.GetCustomerByIdAsync(1))
                .ReturnsAsync(new Customer
                {
                    Id = 1,
                    FirstName = "Test",
                    LastName = "Kund"
                });

            //skapar BookingService med båda de fejkade repositories
            var service = new BookingService(mock.Object, customerMock.Object);

            //skapar en bokning med ogiltig bana
            var booking = new CreateBookingDto
            {
                StartTime = new DateTime(2026, 1, 1, 10, 0, 0),
                CourtNumber = 0,
                CustomerId = 1
            };

            //act
            //anropar metoden som ska testas -försöker skapa bokningen
            var result = await service.CreateBookingAsync(booking);

            //assert
            //förväntar null eftersom det är ett ogiltigt nummer
            Assert.IsNull(result);
        }
        /// <summary>
        /// testar att man kan uppdatera en bokning när alla regler följs
        /// </summary>
        [TestMethod]
        public async Task UpdateBooking_ShouldReturnBookingDto_WhenValidUpdate()
        {
            //arrange

            //skapar en fejkad repository för bokningar
            var mock = new Mock<IBookingRepository>();

            //skapar en fejkad repository för kunder
            var customerMock = new Mock<ICustomerRepository>();

            //låtsas att kunden med id 1 finns
            customerMock.Setup(x => x.GetCustomerByIdAsync(1))
                .ReturnsAsync(new Customer
                {
                    Id = 1,
                    FirstName = "Test",
                    LastName = "Kund"
                });

            //låtsas att bokningen med id 1 redan finns
            //det är den bokning som vi ska uppdatera
            mock.Setup(x => x.GetBookingByIdAsync(1))
                .ReturnsAsync(new Booking
                {
                    Id = 1,
                    CourtNumber = 1,
                    StartTime = new DateTime(2026, 1, 1, 10, 0, 0),
                    CustomerId = 1
                });

            //låtsas att den nya tiden och banan inte krockar
            //med någon annan bokning
            mock.Setup(x => x.BookingExistsAsync(
                2,
                new DateTime(2026, 1, 1, 12, 0, 0),
                1))
                .ReturnsAsync(false);

            //skapar BookingService med båda fejkade repositories
            var service = new BookingService(mock.Object, customerMock.Object);

            //skapar DTO:n med de nya värdena för bokningen
            var bookingToUpdate = new UpdateBookingDto
            {
                CourtNumber = 2,
                StartTime = new DateTime(2026, 1, 1, 12, 0, 0),
                CustomerId = 1
            };

            //act

            //försöker uppdatera bokningen med id 1
            var result = await service.UpdateBookingAsync(1, bookingToUpdate);

            //assert

            //kontrollerar att uppdateringen lyckades
            //en giltig uppdatering ska returnera en BookingDto
            Assert.IsNotNull(result);
        }
        /// <summary>
        /// testar att lediga tider returneras för en specifik bana
        /// när vissa tider redan är bokade
        /// </summary>
        [TestMethod]
        public async Task GetAvailableTimes_ShouldReturnAvailableHours_WhenSomeTimesAreBooked()
        {
            //arrange
            //skapar en fejkad repository för bokninhar
            var mock = new Mock<IBookingRepository>();

            //skapar en fejkad repository för bokningar
            var customerMock = new Mock<ICustomerRepository>();

            //låtsas att bana 1 har två bokade tider den valda dagen -kl 10.00 och 14.00
            mock.Setup(x => x.GetBookingsByDateAndCourtAsync(
                new DateTime(2026, 1, 1),
                1))
                .ReturnsAsync(new List<Booking>
                {
                    new Booking
                    {
                        CourtNumber = 1,
                        StartTime = new DateTime(2026, 1, 1, 10, 0, 0)
                    },
                    new Booking
                    {
                         CourtNumber = 1,
                         StartTime = new DateTime(2026, 1, 1, 14, 0, 0)
                    }
                });

            //skapar BookingService med båda fejk-repositories
            var service = new BookingService(mock.Object, customerMock.Object);

            //act
            //hämtar lediga tider för bana 1 den 1 jan
            var result = await service.GetAvailableTimesAsync(
                new DateTime(2026, 1, 1),
                1);

            //assert
            //10 och 14 ska inte finnas då de redan är bokade
            Assert.IsFalse(result.Contains(10));
            Assert.IsFalse(result.Contains(14));

            //11.00 och 15.00 ska vara lediga
            Assert.IsTrue(result.Contains(11));
            Assert.IsTrue(result.Contains(15));

            //13 tider ska vara lediga
            Assert.AreEqual(13, result.Count);
        }
        /// <summary>
        /// testar att service-metoden reutnerar de bokningar som repositoryn hämtar mellan två angivna datum
        /// </summary>
        [TestMethod]
        public async Task GetBookingsBetweenDates_ShouldReturnBookingSummary()
        {
            //arrange
            //skapar en mock av repository istället för att avända riktig data
            var mock = new Mock<IBookingRepository>();

            //skapar en fejkad repositry för kunder
            var customerMock = new Mock<ICustomerRepository>();
            
            //skapar testdata som repositoryn ska returnera 
            var bookings = new List<Booking>
            {
                new Booking
                {
                    Id = 1,
                    CourtNumber = 1,
                    StartTime = new DateTime(2026, 1, 10, 10, 0,0),
                    //lägger till kund eftersom Service använder b.Customer när Booking görs om till BookingDto
                    Customer = new Customer
                    {
                        FirstName = "Test",
                        LastName = "Kund"
                    }
                }
            };

            //låtsar att repositoryn returnerar vår testbokning när bokningar mellan dessa datum hämtas
            mock.Setup(x => x.GetBookingsBetweenDatesAsync(
                new DateTime(2026, 1, 1),
                new DateTime(2026, 1, 31)))
                .ReturnsAsync(bookings);

            //skapar BookingService med båda fejkade repositories
            var service = new BookingService(mock.Object, customerMock.Object);

            //act
            //hämtar sammanfattningen för bokningar mellan datum
            var result = await service.GetBookingsBetweenDatesAsync(
                new DateTime(2026, 1, 1),
                new DateTime(2026, 1, 31));

            //assert
            //kollar om det totalt finns en bokning
            Assert.AreEqual(1, result.TotalBookings);

            //koollar att bokningen ligger på bana 1
            Assert.AreEqual(1, result.Court1Total);

            //inga bokningar ska finnas på bana 2 och 3
            Assert.AreEqual(0, result.Court2Total);
            Assert.AreEqual(0, result.Court3Total);

            //kollar så bokningslistan innehåller en bokning
            Assert.AreEqual(1, result.Bookings.Count);

            //kollar så bokningen i listan ligger på bana 1
            Assert.AreEqual(1, result.Bookings[0].CourtNumber);

            
        }
    }
}
