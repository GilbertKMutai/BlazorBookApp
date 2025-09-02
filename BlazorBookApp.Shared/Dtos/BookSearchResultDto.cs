namespace BlazorBookApp.Shared.Dtos;

/// <summary>
/// Represents a book returned in search results.
/// </summary>
public sealed class BookSearchResultDto
{
    /// <summary>
    /// Unique work identifier.
    /// </summary>
    public string? WorkId { get; set; }

    /// <summary>
    /// Book title.
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// List of authors.
    /// </summary>
    public string[] Authors { get; set; } = Array.Empty<string>();

    /// <summary>
    /// Year of first publication.
    /// </summary>
    public int? FirstPublishYear { get; set; }

    /// <summary>
    /// Open Library cover ID.
    /// </summary>
    public int? CoverId { get; set; }

    /// <summary>
    /// URL of the cover image.
    /// </summary>
    public string? CoverUrl { get; set; }

    /// <summary>
    /// Average rating.
    /// </summary>
    public double? AverageRating { get; set; }

    /// <summary>
    /// Number of ratings.
    /// </summary>
    public int? RatingsCount { get; set; }
}