namespace BlazorBookApp.Server.Tests;

public class CacheServiceTests
{
    private readonly IMemoryCache _cache = new MemoryCache(new MemoryCacheOptions());
    private readonly Mock<ILogger<CacheService>> _loggerMock = new();

    [Fact]
    public async Task GetOrCreateAsync_ReturnsCachedValue_WhenAvailable()
    {
        // Arrange
        var service = new CacheService(_cache, _loggerMock.Object);
        _cache.Set("test", "cached");

        // Act
        var result = await service.GetOrCreateAsync("test", () => Task.FromResult("new"), TimeSpan.FromMinutes(1));

        // Assert
        Assert.Equal("cached", result);
    }

    [Fact]
    public async Task GetOrCreateAsync_CachesValue_WhenNotPresent()
    {
        // Arrange
        var service = new CacheService(_cache, _loggerMock.Object);

        // Act
        var result = await service.GetOrCreateAsync("newKey", () => Task.FromResult("fresh"), TimeSpan.FromMinutes(1));

        // Assert
        Assert.Equal("fresh", result);
        Assert.True(_cache.TryGetValue("newKey", out _));
    }

    [Fact]
    public async Task GetOrCreateAsync_ReturnsDefault_WhenFactoryThrows()
    {
        // Arrange
        var service = new CacheService(_cache, _loggerMock.Object);

        // Act
        var result = await service.GetOrCreateAsync<string>("errorKey", () => throw new Exception("fail"), TimeSpan.FromMinutes(1));

        // Assert
        Assert.Null(result);
        _loggerMock.VerifyLog(LogLevel.Error, Times.Once());
    }
}