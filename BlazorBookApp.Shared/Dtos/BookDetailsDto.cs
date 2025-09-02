namespace BlazorBookApp.Shared.Dtos;

/// <summary>
/// Represents detailed book information.
/// </summary>
public sealed class BookDetailsDto
{
    /// <summary>
    /// Unique work identifier.
    /// </summary>
    public string? WorkId { get; set; }

    /// <summary>
    /// Book title.
    /// </summary>
    public string? Title { get; set; }

    /// <summary>
    /// Book description.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Subjects or categories.
    /// </summary>
    public string[] Subjects { get; set; } = Array.Empty<string>();

    /// <summary>
    /// Open Library cover IDs.
    /// </summary>
    public int[] CoverIds { get; set; } = Array.Empty<int>();

    /// <summary>
    /// URLs of cover images.
    /// </summary>
    public string[] CoverUrls { get; set; } = Array.Empty<string>();

    /// <summary>
    /// Average rating.
    /// </summary>
    public double? AverageRating { get; set; }

    /// <summary>
    /// Number of ratings.
    /// </summary>
    public int? RatingsCount { get; set; }
}