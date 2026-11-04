namespace Murmur.SharedKernel.Results;

/// <summary>Error categories; each maps to one HTTP status when converted to ProblemDetails.</summary>
public enum ErrorType
{
    Validation,
    NotFound,
    Conflict,
    Forbidden,
    Unauthorized,
}
