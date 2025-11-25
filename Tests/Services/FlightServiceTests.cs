using FlightInformationApi.Data;
using FlightInformationApi.DTOs;
using FlightInformationApi.Models;
using FlightInformationApi.Services;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace FlightInformationApi.Tests.Services;

public class FlightServiceTests
{
    private FlightDbContext GetDbContext()
    {
        var options = new DbContextOptionsBuilder<FlightDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        return new FlightDbContext(options);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsAllFlights()
    {
        // Arrange
        using var context = GetDbContext();
        context.Flights.Add(new Flight
        {
            Id = 1,
            FlightNumber = "AI101",
            Airline = "Air New Zealand",
            DepartureAirport = "MEL",
            ArrivalAirport = "SYD",
            DepartureTime = DateTime.Now,
            ArrivalTime = DateTime.Now.AddHours(5),
            Status = FlightStatus.Scheduled
        });
        context.Flights.Add(new Flight
        {
            Id = 2,
            FlightNumber = "VI102",
            Airline = "Virgin Australia",
            DepartureAirport = "MEL",
            ArrivalAirport = "SYD",
            DepartureTime = DateTime.Now,
            ArrivalTime = DateTime.Now.AddHours(4),
            Status = FlightStatus.Delayed
        });
        await context.SaveChangesAsync();

        var service = new FlightService(context);

        // Act
        var result = await service.GetAllAsync();

        // Assert
        Assert.Equal(2, result.Count());
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsFlight_WhenExists()
    {
        // Arrange
        using var context = GetDbContext();
        context.Flights.Add(new Flight
        {
            Id = 1,
            FlightNumber = "QA103",
            Airline = "Qantas",
            DepartureAirport = "MEL",
            ArrivalAirport = "SYD",
            DepartureTime = DateTime.Now,
            ArrivalTime = DateTime.Now.AddHours(5),
            Status = FlightStatus.Scheduled
        });
        await context.SaveChangesAsync();

        var service = new FlightService(context);

        // Act
        var result = await service.GetByIdAsync(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal("QA103", result.FlightNumber);
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsNull_WhenNotExists()
    {
        // Arrange
        using var context = GetDbContext();
        var service = new FlightService(context);

        // Act
        var result = await service.GetByIdAsync(999);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task CreateAsync_AddsFlightToDatabase()
    {
        // Arrange
        using var context = GetDbContext();
        var service = new FlightService(context);
        var createDto = new CreateFlightDto
        {
            FlightNumber = "JE104",
            Airline = "Jetstar",
            DepartureAirport = "MEL",
            ArrivalAirport = "SYD",
            DepartureTime = DateTime.Now,
            ArrivalTime = DateTime.Now.AddHours(5),
            Status = FlightStatus.Scheduled,

        };

        // Act
        var result = await service.CreateAsync(createDto);

        // Assert
        Assert.NotNull(result);
        var flightInDb = await context.Flights.FindAsync(result.Id);
        Assert.NotNull(flightInDb);
        Assert.Equal("JE104", flightInDb.FlightNumber);
    }

    [Fact]
    public async Task UpdateAsync_UpdatesFlight_WhenExists()
    {
        // Arrange
        using var context = GetDbContext();
        context.Flights.Add(new Flight
        {
            Id = 1,
            FlightNumber = "QA105",
            Airline = "Qantas",
            DepartureAirport = "MEL",
            ArrivalAirport = "SYD",
            DepartureTime = DateTime.Now,
            ArrivalTime = DateTime.Now.AddHours(5),
            Status = FlightStatus.Cancelled
        });
        await context.SaveChangesAsync();

        var service = new FlightService(context);
        var updateDto = new UpdateFlightDto { FlightNumber = "QA105-Updated" };

        // Act
        await service.UpdateAsync(1, updateDto);

        // Assert
        var flightInDb = await context.Flights.FindAsync(1);
        Assert.Equal("QA105-Updated", flightInDb!.FlightNumber);
    }

    [Fact]
    public async Task DeleteAsync_RemovesFlight_WhenExists()
    {
        // Arrange
        using var context = GetDbContext();
        context.Flights.Add(new Flight
        {
            Id = 1,
            FlightNumber = "JE106",
            Airline = "Jetstar",
            DepartureAirport = "MEL",
            ArrivalAirport = "SYD",
            DepartureTime = DateTime.Now,
            ArrivalTime = DateTime.Now.AddHours(5),
            Status = FlightStatus.Cancelled
        });
        await context.SaveChangesAsync();

        var service = new FlightService(context);

        // Act
        await service.DeleteAsync(1);

        // Assert
        var flightInDb = await context.Flights.FindAsync(1);
        Assert.Null(flightInDb);
    }

    [Fact]
    public async Task SearchAsync_FiltersCorrectly_ByAirline()
    {
        // Arrange
        using var context = GetDbContext();
        context.Flights.Add(new Flight
        {
            Id = 1,
            FlightNumber = "JE104",
            Airline = "Jetstar",
            DepartureAirport = "ZQN",
            ArrivalAirport = "DXB",
            DepartureTime = DateTime.Now,
            ArrivalTime = DateTime.Now.AddHours(5),
            Status = FlightStatus.InAir
        });
        context.Flights.Add(new Flight
        {
            Id = 2,
            FlightNumber = "AI108",
            Airline = "Air New Zealand",
            DepartureAirport = "CHC",
            ArrivalAirport = "MEL",
            DepartureTime = DateTime.Now,
            ArrivalTime = DateTime.Now.AddHours(12),
            Status = FlightStatus.Delayed
        });
        context.Flights.Add(new Flight
        {
            Id = 3,
            FlightNumber = "FI115",
            Airline = "Fiji Airways",
            DepartureAirport = "DUD",
            ArrivalAirport = "SYD",
            DepartureTime = DateTime.Now,
            ArrivalTime = DateTime.Now.AddHours(8),
            Status = FlightStatus.Scheduled
        });
        await context.SaveChangesAsync();

        var service = new FlightService(context);

        // Act
        var result = await service.SearchAsync("Fiji", null, null, null, null, null);

        // Assert
        Assert.Single(result);
        Assert.Equal("Fiji Airways", result.First().Airline);
    }

    [Fact]
    public async Task SearchAsync_FiltersCorrectly_ByDepartureAirport()
    {
        // Arrange
        using var context = GetDbContext();
        context.Flights.Add(new Flight
        {
            Id = 1,
            FlightNumber = "JE104",
            Airline = "Jetstar",
            DepartureAirport = "ZQN",
            ArrivalAirport = "DXB",
            DepartureTime = DateTime.Now,
            ArrivalTime = DateTime.Now.AddHours(5),
            Status = FlightStatus.Scheduled
        });
        context.Flights.Add(new Flight
        {
            Id = 2,
            FlightNumber = "VI110",
            Airline = "Virgin Australia",
            DepartureAirport = "MEL",
            ArrivalAirport = "SYD",
            DepartureTime = DateTime.Now,
            ArrivalTime = DateTime.Now.AddHours(2),
            Status = FlightStatus.Delayed
        });
        await context.SaveChangesAsync();

        var service = new FlightService(context);

        // Act
        var result = await service.SearchAsync(null, "MEL", null, null, null, null);

        // Assert
        Assert.Single(result);
        Assert.Equal("MEL", result.First().DepartureAirport);
    }

    [Fact]
    public async Task SearchAsync_FiltersCorrectly_ByArrivalAirport()
    {
        // Arrange
        using var context = GetDbContext();
        context.Flights.Add(new Flight
        {
            Id = 1,
            FlightNumber = "JE113",
            Airline = "Jetstar",
            DepartureAirport = "ZQN",
            ArrivalAirport = "DXB",
            DepartureTime = DateTime.Now,
            ArrivalTime = DateTime.Now.AddHours(5),
            Status = FlightStatus.Scheduled
        });
        context.Flights.Add(new Flight
        {
            Id = 2,
            FlightNumber = "EM117",
            Airline = "Emirates",
            DepartureAirport = "MEL",
            ArrivalAirport = "SYD",
            DepartureTime = DateTime.Now,
            ArrivalTime = DateTime.Now.AddHours(4),
            Status = FlightStatus.Delayed
        });
        await context.SaveChangesAsync();

        var service = new FlightService(context);

        // Act
        var result = await service.SearchAsync(null, null, "SYD", null, null, null);

        // Assert
        Assert.Single(result);
        Assert.Equal("SYD", result.First().ArrivalAirport);
    }

    [Fact]
    public async Task SearchAsync_FiltersCorrectly_ByDate()
    {
        // Arrange
        using var context = GetDbContext();
        var today = DateTime.Today;
        var tomorrow = DateTime.Today.AddDays(1);

        context.Flights.Add(new Flight
        {
            Id = 1,
            FlightNumber = "VI118",
            Airline = "Virgin Australia",
            DepartureAirport = "MEL",
            ArrivalAirport = "SYD",
            DepartureTime = today.AddHours(10),
            ArrivalTime = today.AddHours(15),
            Status = FlightStatus.Scheduled
        });
        context.Flights.Add(new Flight
        {
            Id = 2,
            FlightNumber = "AI119",
            Airline = "Air New Zealand",
            DepartureAirport = "MEL",
            ArrivalAirport = "SYD",
            DepartureTime = tomorrow.AddHours(8),
            ArrivalTime = tomorrow.AddHours(12),
            Status = FlightStatus.Scheduled
        });
        await context.SaveChangesAsync();

        var service = new FlightService(context);

        // Act
        var result = await service.SearchAsync(null, null, null, today, null, null);

        // Assert
        Assert.Single(result);
        Assert.Equal("VI118", result.First().FlightNumber);
    }

    [Fact]
    public async Task SearchAsync_FiltersCorrectly_WithMultipleCriteria()
    {
        // Arrange
        using var context = GetDbContext();
        var today = DateTime.Today;

        context.Flights.Add(new Flight
        {
            Id = 1,
            FlightNumber = "FI120",
            Airline = "Fiji Airways",
            DepartureAirport = "DUD",
            ArrivalAirport = "SYD",
            DepartureTime = today.AddHours(10),
            ArrivalTime = today.AddHours(15),
            Status = FlightStatus.Scheduled
        });
        context.Flights.Add(new Flight
        {
            Id = 2,
            FlightNumber = "JE121",
            Airline = "Jetstar",
            DepartureAirport = "ZQN",
            ArrivalAirport = "DXB",
            DepartureTime = today.AddHours(12),
            ArrivalTime = today.AddHours(16),
            Status = FlightStatus.Scheduled
        });
        context.Flights.Add(new Flight
        {
            Id = 3,
            FlightNumber = "VI122",
            Airline = "Virgin Australia",
            DepartureAirport = "MEL",
            ArrivalAirport = "SYD",
            DepartureTime = today.AddHours(9),
            ArrivalTime = today.AddHours(13),
            Status = FlightStatus.Delayed
        });
        await context.SaveChangesAsync();

        var service = new FlightService(context);

        var result = await service.SearchAsync("Jetstar", "ZQN", "DXB", today, null, null);

        Assert.Single(result);
        Assert.Equal("JE121", result.First().FlightNumber);
        Assert.Equal("Jetstar", result.First().Airline);
    }

    [Fact]
    public async Task SearchAsync_ReturnsEmpty_WhenNoMatches()
    {
        // Arrange
        using var context = GetDbContext();
        context.Flights.Add(new Flight
        {
            Id = 1,
            FlightNumber = "AI123",
            Airline = "Air New Zealand",
            DepartureAirport = "DUD",
            ArrivalAirport = "SYD",
            DepartureTime = DateTime.Now,
            ArrivalTime = DateTime.Now.AddHours(5),
            Status = FlightStatus.Scheduled
        });
        await context.SaveChangesAsync();

        var service = new FlightService(context);

        var result = await service.SearchAsync("NonExistentAirline", null, null, null, null, null);

        Assert.Empty(result);
    }

    [Fact]
    public async Task UpdateAsync_UpdatesOnlySpecifiedFields()
    {
        // Arrange
        using var context = GetDbContext();
        var originalDepartureTime = DateTime.Now;
        var originalArrivalTime = DateTime.Now.AddHours(5);

        context.Flights.Add(new Flight
        {
            Id = 1,
            FlightNumber = "EM124",
            Airline = "Emirates",
            DepartureAirport = "MEL",
            ArrivalAirport = "SYD",
            DepartureTime = originalDepartureTime,
            ArrivalTime = originalArrivalTime,
            Status = FlightStatus.Scheduled
        });
        await context.SaveChangesAsync();

        var service = new FlightService(context);
        var updateDto = new UpdateFlightDto { FlightNumber = "EM124-Updated" };

        await service.UpdateAsync(1, updateDto);

        var flightInDb = await context.Flights.FindAsync(1);
        Assert.Equal("EM124-Updated", flightInDb!.FlightNumber);
        Assert.Equal("Emirates", flightInDb.Airline);
        Assert.Equal("MEL", flightInDb.DepartureAirport);
        Assert.Equal("SYD", flightInDb.ArrivalAirport);
        Assert.Equal(FlightStatus.Scheduled, flightInDb.Status);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsEmpty_WhenDatabaseEmpty()
    {
        using var context = GetDbContext();
        var service = new FlightService(context);

        var result = await service.GetAllAsync();

        Assert.Empty(result);
    }

    [Fact]
    public async Task SearchAsync_FiltersCorrectly_ByDateRange()
    {
        // Arrange
        using var context = GetDbContext();
        var today = DateTime.Today;
        var tomorrow = today.AddDays(1);
        var dayAfterTomorrow = today.AddDays(2);
        var threeDaysLater = today.AddDays(3);

        context.Flights.Add(new Flight
        {
            Id = 1,
            FlightNumber = "AI130",
            Airline = "Air New Zealand",
            DepartureAirport = "MEL",
            ArrivalAirport = "SYD",
            DepartureTime = today.AddHours(10),
            ArrivalTime = today.AddHours(12),
            Status = FlightStatus.Scheduled
        });
        context.Flights.Add(new Flight
        {
            Id = 2,
            FlightNumber = "VI131",
            Airline = "Virgin Australia",
            DepartureAirport = "MEL",
            ArrivalAirport = "SYD",
            DepartureTime = tomorrow.AddHours(14),
            ArrivalTime = tomorrow.AddHours(16),
            Status = FlightStatus.Scheduled
        });
        context.Flights.Add(new Flight
        {
            Id = 3,
            FlightNumber = "JE132",
            Airline = "Jetstar",
            DepartureAirport = "MEL",
            ArrivalAirport = "SYD",
            DepartureTime = dayAfterTomorrow.AddHours(8),
            ArrivalTime = dayAfterTomorrow.AddHours(10),
            Status = FlightStatus.Scheduled
        });
        context.Flights.Add(new Flight
        {
            Id = 4,
            FlightNumber = "QA133",
            Airline = "Qantas",
            DepartureAirport = "MEL",
            ArrivalAirport = "SYD",
            DepartureTime = threeDaysLater.AddHours(15),
            ArrivalTime = threeDaysLater.AddHours(17),
            Status = FlightStatus.Scheduled
        });
        await context.SaveChangesAsync();

        var service = new FlightService(context);

        // Act - Search for flights from today to day after tomorrow (3 days)
        var result = await service.SearchAsync(null, null, null, null, today, dayAfterTomorrow);

        // Assert - Should return 3 flights (today, tomorrow, day after tomorrow)
        Assert.Equal(3, result.Count());
        Assert.Contains(result, f => f.FlightNumber == "AI130");
        Assert.Contains(result, f => f.FlightNumber == "VI131");
        Assert.Contains(result, f => f.FlightNumber == "JE132");
        Assert.DoesNotContain(result, f => f.FlightNumber == "QA133");
    }

    [Fact]
    public async Task SearchAsync_DateRangeWithSingleDay_ReturnsSameDayFlights()
    {
        // Arrange
        using var context = GetDbContext();
        var today = DateTime.Today;

        context.Flights.Add(new Flight
        {
            Id = 1,
            FlightNumber = "EM134",
            Airline = "Emirates",
            DepartureAirport = "MEL",
            ArrivalAirport = "DXB",
            DepartureTime = today.AddHours(10),
            ArrivalTime = today.AddHours(20),
            Status = FlightStatus.Scheduled
        });
        await context.SaveChangesAsync();

        var service = new FlightService(context);

        // Act - Search with same start and end date
        var result = await service.SearchAsync(null, null, null, null, today, today);

        // Assert
        Assert.Single(result);
        Assert.Equal("EM134", result.First().FlightNumber);
    }

    [Fact]
    public async Task SearchAsync_DateRangeWithMultipleCriteria_FiltersCorrectly()
    {
        // Arrange
        using var context = GetDbContext();
        var today = DateTime.Today;
        var tomorrow = today.AddDays(1);

        context.Flights.Add(new Flight
        {
            Id = 1,
            FlightNumber = "JE135",
            Airline = "Jetstar",
            DepartureAirport = "ZQN",
            ArrivalAirport = "DXB",
            DepartureTime = today.AddHours(10),
            ArrivalTime = today.AddHours(20),
            Status = FlightStatus.Scheduled
        });
        context.Flights.Add(new Flight
        {
            Id = 2,
            FlightNumber = "JE136",
            Airline = "Jetstar",
            DepartureAirport = "ZQN",
            ArrivalAirport = "DXB",
            DepartureTime = tomorrow.AddHours(10),
            ArrivalTime = tomorrow.AddHours(20),
            Status = FlightStatus.Scheduled
        });
        context.Flights.Add(new Flight
        {
            Id = 3,
            FlightNumber = "AI137",
            Airline = "Air New Zealand",
            DepartureAirport = "ZQN",
            ArrivalAirport = "DXB",
            DepartureTime = today.AddHours(12),
            ArrivalTime = today.AddHours(22),
            Status = FlightStatus.Scheduled
        });
        await context.SaveChangesAsync();

        var service = new FlightService(context);

        // Act - Search for Jetstar flights from ZQN to DXB within date range
        var result = await service.SearchAsync("Jetstar", "ZQN", "DXB", null, today, tomorrow);

        // Assert - Should return only Jetstar flights
        Assert.Equal(2, result.Count());
        Assert.All(result, f => Assert.Equal("Jetstar", f.Airline));
        Assert.Contains(result, f => f.FlightNumber == "JE135");
        Assert.Contains(result, f => f.FlightNumber == "JE136");
    }
}

