namespace BlazorBookApp.Client.Components;

/// <summary>
/// Base component for rendering a modal dialog that displays extended
/// book details such as cover image, description, rating, and subjects.
/// </summary>
public class BookModalBase : ComponentBase, IDisposable
{
    [Inject]
    private IModalService ModalService { get; set; } = default!;

    [Inject]
    private ILogger<BookModalBase> Logger { get; set; } = default!;

    internal bool IsVisible { get; set; }
    internal BookDetailsDto? CurrentBook { get; set; }
    internal string? Error { get; set; }

    protected override void OnInitialized()
    {
        try
        {
            ModalService.OnShow += ShowModal;
            ModalService.OnHide += HideModal;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Failed to initialize modal event handlers");
            Error = "Failed to initialize modal component";
        }
    }

    internal void ShowModal(BookDetailsDto? book)
    {

        if (book == null)
        {
            Error = "No book details provided";
            return;
        }

        try
        {
            Error = null;

            CurrentBook = book;
            IsVisible = true;
            StateHasChanged();
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error showing modal for book");
            Error = "Failed to display book details";
            StateHasChanged();
        }
    }

    internal async void HideModal()
    {
        try
        {
            IsVisible = false;

            await Task.Delay(400);

            CurrentBook = null;
            Error = null;
            StateHasChanged();
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error hiding modal");
            Error = "Failed to close modal";
            StateHasChanged();
        }
    }

    internal void CloseModal()
    {
        try
        {
            ModalService.Hide();
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error closing modal");
            Error = "Failed to close modal";
            StateHasChanged();
        }
    }

    internal void ClearError()
    {
        Error = null;
        StateHasChanged();
    }

    /// <summary>
    /// Releases resources and unsubscribes from modal service events
    /// when the component is disposed.
    /// </summary>
    public void Dispose()
    {
        try
        {
            ModalService.OnShow -= ShowModal;
            ModalService.OnHide -= HideModal;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error disposing modal component");
        }
    }
}