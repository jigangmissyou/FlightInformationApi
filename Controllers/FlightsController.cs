using FlightInformationApi.DTOs;
using FlightInformationApi.Models;
using FlightInformationApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace FlightInformationApi.Controllers;

/// <summary>
/// Manages flight information operations.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class FlightsController : ControllerBase
{
    private readonly IFlightService _flightService;

    /// <summary>
    /// Initializes a new instance of the <see cref="FlightsController"/> class.
    /// </summary>
    /// <param name="flightService">The flight service.</param>
    public FlightsController(IFlightService flightService)
    {
        _flightService = flightService;
    }

    /// <summary>
    /// Gets all flights.
    /// </summary>
    /// <returns>A list of flights.</returns>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<FlightDto>>> GetAll()
    {
        return Ok(await _flightService.GetAllAsync());
    }

    /// <summary>
    /// Gets a flight by its ID.
    /// </summary>
    /// <param name="id">The flight ID.</param>
    /// <returns>The flight with the specified ID.</returns>
    [HttpGet("{id}")]
    public async Task<ActionResult<FlightDto>> GetById(int id)
    {
        var flight = await _flightService.GetByIdAsync(id);
        if (flight == null)
        {
            return NotFound();
        }
        return Ok(flight);
    }

    /// <summary>
    /// Creates a new flight.
    /// </summary>
    /// <param name="flightDto">The flight creation data.</param>
    /// <returns>The created flight.</returns>
    [HttpPost]
    public async Task<ActionResult<FlightDto>> Create(CreateFlightDto flightDto)
    {
        var createdFlight = await _flightService.CreateAsync(flightDto);
        return CreatedAtAction(nameof(GetById), new { id = createdFlight.Id }, createdFlight);
    }

    /// <summary>
    /// Updates an existing flight.
    /// </summary>
    /// <param name="id">The flight ID.</param>
    /// <param name="flightDto">The flight update data.</param>
    /// <returns>No content.</returns>
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, UpdateFlightDto flightDto)
    {
        var existingFlight = await _flightService.GetByIdAsync(id);
        if (existingFlight == null)
        {
            return NotFound();
        }

        await _flightService.UpdateAsync(id, flightDto);
        return NoContent();
    }

    /// <summary>
    /// Deletes a flight.
    /// </summary>
    /// <param name="id">The flight ID.</param>
    /// <returns>No content.</returns>
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var existingFlight = await _flightService.GetByIdAsync(id);
        if (existingFlight == null)
        {
            return NotFound();
        }

        await _flightService.DeleteAsync(id);
        return NoContent();
    }

    /// <summary>
    /// Searches for flights based on criteria.
    /// </summary>
    /// <param name="airline">The airline name (optional).</param>
    /// <param name="departureAirport">The departure airport (optional).</param>
    /// <param name="arrivalAirport">The arrival airport (optional).</param>
    /// <param name="date">The flight date (optional).</param>
    /// <returns>A list of matching flights.</returns>
    [HttpGet("search")]
    public async Task<ActionResult<IEnumerable<FlightDto>>> Search([FromQuery] string? airline, [FromQuery] string? departureAirport, [FromQuery] string? arrivalAirport, [FromQuery] DateTime? date)
    {
        return Ok(await _flightService.SearchAsync(airline, departureAirport, arrivalAirport, date));
    }
}
