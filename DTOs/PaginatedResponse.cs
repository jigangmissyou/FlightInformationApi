namespace FlightInformationApi.DTOs;

/// <summary>
/// Paginated API response wrapper.
/// </summary>
/// <typeparam name="T">The type of data in the response.</typeparam>
public class PaginatedResponse<T>
{
    /// <summary>
    /// Indicates whether the request was successful.
    /// </summary>
    public bool Success { get; set; } = true;

    /// <summary>
    /// Current page number (1-based).
    /// </summary>
    public int PageNumber { get; set; }

    /// <summary>
    /// Number of items per page.
    /// </summary>
    public int PageSize { get; set; }

    /// <summary>
    /// Total number of items across all pages.
    /// </summary>
    public int TotalCount { get; set; }

    /// <summary>
    /// Total number of pages.
    /// </summary>
    public int TotalPages { get; set; }

    /// <summary>
    /// Indicates if there is a previous page.
    /// </summary>
    public bool HasPreviousPage { get; set; }

    /// <summary>
    /// Indicates if there is a next page.
    /// </summary>
    public bool HasNextPage { get; set; }

    /// <summary>
    /// The data for the current page.
    /// </summary>
    public IEnumerable<T> Data { get; set; } = new List<T>();

    /// <summary>
    /// Optional message.
    /// </summary>
    public string? Message { get; set; }

    /// <summary>
    /// Creates a paginated response.
    /// </summary>
    public PaginatedResponse(IEnumerable<T> data, int totalCount, int pageNumber, int pageSize)
    {
        Data = data;
        TotalCount = totalCount;
        PageNumber = pageNumber;
        PageSize = pageSize;
        TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
        HasPreviousPage = pageNumber > 1;
        HasNextPage = pageNumber < TotalPages;
    }

    /// <summary>
    /// Creates an empty paginated response.
    /// </summary>
    public PaginatedResponse()
    {
    }
}
