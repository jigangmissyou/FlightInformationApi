using FlightInformationApi.Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using System.Text.Json;
using Xunit;

namespace FlightInformationApi.Tests.Infrastructure;

public class GlobalExceptionHandlerTests
{
    private readonly Mock<ILogger<GlobalExceptionHandler>> _mockLogger;
    private readonly GlobalExceptionHandler _handler;

    public GlobalExceptionHandlerTests()
    {
        _mockLogger = new Mock<ILogger<GlobalExceptionHandler>>();
        _handler = new GlobalExceptionHandler(_mockLogger.Object);
    }

    [Fact]
    public async Task TryHandleAsync_LogsException()
    {
        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();
        var exception = new Exception("Test exception");

        await _handler.TryHandleAsync(context, exception, CancellationToken.None);

        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("An unhandled exception has occurred.")),
                exception,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public async Task TryHandleAsync_Returns500StatusCode()
    {
        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();
        var exception = new Exception("Test exception");

        await _handler.TryHandleAsync(context, exception, CancellationToken.None);

        Assert.Equal(StatusCodes.Status500InternalServerError, context.Response.StatusCode);
    }

    [Fact]
    public async Task TryHandleAsync_ReturnsApiSingleResponseFormat()
    {
        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();
        var exception = new Exception("Test exception message");

        await _handler.TryHandleAsync(context, exception, CancellationToken.None);

        context.Response.Body.Seek(0, SeekOrigin.Begin);
        var reader = new StreamReader(context.Response.Body);
        var responseBody = await reader.ReadToEndAsync();

        var jsonDoc = JsonDocument.Parse(responseBody);
        Assert.False(jsonDoc.RootElement.GetProperty("success").GetBoolean());
        Assert.Equal("Test exception message", jsonDoc.RootElement.GetProperty("message").GetString());
        Assert.True(jsonDoc.RootElement.GetProperty("data").ValueKind == JsonValueKind.Null);
    }

    [Fact]
    public async Task TryHandleAsync_ReturnsTrue()
    {
        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();
        var exception = new Exception("Test exception");

        var result = await _handler.TryHandleAsync(context, exception, CancellationToken.None);

        Assert.True(result);
    }
}
