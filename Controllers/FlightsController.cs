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
    /// Gets all flights with pagination.
    /// </summary>
    /// <param name="pageNumber">Page number (default: 1).</param>
    /// <param name="pageSize">Number of items per page (default: 10, max: 100).</param>
    /// <returns>A paginated list of flights.</returns>
    [HttpGet]
    public async Task<ActionResult<PaginatedResponse<FlightDto>>> GetAll([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
    {
        var (data, totalCount) = await _flightService.GetAllAsync(pageNumber, pageSize);
        var response = new PaginatedResponse<FlightDto>(data, totalCount, pageNumber, pageSize);
        return Ok(response);
    }

    /// <summary>
    /// Gets a flight by its ID.
    /// </summary>
    /// <param name="id">The flight ID.</param>
    /// <returns>The flight with the specified ID wrapped in an API response.</returns>
    [HttpGet("{id}")]
    public async Task<ActionResult<ApiSingleResponse<FlightDto>>> GetById(int id)
    {
        var flight = await _flightService.GetByIdAsync(id);
        if (flight == null)
        {
            var errorResponse = new ApiSingleResponse<FlightDto>(null, false, "Flight not found");
            return NotFound(errorResponse);
        }
        var response = new ApiSingleResponse<FlightDto>(flight);
        return Ok(response);
    }

    /// <summary>
    /// Creates a new flight.
    /// </summary>
    /// <param name="flightDto">The flight creation data.</param>
    /// <returns>The created flight wrapped in an API response.</returns>
    [HttpPost]
    public async Task<ActionResult<ApiSingleResponse<FlightDto>>> Create(CreateFlightDto flightDto)
    {
        var createdFlight = await _flightService.CreateAsync(flightDto);
        var response = new ApiSingleResponse<FlightDto>(createdFlight, true, "Flight created successfully");
        return CreatedAtAction(nameof(GetById), new { id = createdFlight.Id }, response);
    }

    /// <summary>
    /// Updates an existing flight.
    /// </summary>
    /// <param name="id">The flight ID.</param>
    /// <param name="flightDto">The flight update data.</param>
    /// <returns>API response indicating success or failure.</returns>
    [HttpPut("{id}")]
    public async Task<ActionResult<ApiSingleResponse<object>>> Update(int id, UpdateFlightDto flightDto)
    {
        var existingFlight = await _flightService.GetByIdAsync(id);
        if (existingFlight == null)
        {
            var errorResponse = new ApiSingleResponse<object>(null, false, "Flight not found");
            return NotFound(errorResponse);
        }

        await _flightService.UpdateAsync(id, flightDto);
        var response = new ApiSingleResponse<object>(null, true, "Flight updated successfully");
        return Ok(response);
    }

    /// <summary>
    /// Deletes a flight.
    /// </summary>
    /// <param name="id">The flight ID.</param>
    /// <returns>API response indicating success or failure.</returns>
    [HttpDelete("{id}")]
    public async Task<ActionResult<ApiSingleResponse<object>>> Delete(int id)
    {
        var existingFlight = await _flightService.GetByIdAsync(id);
        if (existingFlight == null)
        {
            var errorResponse = new ApiSingleResponse<object>(null, false, "Flight not found");
            return NotFound(errorResponse);
        }

        await _flightService.DeleteAsync(id);
        var response = new ApiSingleResponse<object>(null, true, "Flight deleted successfully");
        return Ok(response);
    }

    /// <summary>
    /// Searches for flights based on criteria.
    /// </summary>
    /// <param name="airline">The airline name (optional).</param>
    /// <param name="departureAirport">The departure airport (optional).</param>
    /// <param name="arrivalAirport">The arrival airport (optional).</param>
    /// <param name="date">The flight date (optional, for single date search).</param>
    /// <param name="startDate">The start date for date range search (optional).</param>
    /// <param name="endDate">The end date for date range search (optional).</param>
    /// <returns>A list of matching flights wrapped in an API response.</returns>
    [HttpGet("search")]
    public async Task<ActionResult<ApiResponse<IEnumerable<FlightDto>>>> Search(
        [FromQuery] string? airline,
        [FromQuery] string? departureAirport,
        [FromQuery] string? arrivalAirport,
        [FromQuery] DateTime? date,
        [FromQuery] DateTime? startDate,
        [FromQuery] DateTime? endDate)
    {
        var flights = await _flightService.SearchAsync(airline, departureAirport, arrivalAirport, date, startDate, endDate);
        var response = new ApiResponse<IEnumerable<FlightDto>>(flights);
        return Ok(response);
    }
}
