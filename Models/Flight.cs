using System.ComponentModel.DataAnnotations;

namespace FlightInformationApi.Models;

public class Flight
{
    public int Id { get; set; }

    [Required]
    [StringLength(10)]
    public string FlightNumber { get; set; } = string.Empty;

    [Required]
    [StringLength(50)]
    public string Airline { get; set; } = string.Empty;

    [Required]
    [StringLength(3)]
    public string DepartureAirport { get; set; } = string.Empty;

    [Required]
    [StringLength(3)]
    public string ArrivalAirport { get; set; } = string.Empty;

    [Required]
    public DateTime DepartureTime { get; set; }

    [Required]
    public DateTime ArrivalTime { get; set; }

    [Required]
    public FlightStatus Status { get; set; }
}
