using Microsoft.IdentityModel.Protocols;
using Moq;
using PadelBooking.Core.Interfaces;
using PadelBooking.Core.Models;
using PadelBooking.Core.Services;
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
        public async Task CreateBooking_ShouldReturnTrue_WhenValidBooking()
        {
            //arrange

            //skapar mock av IBookingRepository istället för att använda riktig databas
            var mock = new Mock<IBookingRepository>();
            //gör så att det ser ut som att inga bokningar finns
            mock.Setup(x => x.GetAllBookingsAsync())
                .ReturnsAsync(new List<Booking>());

            //skapar service-klassen och skickar in mockad repository (object är fejk-versionen)
            var service = new BookingService(mock.Object);

            //skapar en giltig bokning -med alla rätta regler
            var booking = new Booking
            {
                 StartTime = new DateTime(2026, 1, 1, 10, 0, 0),
                 CourtNumber = 1
            };

            //act
            //anropar metoden som ska testas
            var result = await service.CreateBookingAsync(booking);

            //assert
            //kontrollerar att bokning godkäns
            Assert.IsTrue(result);
        }
        /// <summary>
        /// testar att det inte går att göra bokning utanför öppettiderna
        /// </summary>
        [TestMethod]
        public async Task CreateBooking_ShouldReturnFalse_WhenTimeIsInvalid()
        {
            //arrange
            //skapar en mock av repositoryn
            var mock = new Mock<IBookingRepository>();
            //låtsar att databasen inte innehåller bokningar
            mock.Setup(x => x.GetAllBookingsAsync())
                .ReturnsAsync(new List<Booking>());
            //skapar service-klassen
            var service = new BookingService(mock.Object);

            //skapar en ogiltig bokning
            var booking = new Booking
            {
                StartTime = new DateTime(2026, 1, 1, 6, 0, 0),
                CourtNumber = 1
            };

            //act
            var result = await service.CreateBookingAsync(booking);

            //assert
            //förväntat svar är false
            Assert.IsFalse(result);

        }
        /// <summary>
        /// testar att bokningar utanför hela timmar nekas
        /// </summary>
        [TestMethod] 
        public async Task CreateBooking_ShouldReturnFalse_WhenNotFullHour()
        {
            //arrange
            //skapar en mock av repositoryn
            var mock = new Mock<IBookingRepository>();
            //låtsar att databasen inte innehåller bokningar
            mock.Setup(x => x.GetAllBookingsAsync())
                .ReturnsAsync(new List<Booking>());
            //skapar service-klassen
            var service = new BookingService(mock.Object);

            //skapar en ogiltig bokning
            var booking = new Booking
            {
                StartTime = new DateTime(2026, 1, 1, 10, 30, 0),
                CourtNumber = 1
            };

            //act
            var result = await service.CreateBookingAsync(booking);

            //assert
            //förväntat svar är false
            Assert.IsFalse(result);
        }
        /// <summary>
        /// testar att man inte kan göra en dubbelbokning
        /// </summary>
        [TestMethod]
        public async Task CreateBooking_ShouldReturnFalse_WhenDoubleBooking()
        {
            //arrange
            var mock = new Mock<IBookingRepository>();

            //säger att bokningen redan finns
            mock.Setup(x => x.BookingExistsAsync(1, new DateTime(2026, 1, 1, 10, 0, 0), It.IsAny<int?>()))
                .ReturnsAsync(true);

            var service = new BookingService(mock.Object);

            //försöker lägga in samma bokning igen
            var booking = new Booking
            {
                Id = 1,
                StartTime = new DateTime(2026, 1, 1, 10, 0, 0),
                CourtNumber = 1
            };

            //act
            var result = await service.CreateBookingAsync(booking);

            //assert
            //förväntar oss false eftersom dubbelbokning inte får göras
            Assert.IsFalse(result);


            ////arrange
            ////gör en bokning
            //var existingBooking = new Booking
            //{
            //    StartTime = new DateTime(2026, 1, 1, 10, 0, 0),
            //    CourtNumber = 1
            //};

            //var mock = new Mock<IBookingRepository>();

            ////fejkar att databasen redan innehåller en bokning
            //mock.Setup(x => x.GetAllBookingsAsync())
            //    .ReturnsAsync(new List<Booking>
            //    {
            //        existingBooking
            //    });
            ////skapar service-klassen
            //var service = new BookingService(mock.Object);

            ////försöker skapa en identisk bokning
            //var booking = new Booking
            //{
            //    StartTime = new DateTime(2026, 1, 1, 10, 0, 0),
            //    CourtNumber = 1
            //};

            ////act
            //var result = await service.CreateBookingAsync(booking);

            ////assert
            //Assert.IsFalse(result);

        }
        /// <summary>
        /// testar att man inte kan uppdatera en dubbelbokning 
        /// </summary>
        [TestMethod] 
        public async Task UppdateBooking_ShouldReturnFalse_WhenDoubleBookingExists()
        {
            //arrange
            //mockar repository så vi inte använder riktig data
            var mock = new Mock<IBookingRepository>();

            mock.Setup(x => x.BookingExistsAsync(1, new DateTime(2026, 1, 1, 10, 0, 0), 1))
                .ReturnsAsync(true);

            ////fejkar att det redan finns en bokning
            //mock.Setup(x => x.GetAllBookingsAsync())
            //    .ReturnsAsync(new List<Booking>
            //    {
            //        new Booking
            //        {
            //            Id = 2,
            //            CourtNumber = 1,
            //            StartTime = new DateTime(2026, 1, 1, 10, 0, 0)
            //        }
            //    });

            var service = new BookingService(mock.Object);

            var bookingUpdate = new Booking
            {
                Id = 1,
                CourtNumber = 1,
                StartTime = new DateTime(2026, 1, 1, 10, 0, 0)
            };

            //act
            var result = await service.UpdateBookingAsync(bookingUpdate);

            //assert
            Assert.IsFalse(result);
        }
        /// <summary>
        /// testar att man inte kan boka en bana på fel ban-nummer
        /// </summary>
        [TestMethod]
        public async Task CreateBooing_ShouldReturnFalse_WhenCourtNumberIsInvalid()
        {
            //arrange
            //skapar en mock av repository så vi slipper använda riktig databas
            var mock = new Mock<IBookingRepository>();

            ////ställer in mocken så den inte hittar dubbelbokning
            //mock.Setup(x => x.BookingExistsAsync(0, new DateTime(2026, 1, 1, 10, 0, 0), null))
            //.ReturnsAsync(false);

            //skapar service-klassen och skickar in fejkad mock-repository
            var service = new BookingService(mock.Object);

            //skapar en bokning med ogiltig bana
            var booking = new Booking
            {
                StartTime = new DateTime(2026, 1, 1, 10, 0, 0),
                CourtNumber = 0
            };

            //act
            //anropar metoden som ska testas
            var result = await service.CreateBookingAsync(booking);

            //assert
            //förväntar false eftersom det är ett ogiltigt nummer
            Assert.IsFalse(result);
        }
        /// <summary>
        /// testar att man kan uppdatera en bokning när alla regler följs
        /// </summary>
        [TestMethod]
        public async Task UpdateBooking_ShouldReturnTrue_WhenValidUpdate()
        {
            //arrange
            //skapar en mock av repository
            var mock = new Mock<IBookingRepository>();

            //låtsas att det inte finns några bokningar som krockar
            mock.Setup(x => x.GetAllBookingsAsync())
                .ReturnsAsync(new List<Booking>());

            //skapar service-klassen
            var service = new BookingService(mock.Object);

            //skapar en giltig uppdatering
            var bookingToUpdate = new Booking
            {
                Id = 1,
                CourtNumber = 2,
                StartTime = new DateTime(2026, 1, 1, 12, 0, 0)
            };

            //act
            //anropar metoden som ska testas
            var result = await service.UpdateBookingAsync(bookingToUpdate);

            //assert
            //förväntar true eftersom alla regler följs för uppdartering
            Assert.IsTrue(result);

        }
        /// <summary>
        /// testar så man kan se lediga tider när bokade tider existerar 
        /// </summary>
        [TestMethod]
        public async Task GetAvailableTimes_ShouldReturnAvailableHours_WhenSomeTimesAreBooked()
        {
            //arrange
            //mockar repository så vi inte använder riktig data
            var mock = new Mock<IBookingRepository>();

            //låtsar att två tider redan är bokade
            mock.Setup(x => x.GetBookingsByDateAsync(new DateTime(2026, 1, 1)))
                .ReturnsAsync(new List<Booking>
                {
                    new Booking
                    {
                        CourtNumber = 1,
                        StartTime = new DateTime(2026, 1, 1, 10, 0, 0)
                    },
                    new Booking
                    {
                        CourtNumber = 2,
                        StartTime = new DateTime(2026, 1, 1, 14, 0, 0)
                    }
                });

            var service = new BookingService(mock.Object);

            //act
            var result = await service.GetAvailableTimesAsync(new DateTime(2026, 1, 1));

            //assert
            //bokade tider ska inte finnas med
            Assert.IsFalse(result.Contains(10));
            Assert.IsFalse(result.Contains(14));

            //lediga tider ska finnas med
            Assert.IsTrue(result.Contains(11));
            Assert.IsTrue(result.Contains(15));

            //totalt ska det finnas 14 tider kvar om 2 är bokade
            Assert.AreEqual(14, result.Count);
        }
        /// <summary>
        /// testar att service-metoden reutnerar de bokningar som repositoryn hämtar mellan två angivna datum
        /// </summary>
        [TestMethod]
        public async Task GetBookingsBetweenDates_ShouldReturnBookingsWithinDateRange()
        {
            //arrange
            //skapar en mock av repository istället för att avända riktig data
            var mock = new Mock<IBookingRepository>();

            //skapar testdata som repositoryn ska returnera 
            var bookings = new List<Booking>
            {
                new Booking
                {
                    Id = 1,
                    CourtNumber = 1,
                    StartTime = new DateTime(2026, 1, 10, 10, 0,0)
                }
            };

            //om metoden GetBookingsBetweenDatesAsync anropas med dessa datum ska den returnera listan bookings
            mock.Setup(x => x.GetBookingsBetweenDatesAsync(
                new DateTime(2026, 1, 1),
                new DateTime(2026, 1, 31)))
                .ReturnsAsync(bookings);

            //skapar service-klassen och skickar in den fejkade repositoryn
            var service = new BookingService(mock.Object);

            //act
            //anropar metoden som ska testas 
            var result = await service.GetBookingsBetweenDatesAsync(
                new DateTime(2026, 1, 1),
                new DateTime(2026, 1, 31));

            //assert
            //resultatet ska returnera den bokning som repositoryn reutrnerad 
            Assert.AreEqual(1, result.Count);
            //kontrollerar att bokningen ligger på bana 1.
            Assert.AreEqual(1, result[0].CourtNumber);
        }
    }
}
