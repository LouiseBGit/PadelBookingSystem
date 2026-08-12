using Microsoft.EntityFrameworkCore;
using PadelBooking.Core.Data;
using PadelBooking.Core.Models;
using PadelBooking.Core.Repositories;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Security.Cryptography.Xml;
using Microsoft.Extensions.DependencyInjection;

namespace PadelBooking.Tests;

[TestClass]
public class BookingRepositoryTests
{
    private DbContextOptions<AppDbContext>? _options;

    /// <summary>
    /// varje test får en ny, ren, databas
    /// </summary>
    [TestInitialize]
    public void Setup()
    {
        _options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

    }

    [TestMethod]
    public async Task AddAsync_ShouldAddBookingToDatabase()
    {
        //arrange
        //skapar en context som ger tillgång till testdatabasen
        using var context = new AppDbContext(_options);
        
        //skapar repositoryn vi ska testa
        var repository = new BookingRepository(context);
       
        //skapar test-bokning som ska sparas
        var booking = new Booking
        {
            CourtNumber = 1,
            StartTime = new DateTime(2026, 1, 1, 10, 0, 0),
            CustomerId = 1
        };

        //act
        //anropar repository och lägger till bokning i test-databasen
        await repository.AddAsync(booking);

        //assert
        //skapar en ny context mot samma databas för att kontrollera att den sparades
        using var assertContext = new AppDbContext(_options);

        //räknar bokningarna som finns i databasen
        var result = await assertContext.Bookings.CountAsync();

        //det ska finnas exakt en bokning
        Assert.AreEqual(1, result);
    }

    [TestMethod]
    public async Task DeleteAsync_ShouldRemoveBookingFromDatabase()
    {
        //arrange
        //skapa context som ger tillgång till testdatabasen
        using var context = new AppDbContext(_options);

        //skapar repositoryn som ska testas
        var repository = new BookingRepository(context);

        //lägger in en bokning som sen ska tas bort
        var booking = new Booking
        {
            CourtNumber = 1,
            StartTime = new DateTime(2026, 1, 1, 10, 0, 0),
            CustomerId = 1
        };

        //lägger till bokningen som sen ska tas bort 
        await repository.AddAsync(booking);

        //act
        //ta bort bokningen med hjälp av id
        await repository.DeleteAsync(booking.Id);

        //assert
        //skapar ny context mot samma testdatabas för att kontrollera att databasen är tom
        using var assertContext = new AppDbContext(_options);

        //räknar hur många bokningar som finns kvar
        var count = await assertContext.Bookings.CountAsync();

        //det ska finnas noll bokningar kvar
        Assert.AreEqual(0, count);

    }

    [TestMethod]
    public async Task BookingExistsAsync_ShouldReturnTrue_WhenBookingExists()
    {
        //arrange
        //skapar context som ger tillgång till testdatabasen
        using var context = new AppDbContext(_options);

        //skapar repositoryn som vi ska testa
        var repository = new BookingRepository(context);

        //lägger in en bokning direkt i testdatabasen
        await context.Bookings.AddAsync(new Booking
        {
            CourtNumber = 1,
            StartTime = new DateTime(2026, 1, 1, 10, 0, 0)
        });

        //sparar bokningen i testdatabasen
        await context.SaveChangesAsync();

        //act
        //frågar repositoryn om det finns en bokning på bana 1 den 1 jan 10.00
        var exists = await repository.BookingExistsAsync(
            1,
            new DateTime(2026, 1, 1, 10, 0, 0)
        );

        //assert
        //ska vara true eftersom bokning finns
        Assert.IsTrue(exists);
    }
}
