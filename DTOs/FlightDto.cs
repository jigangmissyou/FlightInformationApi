using FlightInformationApi.Models;

namespace FlightInformationApi.DTOs;

/// <summary>
/// Represents a flight with all its details.
/// </summary>
public class FlightDto
{
    /// <summary>
    /// The unique identifier of the flight.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// The airline operating the flight.
    /// </summary>
    public string Airline { get; set; } = string.Empty;

    /// <summary>
    /// The flight number.
    /// </summary>
    public string FlightNumber { get; set; } = string.Empty;

    /// <summary>
    /// The departure airport code.
    /// </summary>
    public string DepartureAirport { get; set; } = string.Empty;

    /// <summary>
    /// The arrival airport code.
    /// </summary>
    public string ArrivalAirport { get; set; } = string.Empty;

    /// <summary>
    /// The scheduled departure time.
    /// </summary>
    public DateTime DepartureTime { get; set; }

    /// <summary>
    /// The scheduled arrival time.
    /// </summary>
    public DateTime ArrivalTime { get; set; }

    /// <summary>
    /// The current status of the flight.
    /// </summary>
    public string Status { get; set; } = string.Empty;
}
