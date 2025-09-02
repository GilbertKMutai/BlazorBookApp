[assembly: InternalsVisibleTo("BlazorBookApp.Client.BUnitTests")]
namespace BlazorBookApp.Client.Pages;

/// <summary>
/// Base component for the home page.  
/// Handles search, error display, loading state, and showing book details in a modal.
/// </summary>
public class HomeBase : ComponentBase
{
    internal BookSearchResultDto[]? Results { get; set; }
    internal bool ShowError { get; set; }
    internal string ErrorMessage { get; set; } = string.Empty;
    protected bool IsLoading { get; set; }

    [Inject]
    private IApiClient Api { get; set; } = default!;

    [Inject]
    private IModalService ModalService { get; set; } = default!;

    [Inject]
    private ILogger<HomeBase> Logger { get; set; } = default!;

    internal async Task OnSearch(string query)
    {
        ShowError = false;
        ErrorMessage = string.Empty;
        IsLoading = true;
        StateHasChanged();

        var result = await Api.SearchAsync(query);

        if (result.IsSuccess)
        {
            Results = result.Value;
        }
        else
        {
            ShowError = true;
            ErrorMessage = GetUserFriendlyErrorMessage(result.Error, result.StatusCode, result.ErrorCode);
            Results = Array.Empty<BookSearchResultDto>();

            if (result.StatusCode >= 500)
            {
                Logger.LogError("API error during search: {ErrorCode} - {ErrorMessage}",
                    result.ErrorCode, result.Error);
            }
        }

        IsLoading = false;
    }

    internal async Task ShowDetails(BookSearchResultDto book)
    {
        if (book.WorkId == null) return;

        ShowError = false;
        ErrorMessage = string.Empty;

        var result = await Api.GetDetailsAsync(book.WorkId);

        if (result.IsSuccess)
        {
            ModalService.ShowDetails(result.Value);
        }
        else
        {
            ShowError = true;
            ErrorMessage = GetUserFriendlyErrorMessage(result.Error, result.StatusCode, result.ErrorCode);

            if (result.StatusCode >= 500)
            {
                Logger.LogError("API error fetching details: {ErrorCode} - {ErrorMessage}",
                    result.ErrorCode, result.Error);
            }
        }
    }

    internal string GetUserFriendlyErrorMessage(string error, int statusCode, string errorCode)
    {
        return errorCode switch
        {
            "TITLE_REQUIRED" => "Please enter a book title to search",
            "WORK_ID_REQUIRED" => "Book ID is required",
            "NOT_FOUND" => "The requested book was not found",
            "NETWORK_ERROR" => "Network error. Please check your connection and try again",
            "EXTERNAL_API_ERROR" => "Temporarily unable to search for books. Please try again later",
            "INVALID_RESPONSE" => "Received an invalid response. Please try again",
            _ => statusCode switch
            {
                400 => "Invalid request. Please check your input",
                404 => "The requested resource was not found",
                500 => "An unexpected error occurred. Please try again",
                503 => "Service is temporarily unavailable. Please try again later",
                _ => "An error occurred. Please try again"
            }
        };
    }
}