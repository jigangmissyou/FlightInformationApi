using FlightInformationApi.DTOs;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Xunit;

namespace FlightInformationApi.Tests.DTOs;

public class ApiValidationErrorResponseTests
{
    [Fact]
    public void FromModelState_WithSingleFieldError_ReturnsCorrectResponse()
    {
        var modelState = new ModelStateDictionary();
        modelState.AddModelError("FlightNumber", "Flight number is required");

        var response = ApiValidationErrorResponse.FromModelState(modelState);

        Assert.False(response.Success);
        Assert.Equal("Validation failed", response.Message);
        Assert.Single(response.Errors);
        Assert.True(response.Errors.ContainsKey("FlightNumber"));
        Assert.Single(response.Errors["FlightNumber"]);
        Assert.Equal("Flight number is required", response.Errors["FlightNumber"][0]);
    }

    [Fact]
    public void FromModelState_WithMultipleFieldErrors_ReturnsAllErrors()
    {
        var modelState = new ModelStateDictionary();
        modelState.AddModelError("FlightNumber", "Flight number is required");
        modelState.AddModelError("Airline", "Airline is required");
        modelState.AddModelError("FlightNumber", "Flight number must be valid");

        var response = ApiValidationErrorResponse.FromModelState(modelState);

        Assert.False(response.Success);
        Assert.Equal(2, response.Errors.Count);
        Assert.Equal(2, response.Errors["FlightNumber"].Count);
        Assert.Single(response.Errors["Airline"]);
    }



    [Fact]
    public void FromModelState_WithEmptyErrorMessage_UsesFallbackMessage()
    {
        var modelState = new ModelStateDictionary();
        modelState.AddModelError("Field", "");

        var response = ApiValidationErrorResponse.FromModelState(modelState);

        Assert.Single(response.Errors);
        Assert.Equal("Invalid value", response.Errors["Field"][0]);
    }

    [Fact]
    public void FromModelState_WithEmptyModelState_ReturnsEmptyErrors()
    {
        var modelState = new ModelStateDictionary();

        var response = ApiValidationErrorResponse.FromModelState(modelState);

        Assert.False(response.Success);
        Assert.Equal("Validation failed", response.Message);
        Assert.Empty(response.Errors);
    }

    [Fact]
    public void FromModelState_WithValidState_ReturnsEmptyErrors()
    {
        var modelState = new ModelStateDictionary();
        modelState.SetModelValue("ValidField", new ValueProviderResult("value"));

        var response = ApiValidationErrorResponse.FromModelState(modelState);

        Assert.Empty(response.Errors);
    }
}
