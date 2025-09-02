namespace BlazorBookApp.Client.Components;

/// <summary>
/// Base class for the search results component. 
/// Renders a collection of book result cards and 
/// allows selection of a specific book.
/// </summary>
public class SearchResultBase : ComponentBase
{
    /// <summary>
    /// The collection of book search results to display.
    /// Each item contains summary information about a book.
    /// </summary>
    [Parameter]
    public BookSearchResultDto[] Items { get; set; } = new BookSearchResultDto[0];

    /// <summary>
    /// Callback that is invoked when a book card is selected.
    /// The selected <see cref="BookSearchResultDto"/> is passed as the argument.
    /// </summary>
    [Parameter]
    public EventCallback<BookSearchResultDto> OnSelect { get; set; }
}
