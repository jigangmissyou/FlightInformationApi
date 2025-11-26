using FlightInformationApi.DTOs;
using Xunit;

namespace FlightInformationApi.Tests.DTOs;

public class PaginatedResponseTests
{
    [Fact]
    public void Constructor_WithZeroTotalCount_CalculatesCorrectly()
    {
        var response = new PaginatedResponse<string>(new List<string>(), 0, 1, 10);

        Assert.Equal(0, response.TotalCount);
        Assert.Equal(0, response.TotalPages);
        Assert.False(response.HasPreviousPage);
        Assert.False(response.HasNextPage);
    }

    [Fact]
    public void Constructor_WithPageSizeEqualToTotalCount_CalculatesOnePage()
    {
        var data = new List<string> { "item1", "item2", "item3" };
        var response = new PaginatedResponse<string>(data, 3, 1, 3);

        Assert.Equal(1, response.TotalPages);
        Assert.False(response.HasPreviousPage);
        Assert.False(response.HasNextPage);
    }

    [Fact]
    public void Constructor_OnFirstPage_HasNosPreviousPage()
    {
        var response = new PaginatedResponse<string>(new List<string>(), 100, 1, 10);

        Assert.False(response.HasPreviousPage);
        Assert.True(response.HasNextPage);
    }

    [Fact]
    public void Constructor_OnLastPage_HasNosNextPage()
    {
        var response = new PaginatedResponse<string>(new List<string>(), 25, 3, 10);

        Assert.True(response.HasPreviousPage);
        Assert.False(response.HasNextPage);
        Assert.Equal(3, response.TotalPages);
    }

    [Fact]
    public void Constructor_OnMiddlePage_HasBothPreviousAndNext()
    {
        var response = new PaginatedResponse<string>(new List<string>(), 100, 5, 10);

        Assert.True(response.HasPreviousPage);
        Assert.True(response.HasNextPage);
        Assert.Equal(10, response.TotalPages);
    }

    [Fact]
    public void DefaultConstructor_InitializesWithDefaults()
    {
        var response = new PaginatedResponse<string>();

        Assert.True(response.Success);
        Assert.Empty(response.Data);
        Assert.Equal(0, response.TotalCount);
        Assert.Null(response.Message);
    }
}
