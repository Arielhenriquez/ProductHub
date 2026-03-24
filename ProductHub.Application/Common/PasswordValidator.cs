namespace ProductHub.Application.Common;

/// <summary>
/// Shared, reusable password policy validator.
/// Used by RegisterAsync and ResetPasswordAsync so both always enforce the same rules.
/// </summary>
public static class PasswordValidator
{
    public static List<string> Validate(string password)
    {
        var errors = new List<string>();

        if (password.Length < 8)
            errors.Add("Password must be at least 8 characters.");
        if (!password.Any(char.IsUpper))
            errors.Add("Password must contain at least one uppercase letter.");
        if (!password.Any(char.IsLower))
            errors.Add("Password must contain at least one lowercase letter.");
        if (!password.Any(char.IsDigit))
            errors.Add("Password must contain at least one number.");
        if (!password.Any(c => !char.IsLetterOrDigit(c)))
            errors.Add("Password must contain at least one special character.");

        return errors;
    }
}
