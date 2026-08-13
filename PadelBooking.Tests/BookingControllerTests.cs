
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
            //skapar en BookingDto som låtsas vara den bokning som service hittar
            var booking = new BookingDto
            {
                Id = 1,
                CourtNumber = 2,
                StartTime = new DateTime(2026, 1, 1, 10, 0, 0)
            };

            //gör en mock av service
            var mock = new Mock<IBookingService>();

            //gör så att det ser ut som att en bokning finns med id 1
            mock.Setup(x => x.GetBookingByIdAsync(1))
                .ReturnsAsync(booking);

            //skapar controllern och skickar in låtsas-servicen
            var controller = new BookingsController(mock.Object);

            //act
            //anropar controller-metoden som ska testas
            var result = await controller.GetBookingById(1);

            //assert
            //kollar att controllern returnerar 200 OK med ett objekt (bokningen)
            //OkObjectResult betyder att svaret är OK och innehåller data
            Assert.IsInstanceOfType(result.Result, typeof(OkObjectResult));
        }

        /// <summary>
        /// testar att controllern returnerar 404
        /// när bokning med det angivna id:t inte finns
        /// </summary>
        [TestMethod]
        public async Task GetBookingById_ShouldReturnNotFound_WhenBookingDoesNotExist()
        {
            //arrange
            //skapar en fejkad service
            var mock = new Mock<IBookingService>();

            //låtsa att service inte hittar bokning med id 1 (null = bokning finns inte)
            mock.Setup(x => x.GetBookingByIdAsync(1))
                .ReturnsAsync((BookingDto?)null);

            //skapar controllern och skickar in den fejkade servicen
            var controller = new BookingsController(mock.Object);

            //act
            //försöker hämta bokningen med id 1
            var result = await controller.GetBookingById(1);


            //assert
            //förväntar att det bli 404 (notfound)
            Assert.IsInstanceOfType(result.Result, typeof(NotFoundResult));
        }

        /// <summary>
        /// testar att controllern returnerar 400 bad request 
        /// när bokning inte godkänns av service
        /// </summary>
        [TestMethod]
        public async Task CreateBooking_ShouldReturnBadRequest_WhenBookingBreaksRules()
        {
            //arrange
            //skapar mock av service
            var mock = new Mock<IBookingService>();

            //låtsas att service inte godkänner bokningen. Null betyder att bokningen inte kunde skapas
            mock.Setup(x => x.CreateBookingAsync(It.IsAny<CreateBookingDto>()))
                .ReturnsAsync((BookingDto?)null);

            //skapar controller och skickar in den fejkae servicen
            var controller = new BookingsController(mock.Object);

            //skapar DTO med bokningsdata som skickas till controllern
            var dto = new CreateBookingDto
            {
                CourtNumber = 1,
                StartTime = new DateTime(2026, 1, 1, 10, 0, 0),
                CustomerId = 1
            };

            //act
            //försöker skapa bokningen genom controllern
            var result = await controller.CreateBooking(dto);

            //assert
            //kollar att controllern returnerar 400 bad request
            //badRequestObjectResult = 400 bad request med info i svaret
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));

        }

        /// <summary>
        /// testar att controllern returnerar 201 Created
        /// när bokning är giltig och skapas
        /// </summary>
        [TestMethod]
        public async Task CreateBooking_ShouldReturnCreated_WhenBookingIsValid()
        {
            //arrange
            //skapar mock av service
            var mock = new Mock<IBookingService>();

            //låtsas att bokning godkänns av service och returnerar den skapade bokningen som BookingDto
            mock.Setup(x => x.CreateBookingAsync(It.IsAny<CreateBookingDto>()))
                .ReturnsAsync(new BookingDto
                {
                    Id = 1,
                    CourtNumber = 1,
                    StartTime = new DateTime(2026, 1, 1, 10, 0, 0),
                    CustomerName = "Test Kund"
                });

            //skapar controllern och skickar in den fejkade servicen
            var controller = new BookingsController(mock.Object);

            //skapar dto med giltig bokning
            var dto = new CreateBookingDto
            {
                CourtNumber = 1,
                StartTime = new DateTime(2026, 1, 1, 10, 0, 0),
                CustomerId = 1
            };

            //act
            //försöker skapa bokningen genom controllern
            var result = await controller.CreateBooking(dto);

            //assert
            //kollar att controllern returnerar 201 Created
            //CreatedAtActionResult betyder att något nytt skapades och att svaret innehåller den skapade bokningen
            Assert.IsInstanceOfType(result, typeof(CreatedAtActionResult));
        }

        /// <summary>
        /// Testar att controllern returnerar 404 Not Found
        /// när man försöker ta bort en bokning som inte finns
        /// </summary>
        [TestMethod]
        public async Task DeleteBooking_ShouldReturnNotFound_WhenBookingDoesNotExist()
        {
            //arrange
            //skapar fejkad service
            var mock = new Mock<IBookingService>();

            //låtsas att service inte kunde ta bort bokningen
            mock.Setup(x => x.DeleteBookingAsync(1))
                .ReturnsAsync(false);

            //skapar controllern och skickar in den fejkade servicen
            var controller = new BookingsController(mock.Object);

            //act
            //försöker ta bort bokningen med id 1
            var result = await controller.DeleteBooking(1);

            //assert
            //kollar att controllern returnerar 404 Not Found
            Assert.IsInstanceOfType(result, typeof(NotFoundObjectResult));
        }

        /// <summary>
        /// testar om bookingController svarar med 404 Not Found, om bokning inte finns
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task UpdateBooking_ShouldReturnNotFound_WhenBookingDoesNotExist()
        {
            //arrange
            var mock = new Mock<IBookingService>();

            //bokningen finns inte
            mock.Setup(x => x.GetBookingByIdAsync(1))
                .ReturnsAsync((BookingDto?)null);

            var controller = new BookingsController(mock.Object);

            var dto = new UpdateBookingDto
            {
                CourtNumber = 1,
                StartTime = new DateTime(2026, 1, 1, 10, 0, 0),
                CustomerId = 1
            };

            //act
            var result = await controller.UpdateBooking(1, dto);

            //assert
            Assert.IsInstanceOfType(result, typeof(NotFoundObjectResult));

        }

    }
}

