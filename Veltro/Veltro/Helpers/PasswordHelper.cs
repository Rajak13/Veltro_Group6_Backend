namespace Veltro.Helpers;

/// <summary>BCrypt-based password hashing and verification utilities.</summary>
public static class PasswordHelper
{
    /// <summary>Hashes a plain-text password using BCrypt.</summary>
    public static string Hash(string password) =>
        BCrypt.Net.BCrypt.HashPassword(password);

    /// <summary>Verifies a plain-text password against a BCrypt hash.</summary>
    public static bool Verify(string password, string hash) =>
        BCrypt.Net.BCrypt.Verify(password, hash);
}
