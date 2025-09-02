namespace BlazorBookApp.Client.BUnitTests;

public class BookDetailsItemTests : TestContext
{
    [Fact]
    public void Render_WithNullItem_ShowsEmptyState()
    {
        // Arrange
        var cut = RenderComponent<BookDetailsItem>();

        // Act
        var emptyState = cut.Find(".empty-state");

        // Assert
        Assert.NotNull(emptyState);
        Assert.Contains("Select a book to see details", emptyState.TextContent);
    }

    [Fact]
    public void Render_WithValidItem_ShowsBookDetails()
    {
        // Arrange
        var book = new BookDetailsDto
        {
            Title = "Test Book",
            Description = "Test Description",
            Subjects = new[] { "Subject1", "Subject2" },
            CoverIds = new[] { 12345 }
        };

        // Act
        var cut = RenderComponent<BookDetailsItem>(parameters => parameters
            .Add(p => p.Item, book)
        );

        // Assert
        Assert.Contains("Test Book", cut.Markup);
        Assert.Contains("Test Description", cut.Markup);
        Assert.Contains("Subject1", cut.Markup);
        Assert.Contains("Subject2", cut.Markup);
    }

    [Fact]
    public void Render_WithNoCoverIds_ShowsPlaceholder()
    {
        // Arrange
        var book = new BookDetailsDto
        {
            Title = "Test Book",
            CoverIds = new int[0]
        };

        // Act
        var cut = RenderComponent<BookDetailsItem>(parameters => parameters
            .Add(p => p.Item, book)
        );

        // Assert
        Assert.NotNull(cut.Find(".book-cover-placeholder"));
    }

    [Fact]
    public void ClickCloseButton_TriggersOnClose()
    {
        // Arrange
        var closeInvoked = false;
        var book = new BookDetailsDto { Title = "Test Book" };

        var cut = RenderComponent<BookDetailsItem>(parameters => parameters
            .Add(p => p.Item, book)
            .Add(p => p.OnClose, EventCallback.Factory.Create(this, () => closeInvoked = true))
        );

        // Act
        var closeButton = cut.Find(".btn-close");
        closeButton.Click();

        // Assert
        Assert.True(closeInvoked);
    }
}