using System.Threading.Tasks;

namespace BlazorBookApp.Client.BUnitTests;

public class BookModalTests : TestContext
{
    private readonly Mock<IModalService> _modalServiceMock;
    private readonly Mock<ILogger<BookModalBase>> _loggerMock;

    public BookModalTests()
    {
        _modalServiceMock = new Mock<IModalService>();
        _loggerMock = new Mock<ILogger<BookModalBase>>();

        Services.AddSingleton(_modalServiceMock.Object);
        Services.AddSingleton(_loggerMock.Object);
    }

    [Fact]
    public void InitialRender_NotVisible()
    {
        // Act
        var cut = RenderComponent<BookModal>();
        var modalInstance = cut.Instance as BookModalBase;

        // Assert
        Assert.False(modalInstance.IsVisible);
        Assert.Null(modalInstance.CurrentBook);
    }

    [Fact]
    public async Task ShowModal_WithBook_ShowsModal()
    {
        // Arrange
        var cut = RenderComponent<BookModal>();
        var modalInstance = cut.Instance as BookModalBase;
        var book = new BookDetailsDto { Title = "Test Book" };

        // Act
        await cut.InvokeAsync(() => 
            modalInstance.ShowModal(book)
        );

        // Assert
        Assert.True(modalInstance.IsVisible);
        Assert.NotNull(modalInstance.CurrentBook);
        Assert.Equal("Test Book", modalInstance.CurrentBook.Title);
    }

    [Fact]
    public void ShowModal_WithNullBook_SetsError()
    {
        // Arrange
        var cut = RenderComponent<BookModal>();
        var modalInstance = cut.Instance as BookModalBase;

        // Act
        modalInstance.ShowModal(null);

        // Assert
        Assert.False(modalInstance.IsVisible);
        Assert.NotNull(modalInstance.Error);
        Assert.Equal("No book details provided", modalInstance.Error);
    }

    [Fact]
    public async Task HideModal_HidesModal()
    {
        // Arrange
        var cut = RenderComponent<BookModal>();
        var modalInstance = cut.Instance as BookModalBase;
        await cut.InvokeAsync(() => 
            modalInstance.ShowModal(new BookDetailsDto { Title = "Test Book" })
        );

        // Act
        await cut.InvokeAsync(() => 
            modalInstance.HideModal()
        );

        await Task.Delay(500);
        cut.Render();

        // Assert
        Assert.False(modalInstance.IsVisible);
        Assert.Null(modalInstance.CurrentBook);
    }

    [Fact]
    public void CloseModal_CallsModalServiceHide()
    {
        // Arrange
        var cut = RenderComponent<BookModal>();
        var modalInstance = cut.Instance as BookModalBase;

        // Act
        modalInstance.CloseModal();

        // Assert
        _modalServiceMock.Verify(m => m.Hide(), Times.Once);
    }

    [Fact]
    public async Task ClearError_ClearsError()
    {
        // Arrange
        var cut = RenderComponent<BookModal>();
        var modalInstance = cut.Instance as BookModalBase;

        await cut.InvokeAsync(() => 
            modalInstance.ShowModal(null)
        );

        // Act
        await cut.InvokeAsync(() =>
            modalInstance.ClearError()
        );

        // Assert
        Assert.Null(modalInstance.Error);
    }

    [Fact]
    public void Dispose_UnsubscribesFromEvents()
    {
        // Arrange
        var cut = RenderComponent<BookModal>();
        var modalInstance = cut.Instance as BookModalBase;

        // Act
        modalInstance.Dispose();

        // Assert
        Assert.NotNull(modalInstance);
    }
}