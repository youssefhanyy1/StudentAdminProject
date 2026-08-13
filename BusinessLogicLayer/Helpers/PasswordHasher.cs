using Microsoft.AspNetCore.Localization;

namespace BusinessLogicLayer.Helpers
{
    public static class PasswordHasher
    {

        public static string HashPassword(string password)
        {

            return BCrypt.Net.BCrypt.HashPassword(password);
        }
        public static bool VerifyPassword(string enteredPassword, string hashedPassword)
        {
            if (string.IsNullOrEmpty(enteredPassword) || string.IsNullOrEmpty(hashedPassword))
            {
                return false;
            }


            return BCrypt.Net.BCrypt.Verify(enteredPassword, hashedPassword);
           
        }
    }
}