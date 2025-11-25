using System.ComponentModel.DataAnnotations;
using FlightInformationApi.Models;

namespace FlightInformationApi.DTOs;

/// <summary>
/// Data transfer object for updating an existing flight.
/// </summary>
public class UpdateFlightDto
{
    /// <summary>
    /// The airline operating the flight.
    /// </summary>
    public string? Airline { get; set; }

    /// <summary>
    /// The flight number.
    /// </summary>
    public string? FlightNumber { get; set; }

    /// <summary>
    /// The departure airport code.
    /// </summary>
    public string? DepartureAirport { get; set; }

    /// <summary>
    /// The arrival airport code.
    /// </summary>
    public string? ArrivalAirport { get; set; }

    /// <summary>
    /// The scheduled departure time.
    /// </summary>
    public DateTime? DepartureTime { get; set; }

    /// <summary>
    /// The scheduled arrival time.
    /// </summary>
    public DateTime? ArrivalTime { get; set; }

    /// <summary>
    /// The current status of the flight.
    /// </summary>
    public FlightStatus? Status { get; set; }
}
