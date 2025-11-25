using FlightInformationApi.DTOs;
using FlightInformationApi.Models;

namespace FlightInformationApi.Services;

public interface IFlightService
{
    Task<IEnumerable<FlightDto>> GetAllAsync();
    Task<FlightDto?> GetByIdAsync(int id);
    Task<FlightDto> CreateAsync(CreateFlightDto flightDto);
    Task UpdateAsync(int id, UpdateFlightDto flightDto);
    Task DeleteAsync(int id);
    Task<IEnumerable<FlightDto>> SearchAsync(string? airline, string? departureAirport, string? arrivalAirport, DateTime? date, DateTime? startDate, DateTime? endDate);
}
