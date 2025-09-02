namespace BlazorBookApp.Client.Services;

/// <summary>
/// Default implementation of <see cref="IModalService"/> for raising events
/// to show or hide a modal dialog.
/// </summary>
public class ModalService : IModalService
{
    /// <inheritdoc />
    public event Action<BookDetailsDto>? OnShow;

    /// <inheritdoc />
    public event Action? OnHide;

    /// <inheritdoc />
    public void ShowDetails(BookDetailsDto book) => OnShow?.Invoke(book);

    /// <inheritdoc />
    public void Hide() => OnHide?.Invoke();
}
