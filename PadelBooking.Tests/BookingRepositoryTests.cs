using Microsoft.EntityFrameworkCore;
using PadelBooking.Core.Data;
using PadelBooking.Core.Models;
using PadelBooking.Core.Repositories;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Security.Cryptography.Xml;

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

        var booking = new Booking
        {
            CourtNumber = 1,
            StartTime = new DateTime(2026, 1, 1, 10, 0, 0),
            CustomerId = 1
        };

        //act
        await repository.AddAsync(booking);

        //assert
        using var assertContext = new AppDbContext(_options);

        //kollar att databasen fick data
        var result = await assertContext.Bookings.CountAsync();

        Assert.AreEqual(1, result);
    }

    [TestMethod]
    public async Task DeleteAsync_ShouldRemoveBookingFromDatabase()
    {
        //arrange
        using var context = new AppDbContext(_options);
        var repository = new BookingRepository(context);

        var booking = new Booking
        {
            CourtNumber = 1,
            StartTime = new DateTime(2026, 1, 1, 10, 0, 0),
            CustomerId = 1
        };

        await repository.AddAsync(booking);

        //act
        await repository.DeleteAsync(booking.Id);

        //assert
        using var assertContext = new AppDbContext(_options);

        var count = await assertContext.Bookings.CountAsync();

        Assert.AreEqual(0, count);

    }

    [TestMethod]
    public async Task BookingExistsAsync_ShouldReturnTrue_WhenBookingExists()
    {
        using var context = new AppDbContext(_options);
        var repository = new BookingRepository(context);

        await context.Bookings.AddAsync(new Booking
        {
            CourtNumber = 1,
            StartTime = new DateTime(2026, 1, 1, 10, 0, 0)
        });

        await context.SaveChangesAsync();

        var exists = await repository.BookingExistsAsync(
            1,
            new DateTime(2026, 1, 1, 10, 0, 0)
        );

        Assert.IsTrue(exists);
    }
}
