using BlazorBookApp.Shared.Utilities;

namespace BlazorBookApp.Shared.Tests;

public class OperationResultTests
{
    [Fact]
    public void Success_Should_Set_IsSuccess_To_True()
    {
        // Arrange and Act
        var result = OperationResult<string>.Success("Hello");

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal("Hello", result.Value);
        Assert.Equal(200, result.StatusCode);
        Assert.Null(result.Error);
        Assert.Null(result.ErrorCode);
    }

    [Fact]
    public void Failure_Should_Set_IsSuccess_To_False()
    {
        // Arrange and Act
        var result = OperationResult<string>.Failure("Something went wrong", 400, "BAD_REQUEST");

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("Something went wrong", result.Error);
        Assert.Equal(400, result.StatusCode);
        Assert.Equal("BAD_REQUEST", result.ErrorCode);
    }

    [Fact]
    public void NonGeneric_Failure_Should_Set_Error()
    {
        // Arrange and Act
        var result = OperationResult.Failure("Oops");

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("Oops", result.Error);
        Assert.Equal(500, result.StatusCode);
    }
}