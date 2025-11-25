namespace FlightInformationApi.DTOs;

/// <summary>
/// Generic API response wrapper for collection results.
/// </summary>
/// <typeparam name="T">The type of data in the response.</typeparam>
public class ApiResponse<T>
{
    /// <summary>
    /// Indicates whether the request was successful.
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// The data payload.
    /// </summary>
    public T Data { get; set; }

    /// <summary>
    /// Optional error message if Success is false.
    /// </summary>
    public string? Message { get; set; }

    public ApiResponse(T data, bool success = true, string? message = null)
    {
        Success = success;
        Data = data;
        Message = message;
    }
}

/// <summary>
/// API response wrapper for single item results.
/// </summary>
/// <typeparam name="T">The type of data in the response.</typeparam>
public class ApiSingleResponse<T>
{
    /// <summary>
    /// Indicates whether the request was successful.
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// The data payload.
    /// </summary>
    public T? Data { get; set; }

    /// <summary>
    /// Optional message.
    /// </summary>
    public string? Message { get; set; }

    public ApiSingleResponse(T? data, bool success = true, string? message = null)
    {
        Success = success;
        Data = data;
        Message = message;
    }
}
