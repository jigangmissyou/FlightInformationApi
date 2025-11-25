using FlightInformationApi.Controllers;
using FlightInformationApi.DTOs;
using FlightInformationApi.Services;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace FlightInformationApi.Tests.Controllers
{
    public class FlightsControllerTests
    {
        private readonly Mock<IFlightService> _mockFlightService;
        private readonly FlightsController _controller;

        public FlightsControllerTests()
        {
            _mockFlightService = new Mock<IFlightService>();
            _controller = new FlightsController(_mockFlightService.Object);
        }

        [Fact]
        public async Task GetAll_ReturnsOkResult_WithListOfFlights()
        {
            // Arrange
            var flights = new List<FlightDto>
            {
                new FlightDto { Id = 1, FlightNumber = "AI101", Airline = "Air New Zealand" }
            };
            _mockFlightService.Setup(s => s.GetAllAsync()).ReturnsAsync(flights);

            // Act
            var result = await _controller.GetAll();

            // Assert
            var ok = Assert.IsType<OkObjectResult>(result.Result);
            var response = Assert.IsType<ApiResponse<IEnumerable<FlightDto>>>(ok.Value);
            Assert.True(response.Success);
            Assert.Single(response.Data);
        }

        [Fact]
        public async Task GetById_ReturnsOkResult_WhenFlightExists()
        {
            var flight = new FlightDto { Id = 2, FlightNumber = "VI102" };
            _mockFlightService.Setup(s => s.GetByIdAsync(2)).ReturnsAsync(flight);

            var result = await _controller.GetById(2);

            var ok = Assert.IsType<OkObjectResult>(result.Result);
            var response = Assert.IsType<ApiSingleResponse<FlightDto>>(ok.Value);
            Assert.True(response.Success);
            Assert.NotNull(response.Data);
            Assert.Equal(2, response.Data.Id);
        }

        [Fact]
        public async Task GetById_ReturnsNotFound_WhenFlightDoesNotExist()
        {
            _mockFlightService.Setup(s => s.GetByIdAsync(99)).ReturnsAsync((FlightDto?)null);

            var result = await _controller.GetById(99);

            var notFound = Assert.IsType<NotFoundObjectResult>(result.Result);
            var response = Assert.IsType<ApiSingleResponse<FlightDto>>(notFound.Value);
            Assert.False(response.Success);
            Assert.Equal("Flight not found", response.Message);
        }

        [Fact]
        public async Task Create_ReturnsCreatedAtAction_WithCreatedFlight()
        {
            var createDto = new CreateFlightDto { FlightNumber = "QA103" };
            var created = new FlightDto { Id = 3, FlightNumber = "QA103" };
            _mockFlightService.Setup(s => s.CreateAsync(createDto)).ReturnsAsync(created);

            var result = await _controller.Create(createDto);

            var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
            var response = Assert.IsType<ApiSingleResponse<FlightDto>>(createdResult.Value);
            Assert.True(response.Success);
            Assert.Equal("Flight created successfully", response.Message);
            Assert.NotNull(response.Data);
            Assert.Equal(3, response.Data.Id);
        }

        [Fact]
        public async Task Update_ReturnsNoContent_WhenFlightExists()
        {
            var updateDto = new UpdateFlightDto { FlightNumber = "JE104" };
            _mockFlightService.Setup(s => s.GetByIdAsync(4)).ReturnsAsync(new FlightDto());
            _mockFlightService.Setup(s => s.UpdateAsync(4, updateDto)).Returns(Task.CompletedTask);

            var result = await _controller.Update(4, updateDto);

            var ok = Assert.IsType<OkObjectResult>(result.Result);
            var response = Assert.IsType<ApiSingleResponse<object>>(ok.Value);
            Assert.True(response.Success);
            Assert.Equal("Flight updated successfully", response.Message);
        }

        [Fact]
        public async Task Update_ReturnsNotFound_WhenFlightDoesNotExist()
        {
            var updateDto = new UpdateFlightDto { FlightNumber = "JE104" };
            _mockFlightService.Setup(s => s.GetByIdAsync(404)).ReturnsAsync((FlightDto?)null);

            var result = await _controller.Update(404, updateDto);

            var notFound = Assert.IsType<NotFoundObjectResult>(result.Result);
            var response = Assert.IsType<ApiSingleResponse<object>>(notFound.Value);
            Assert.False(response.Success);
            Assert.Equal("Flight not found", response.Message);
        }

        [Fact]
        public async Task Delete_ReturnsNoContent_WhenFlightExists()
        {
            _mockFlightService.Setup(s => s.GetByIdAsync(5)).ReturnsAsync(new FlightDto());
            _mockFlightService.Setup(s => s.DeleteAsync(5)).Returns(Task.CompletedTask);

            var result = await _controller.Delete(5);

            var ok = Assert.IsType<OkObjectResult>(result.Result);
            var response = Assert.IsType<ApiSingleResponse<object>>(ok.Value);
            Assert.True(response.Success);
            Assert.Equal("Flight deleted successfully", response.Message);
        }

        [Fact]
        public async Task Delete_ReturnsNotFound_WhenFlightDoesNotExist()
        {
            _mockFlightService.Setup(s => s.GetByIdAsync(888)).ReturnsAsync((FlightDto?)null);

            var result = await _controller.Delete(888);

            var notFound = Assert.IsType<NotFoundObjectResult>(result.Result);
            var response = Assert.IsType<ApiSingleResponse<object>>(notFound.Value);
            Assert.False(response.Success);
            Assert.Equal("Flight not found", response.Message);
        }

        [Fact]
        public async Task Search_ReturnsOkResult_WithFilteredFlights()
        {
            var flights = new List<FlightDto>
            {
                new FlightDto { Id = 8, FlightNumber = "AI108", Airline = "Air New Zealand" }
            };

            _mockFlightService
                .Setup(s => s.SearchAsync("Air New Zealand", null, null, null, null, null))
                .ReturnsAsync(flights);

            var result = await _controller.Search("Air New Zealand", null, null, null, null, null);

            var ok = Assert.IsType<OkObjectResult>(result.Result);
            var response = Assert.IsType<ApiResponse<IEnumerable<FlightDto>>>(ok.Value);
            Assert.True(response.Success);
            Assert.Single(response.Data);
        }

        [Fact]
        public async Task Search_ReturnsOkResult_WithMultipleFilters()
        {
            var flights = new List<FlightDto>
            {
                new FlightDto
                {
                    Id = 10,
                    FlightNumber = "VI110",
                    Airline = "Virgin Australia",
                    DepartureAirport = "NPE",
                    ArrivalAirport = "DXB"
                }
            };

            var date = new DateTime(2025, 6, 20);

            _mockFlightService
                .Setup(s => s.SearchAsync("Virgin Australia", "NPE", "DXB", date, null, null))
                .ReturnsAsync(flights);

            var result = await _controller.Search("Virgin Australia", "NPE", "DXB", date, null, null);

            var ok = Assert.IsType<OkObjectResult>(result.Result);
            var response = Assert.IsType<ApiResponse<IEnumerable<FlightDto>>>(ok.Value);
            Assert.True(response.Success);
            Assert.Single(response.Data);
            Assert.Equal("VI110", response.Data.First().FlightNumber);
            Assert.Equal("NPE", response.Data.First().DepartureAirport);
            Assert.Equal("DXB", response.Data.First().ArrivalAirport);
        }
    }
}

