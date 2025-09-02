namespace BlazorBookApp.Shared.Utilities;

/// <summary>
/// Represents the result of an operation that returns a value.
/// Encapsulates success/failure state, value, error details, and status codes.
/// </summary>
/// <typeparam name="T">The type of the value returned by the operation.</typeparam>
public class OperationResult<T>
{
    /// <summary>
    /// Gets a value indicating whether the operation was successful.
    /// </summary>
    public bool IsSuccess { get; }

    /// <summary>
    /// Gets the value returned by the operation if successful.
    /// </summary>
    public T Value { get; }

    /// <summary>
    /// Gets the error message if the operation failed; otherwise <c>null</c>.
    /// </summary>
    public string Error { get; }

    /// <summary>
    /// Gets the HTTP-style status code associated with the operation result.
    /// Defaults to <c>200</c> on success.
    /// </summary>
    public int StatusCode { get; }

    /// <summary>
    /// Gets a machine-friendly error code (e.g., <c>NETWORK_ERROR</c>, <c>NOT_FOUND</c>).
    /// </summary>
    public string ErrorCode { get; }

    /// <summary>
    /// Creates a new instance of <see cref="OperationResult{T}"/>.
    /// </summary>
    /// <param name="value">The value returned by the operation.</param>
    /// <param name="isSuccess">Whether the operation succeeded.</param>
    /// <param name="error">The error message if failed.</param>
    /// <param name="statusCode">The status code associated with the result.</param>
    /// <param name="errorCode">A machine-friendly error code.</param>
    [JsonConstructor]
    protected OperationResult(T value, bool isSuccess, string error, int statusCode, string errorCode)
    {
        Value = value;
        IsSuccess = isSuccess;
        Error = error;
        StatusCode = statusCode;
        ErrorCode = errorCode;
    }

    /// <summary>
    /// Creates a successful result containing the specified value.
    /// </summary>
    /// <param name="value">The value to return in the result.</param>
    /// <returns>An <see cref="OperationResult{T}"/> marked as success.</returns>
    public static OperationResult<T> Success(T value) => new OperationResult<T>(value, true, null, 200, null);

    /// <summary>
    /// Creates a failed result with the specified error details.
    /// </summary>
    /// <param name="error">The human-readable error message.</param>
    /// <param name="statusCode">The HTTP-style status code to return (default is 500).</param>
    /// <param name="errorCode">The machine-friendly error code (optional).</param>
    /// <returns>An <see cref="OperationResult{T}"/> marked as failure.</returns>
    public static OperationResult<T> Failure(string error, int statusCode = 500, string errorCode = null) =>
        new OperationResult<T>(default, false, error, statusCode, errorCode);
}

/// <summary>
/// Represents the result of an operation that does not return a value.
/// Encapsulates success/failure state, error details, and status codes.
/// </summary>
public class OperationResult
{
    /// <summary>
    /// Gets a value indicating whether the operation was successful.
    /// </summary>
    public bool IsSuccess { get; }

    /// <summary>
    /// Gets the error message if the operation failed; otherwise <c>null</c>.
    /// </summary>
    public string Error { get; }

    /// <summary>
    /// Gets the HTTP-style status code associated with the operation result.
    /// Defaults to <c>200</c> on success.
    /// </summary>
    public int StatusCode { get; }

    /// <summary>
    /// Gets a machine-friendly error code (e.g., <c>NETWORK_ERROR</c>, <c>NOT_FOUND</c>).
    /// </summary>
    public string ErrorCode { get; }

    /// <summary>
    /// Creates a new instance of <see cref="OperationResult"/>.
    /// </summary>
    /// <param name="isSuccess">Whether the operation succeeded.</param>
    /// <param name="error">The error message if failed.</param>
    /// <param name="statusCode">The status code associated with the result.</param>
    /// <param name="errorCode">A machine-friendly error code.</param>
    protected OperationResult(bool isSuccess, string error, int statusCode, string errorCode)
    {
        IsSuccess = isSuccess;
        Error = error;
        StatusCode = statusCode;
        ErrorCode = errorCode;
    }

    /// <summary>
    /// Creates a successful result.
    /// </summary>
    /// <returns>An <see cref="OperationResult"/> marked as success.</returns>
    public static OperationResult Success() => new OperationResult(true, null, 200, null);

    /// <summary>
    /// Creates a failed result with the specified error details.
    /// </summary>
    /// <param name="error">The human-readable error message.</param>
    /// <param name="statusCode">The HTTP-style status code to return (default is 500).</param>
    /// <param name="errorCode">The machine-friendly error code (optional).</param>
    /// <returns>An <see cref="OperationResult"/> marked as failure.</returns>
    public static OperationResult Failure(string error, int statusCode = 500, string errorCode = null) =>
        new OperationResult(false, error, statusCode, errorCode);
}