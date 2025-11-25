using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace FlightInformationApi.DTOs;

/// <summary>
/// API response wrapper for validation errors.
/// </summary>
public class ApiValidationErrorResponse
{
    /// <summary>
    /// Indicates the request failed due to validation errors.
    /// </summary>
    public bool Success { get; set; } = false;

    /// <summary>
    /// Error message describing the validation failure.
    /// </summary>
    public string Message { get; set; } = "Validation failed";

    /// <summary>
    /// Dictionary of field names and their validation error messages.
    /// </summary>
    public Dictionary<string, List<string>> Errors { get; set; } = new();

    /// <summary>
    /// Creates a validation error response from ModelStateDictionary.
    /// </summary>
    public static ApiValidationErrorResponse FromModelState(ModelStateDictionary modelState)
    {
        var response = new ApiValidationErrorResponse();

        foreach (var entry in modelState)
        {
            if (entry.Value?.Errors.Count > 0)
            {
                var errors = entry.Value.Errors
                    .Select(e => string.IsNullOrEmpty(e.ErrorMessage) ? e.Exception?.Message ?? "Invalid value" : e.ErrorMessage)
                    .ToList();

                response.Errors[entry.Key] = errors;
            }
        }

        return response;
    }
}
