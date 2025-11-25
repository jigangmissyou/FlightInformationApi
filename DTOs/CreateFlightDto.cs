using System.ComponentModel.DataAnnotations;
using FlightInformationApi.Models;

namespace FlightInformationApi.DTOs;

/// <summary>
/// Data transfer object for creating a new flight.
/// </summary>
public class CreateFlightDto
{
    /// <summary>
    /// The flight number.
    /// </summary>
    [Required]
    [StringLength(10)]
    public string FlightNumber { get; set; } = string.Empty;

    /// <summary>
    /// The airline operating the flight.
    /// </summary>
    [Required]
    [StringLength(50)]
    public string Airline { get; set; } = string.Empty;

    /// <summary>
    /// The departure airport code.
    /// </summary>
    [Required]
    [StringLength(3)]
    public string DepartureAirport { get; set; } = string.Empty;

    /// <summary>
    /// The arrival airport code.
    /// </summary>
    [Required]
    [StringLength(3)]
    public string ArrivalAirport { get; set; } = string.Empty;

    /// <summary>
    /// The scheduled departure time.
    /// </summary>
    [Required]
    public DateTime DepartureTime { get; set; }

    /// <summary>
    /// The scheduled arrival time.
    /// </summary>
    [Required]
    public DateTime ArrivalTime { get; set; }

    /// <summary>
    /// The current status of the flight.
    /// </summary>
    [Required]
    public FlightStatus Status { get; set; }
}
