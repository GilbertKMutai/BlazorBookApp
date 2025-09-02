namespace BlazorBookApp.Client.Services.Contracts;

/// <summary>
/// Defines a service for handling and formatting application errors.
/// </summary>
public interface IErrorHandlerService
{
    /// <summary>
    /// Handles an exception and returns a user-friendly error message.
    /// </summary>
    /// <param name="ex">The exception to handle.</param>
    /// <returns>A formatted error message describing the issue.</returns>
    string HandleError(Exception ex);

    /// <summary>
    /// Handles an exception with additional context information and returns a user-friendly error message.
    /// </summary>
    /// <param name="ex">The exception to handle.</param>
    /// <param name="context">Additional context or description about where the error occurred.</param>
    /// <returns>A formatted error message describing the issue.</returns>
    string HandleError(Exception ex, string context);
}
