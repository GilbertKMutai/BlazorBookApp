namespace BlazorBookApp.Client.Components;
/// <summary>
/// Represents the base component for displaying detailed information
/// about a selected book, including cover, description, and subjects.
/// </summary>
public class BookDetailsItemBase : ComponentBase
{
    /// <summary>
    /// Gets or sets the details of the currently selected book.
    /// If <c>null</c>, the component displays an empty state.
    /// </summary>
    [Parameter]
    public BookDetailsDto? Item { get; set; }

    /// <summary>
    /// Gets or sets the callback invoked when the user closes the details view.
    /// </summary>
    [Parameter]
    public EventCallback OnClose { get; set; }
}