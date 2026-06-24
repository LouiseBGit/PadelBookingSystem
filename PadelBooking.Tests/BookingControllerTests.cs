
using Moq;
using PadelBooking.Core.Interfaces;
using PadelBooking.Core.Models;
using PadelBooking.API.Controllers;
using Microsoft.AspNetCore.Mvc;
using PadelBooking.Core.DTOs;
using Microsoft.OpenApi.Any;

namespace PadelBooking.Tests
{
    [TestClass]
    public class BookingControllerTests
    {
        /// <summary>
        /// testar att controllern returnerar Ok när bokningen på specifikt id finns
        /// </summary>
        [TestMethod]
        public async Task GetBookingById_ShouldReturnOk_WhenBookingExists()
        {
            //arrange
            //skapar en bokning
            var booking = new BookingDto
            {
                Id = 1,
                CourtNumber = 2,
                StartTime = new DateTime(2026, 1, 1, 10, 0, 0)
            };

            //gör en mock av service
            var mock = new Mock<IBookingService>();

            //gör så att det ser ut som att en bokning finns
            mock.Setup(x => x.GetBookingByIdAsync(1))
                .ReturnsAsync(booking);

            //skapar controllern och skickar in låtsas-servicen
            var controller = new BookingsController(mock.Object);

            //act
            var result = await controller.GetBookingById(1);

            //assert
            Assert.IsInstanceOfType(result.Result, typeof(OkObjectResult));
        }

        [TestMethod]
        public async Task GetBookingById_ShouldReturnNotFound_WhenBookingDoesNotExist()
        {
            //arrange
            var mock = new Mock<IBookingService>();

            //låtsa att bokning inte finns
            mock.Setup(x => x.GetBookingByIdAsync(1))
                .ReturnsAsync((BookingDto?)null);

            var controller = new BookingsController(mock.Object);

            //act
            var result = await controller.GetBookingById(1);


            //assert
            //förväntar att det bli 404 (notfound)
            Assert.IsInstanceOfType(result.Result, typeof(NotFoundResult));
        }

        [TestMethod]
        public async Task CreateBooking_ShouldReturnBadRequest_WhenBookingBreaksRules()
        {
            //arrange
            //skapar mock av service
            var mock = new Mock<IBookingService>();

            mock.Setup(x => x.CreateBookingAsync(It.IsAny<Booking>()))
                .ReturnsAsync(false);

            var controller = new BookingsController(mock.Object);

            //skapar DTO med bokningsdata
            var dto = new CreateBookingDto
            {
                CourtNumber = 1,
                StartTime = new DateTime(2026, 1, 1, 10, 0, 0),
                CustomerId = 1
            };

            //act
            var result = await controller.CreateBooking(dto);

            //assert
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
          
        }

        [TestMethod]
        public async Task CreateBooking_ShouldReturnOk_WhenBookingIsValid()
        {
            //arrange
            //skapar mock av service
            var mock = new Mock<IBookingService>();

            //låtsas att bokning godkänns av service
            mock.Setup(x => x.CreateBookingAsync(It.IsAny<Booking>()))
                .ReturnsAsync(true);

            var controller = new BookingsController(mock.Object);

            //skapar dto med giltig bokning
            var dto = new CreateBookingDto
            {
                CourtNumber = 1,
                StartTime = new DateTime(2026, 1, 1, 10, 0, 0),
                CustomerId = 1
            };

            //act
            var result = await controller.CreateBooking(dto);

            //assert
            //förväntar att controllern returnerar 200 (ok)
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
        }

        [TestMethod]
        public async Task DeleteBooking_ShouldReturnNotFound_WhenBookingDoesNotExist()
        {
            //arrange
            var mock = new Mock<IBookingService>();

            mock.Setup(x => x.GetBookingByIdAsync(1))
                .ReturnsAsync((BookingDto?)null);

            var controller = new BookingsController(mock.Object);

            //act
            var result = await controller.DeleteBooking(1);

            //assert
            Assert.IsInstanceOfType(result, typeof(NotFoundObjectResult));
        }

    }

}

