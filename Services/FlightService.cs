using FlightInformationApi.Data;
using FlightInformationApi.DTOs;
using FlightInformationApi.Models;
using Microsoft.EntityFrameworkCore;

namespace FlightInformationApi.Services;

public class FlightService : IFlightService
{
    private readonly FlightDbContext _context;

    public FlightService(FlightDbContext context)
    {
        _context = context;
    }

    public async Task<(IEnumerable<FlightDto> Data, int TotalCount)> GetAllAsync(int pageNumber = 1, int pageSize = 10)
    {
        // Validate pagination parameters
        if (pageNumber < 1) pageNumber = 1;
        if (pageSize < 1) pageSize = 10;
        if (pageSize > 100) pageSize = 100; // Max page size limit

        // Get total count
        var totalCount = await _context.Flights.CountAsync();

        // Get paginated data
        var flights = await _context.Flights
            .OrderBy(f => f.Id) // Ensure consistent ordering
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var flightDtos = flights.Select(f => MapToDto(f));

        return (flightDtos, totalCount);
    }

    public async Task<FlightDto?> GetByIdAsync(int id)
    {
        var flight = await _context.Flights.FirstOrDefaultAsync(f => f.Id == id);
        return flight == null ? null : MapToDto(flight);
    }

    public async Task<FlightDto> CreateAsync(CreateFlightDto flightDto)
    {
        var flight = new Flight
        {
            FlightNumber = flightDto.FlightNumber,
            Airline = flightDto.Airline,
            DepartureAirport = flightDto.DepartureAirport,
            ArrivalAirport = flightDto.ArrivalAirport,
            DepartureTime = flightDto.DepartureTime,
            ArrivalTime = flightDto.ArrivalTime,
            Status = flightDto.Status
        };

        _context.Flights.Add(flight);
        await _context.SaveChangesAsync();

        return MapToDto(flight);
    }

    public async Task UpdateAsync(int id, UpdateFlightDto flightDto)
    {
        var existingFlight = await _context.Flights.FirstOrDefaultAsync(f => f.Id == id);
        if (existingFlight == null) return;

        if (flightDto.FlightNumber != null) existingFlight.FlightNumber = flightDto.FlightNumber;
        if (flightDto.Airline != null) existingFlight.Airline = flightDto.Airline;
        if (flightDto.DepartureAirport != null) existingFlight.DepartureAirport = flightDto.DepartureAirport;
        if (flightDto.ArrivalAirport != null) existingFlight.ArrivalAirport = flightDto.ArrivalAirport;
        if (flightDto.DepartureTime.HasValue) existingFlight.DepartureTime = flightDto.DepartureTime.Value;
        if (flightDto.ArrivalTime.HasValue) existingFlight.ArrivalTime = flightDto.ArrivalTime.Value;
        if (flightDto.Status.HasValue) existingFlight.Status = flightDto.Status.Value;

        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var flight = await _context.Flights.FirstOrDefaultAsync(f => f.Id == id);
        if (flight != null)
        {
            _context.Flights.Remove(flight);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<IEnumerable<FlightDto>> SearchAsync(string? airline, string? departureAirport, string? arrivalAirport, DateTime? date, DateTime? startDate, DateTime? endDate)
    {
        // Start with a queryable source so conditions can be chained dynamically
        var query = _context.Flights.AsQueryable();

        if (!string.IsNullOrEmpty(airline))
        {
            // Airline name is case-insensitive and partial match
            query = query.Where(f => f.Airline.Contains(airline, StringComparison.OrdinalIgnoreCase));
        }

        if (!string.IsNullOrEmpty(departureAirport))
        {
            query = query.Where(f => f.DepartureAirport.Contains(departureAirport, StringComparison.OrdinalIgnoreCase));
        }

        if (!string.IsNullOrEmpty(arrivalAirport))
        {
            query = query.Where(f => f.ArrivalAirport.Contains(arrivalAirport, StringComparison.OrdinalIgnoreCase));
        }

        // Date filtering: support both single date and date range
        if (startDate.HasValue && endDate.HasValue)
        {
            // Date range search
            query = query.Where(f =>
                f.DepartureTime.Date >= startDate.Value.Date &&
                f.DepartureTime.Date <= endDate.Value.Date
            );
        }
        else if (date.HasValue)
        {
            // Single date search (backward compatibility)
            query = query.Where(f => f.DepartureTime.Date == date.Value.Date);
        }

        var flights = await query.ToListAsync();
        return flights.Select(f => MapToDto(f));
    }

    private static FlightDto MapToDto(Flight flight)
    {
        return new FlightDto
        {
            Id = flight.Id,
            FlightNumber = flight.FlightNumber,
            Airline = flight.Airline,
            DepartureAirport = flight.DepartureAirport,
            ArrivalAirport = flight.ArrivalAirport,
            DepartureTime = flight.DepartureTime,
            ArrivalTime = flight.ArrivalTime,
            Status = flight.Status.ToString()
        };
    }
}
