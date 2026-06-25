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
        using var context = new AppDbContext(_options);
        //testar riktig EF
        var repository = new BookingRepository(context);
        //skapar test-bokning
        var booking = new Booking
        {
            CourtNumber = 1,
            StartTime = new DateTime(2026, 1, 1, 10, 0, 0),
            CustomerId = 1
        };

        //act
        //lägga till bokning i databasen
        await repository.AddAsync(booking);

        //assert
        //kontrollera att den sparades
        using var assertContext = new AppDbContext(_options);

        //kollar att databasen fick data
        var result = await assertContext.Bookings.CountAsync();

        Assert.AreEqual(1, result);
    }

    [TestMethod]
    public async Task DeleteAsync_ShouldRemoveBookingFromDatabase()
    {
        //arrange
        //skapa context och repository
        using var context = new AppDbContext(_options);
        var repository = new BookingRepository(context);
        //lägger in en bokning som ska tas bort
        var booking = new Booking
        {
            CourtNumber = 1,
            StartTime = new DateTime(2026, 1, 1, 10, 0, 0),
            CustomerId = 1
        };

        await repository.AddAsync(booking);

        //act
        //ta bort bokning
        await repository.DeleteAsync(booking.Id);

        //assert
        //kontrollera att databasen är tom
        using var assertContext = new AppDbContext(_options);

        var count = await assertContext.Bookings.CountAsync();

        Assert.AreEqual(0, count);

    }

    [TestMethod]
    public async Task BookingExistsAsync_ShouldReturnTrue_WhenBookingExists()
    {
        //arrange
        //skapar context och repository
        using var context = new AppDbContext(_options);
        var repository = new BookingRepository(context);
        //lägger in en bokning direkt i databasen
        await context.Bookings.AddAsync(new Booking
        {
            CourtNumber = 1,
            StartTime = new DateTime(2026, 1, 1, 10, 0, 0)
        });

        await context.SaveChangesAsync();
        //act
        //kontrollerar om bokning finns
        var exists = await repository.BookingExistsAsync(
            1,
            new DateTime(2026, 1, 1, 10, 0, 0)
        );

        //assert
        //ska vara true eftersom bokning finns
        Assert.IsTrue(exists);
    }
}
