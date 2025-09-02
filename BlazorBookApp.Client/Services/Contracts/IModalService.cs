namespace BlazorBookApp.Client.Services.Contracts;

/// <summary>
/// Defines a service for displaying and hiding modal dialogs that show
/// detailed information about a book.
/// </summary>
public interface IModalService
{
    /// <summary>
    /// Occurs when a modal should be displayed with the specified book details.
    /// </summary>
    event Action<BookDetailsDto>? OnShow;

    /// <summary>
    /// Occurs when a modal should be hidden.
    /// </summary>
    event Action? OnHide;

    /// <summary>
    /// Displays a modal containing details about the specified book.
    /// </summary>
    /// <param name="book">The book whose details should be shown in the modal.</param>
    void ShowDetails(BookDetailsDto book);

    /// <summary>
    /// Hides the currently displayed modal.
    /// </summary>
    void Hide();
}
