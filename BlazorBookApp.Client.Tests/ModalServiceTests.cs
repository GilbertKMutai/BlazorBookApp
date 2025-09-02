namespace BlazorBookApp.Client.Tests;

public class ModalServiceTests
{
    private readonly ModalService _service;

    public ModalServiceTests()
    {
        _service = new ModalService();
    }

    [Fact]
    public void ShowDetails_RaisesOnShowEvent()
    {
        // Arrange
        var book = new BookDetailsDto { Title = "Test Book" };
        BookDetailsDto raisedBook = null;
        _service.OnShow += (b) => raisedBook = b;

        // Act
        _service.ShowDetails(book);

        // Assert
        Assert.NotNull(raisedBook);
        Assert.Equal(book.Title, raisedBook.Title);
    }

    [Fact]
    public void Hide_RaisesOnHideEvent()
    {
        // Arrange
        var eventRaised = false;
        _service.OnHide += () => eventRaised = true;

        // Act
        _service.Hide();

        // Assert
        Assert.True(eventRaised);
    }

    [Fact]
    public void ShowDetails_WithNullBook_DoesNotRaiseException()
    {
        // Arrange
        var exceptionThrown = false;
        _service.OnShow += (b) => { };

        // Act
        try
        {
            _service.ShowDetails(null);
        }
        catch
        {
            exceptionThrown = true;
        }

        // Assert
        Assert.False(exceptionThrown);
    }
}
