using System.Text.RegularExpressions;

namespace Chatbot_backend.Helpers
{
    public class PasswordValidator
    {
        public static bool IsValid(string password, out List<string> errors)
        {
            errors = new List<string>();

            // Check minimum length
            if (password.Length < 8)
            {
                errors.Add("Password must be at least 8 characters long");
            }

            // Check for uppercase letters
            if (!Regex.IsMatch(password, @"[A-Z]"))
            {
                errors.Add("Password must contain at least one uppercase letter");
            }

            // Check for lowercase letters
            if (!Regex.IsMatch(password, @"[a-z]"))
            {
                errors.Add("Password must contain at least one lowercase letter");
            }

            // Check for digits
            if (!Regex.IsMatch(password, @"[0-9]"))
            {
                errors.Add("Password must contain at least one digit");
            }

            // Check for special characters
            if (!Regex.IsMatch(password, @"[^a-zA-Z0-9]"))
            {
                errors.Add("Password must contain at least one special character");
            }

            return errors.Count == 0;
        }
    }
}

