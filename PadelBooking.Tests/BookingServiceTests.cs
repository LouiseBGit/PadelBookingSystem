using Moq;
using PadelBooking.Core.Interfaces;
using PadelBooking.Core.Models;
using PadelBooking.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PadelBooking.Tests
{
    [TestClass]
    public class BookingServiceTests
    {
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

        [TestMethod]
        public async Task CreateBooking_ShouldReturnFalse_WhenDoubleBooking()
        {
            //arrange
            var mock = new Mock<IBookingRepository>();

            //säger att bokningen redan finns
            mock.Setup(x => x.BookingExistsAsync(1, new DateTime(2026, 1, 1, 10, 0, 0)))
                .ReturnsAsync(true);

            var service = new BookingService(mock.Object);

            var booking = new Booking
            {
                StartTime = new DateTime(2026, 1, 1, 10, 0, 0),
                CourtNumber = 1
            };

            //act
            var result = await service.CreateBookingAsync(booking);

            //assert
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

        [TestMethod]
        public async Task CreateBooing_ShouldReturnFalse_WhenCourtNumberIsInvalid()
        {
            //arrange
            //skapar en mock av repository så vi slipper använda riktig databas
            var mock = new Mock<IBookingRepository>();

            //ställer in mocken så den inte hittar dubbelbokning
            mock.Setup(x => x.BookingExistsAsync(0, new DateTime(2026, 1, 1, 10, 0, 0)))
            .ReturnsAsync(false);

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


    }
}
