namespace Murmur.SharedKernel;

/// <summary>
/// The only way modules refer to a user (ADR-0010): never Identity's internal key or the email address.
/// </summary>
/// <remarks><c>default(UserId)</c> wraps <see cref="Guid.Empty"/> and is never a valid user.</remarks>
public readonly record struct UserId(Guid Value)
{
    /// <summary>Creates a new time-ordered (version 7) id, which keeps Postgres B-tree inserts sequential.</summary>
    public static UserId New() => new(Guid.CreateVersion7());

    public override string ToString() => Value.ToString();
}
