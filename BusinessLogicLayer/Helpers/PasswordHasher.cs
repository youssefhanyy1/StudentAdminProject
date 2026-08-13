namespace BusinessLogicLayer.Helpers
{
    public static class PasswordHasher
    {
        // 1. دالة لتشفير كلمة المرور (تُستخدم عند التسجيل Register)
        public static string HashPassword(string password)
        {
            // مكتبة BCrypt بتقوم بإنشاء Salt عشوائي وتشفير الباسورد تلقائياً
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        // 2. دالة للتحقق من كلمة المرور (تُستخدم عند تسجيل الدخول Login)
        public static bool VerifyPassword(string enteredPassword, string hashedPassword)
        {
            if (string.IsNullOrEmpty(enteredPassword) || string.IsNullOrEmpty(hashedPassword))
            {
                return false;
            }

            // بتقارن الباسورد اللي اليوزر دخله بالـ Hash المتخزن في الداتابيز
            return BCrypt.Net.BCrypt.Verify(enteredPassword, hashedPassword);
        }
    }
}