namespace BlazorBookApp.Server.Services.Exceptions;

/// <summary>
/// Exception thrown when a book service operation fails.
/// </summary>
public class BookServiceException : Exception
{
    /// <summary>
    /// The operation that caused the failure (e.g., "SearchAsync").
    /// </summary>
    public string Operation { get; }

    /// <summary>
    /// The resource identifier related to the failure.
    /// </summary>
    public string ResourceId { get; }

    /// <summary>
    /// Creates a new instance of <see cref="BookServiceException"/>.
    /// </summary>
    /// <param name="operation">The failed operation.</param>
    /// <param name="resourceId">The related resource ID.</param>
    /// <param name="message">The error message.</param>
    public BookServiceException(string operation, string resourceId, string message)
        : base(message)
    {
        Operation = operation;
        ResourceId = resourceId;
    }

    /// <summary>
    /// Creates a new instance of <see cref="BookServiceException"/> with an inner exception.
    /// </summary>
    /// <param name="operation">The failed operation.</param>
    /// <param name="resourceId">The related resource ID.</param>
    /// <param name="message">The error message.</param>
    /// <param name="innerException">The underlying exception.</param>
    public BookServiceException(string operation, string resourceId, string message, Exception innerException)
        : base(message, innerException)
    {
        Operation = operation;
        ResourceId = resourceId;
    }
}
