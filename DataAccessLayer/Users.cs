using DataAccessLayer;
using Microsoft.Data.SqlClient;
using System;
using System.Data;

namespace studentDataAccessLayer
{
    // 1. كلاس نقل البيانات (DTO) الخاص بالمستخدم
    public class UserDTO
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string PasswordHash { get; set; }
        public string Role { get; set; } // Admin أو Student

        public UserDTO(int id, string username, string passwordHash, string role)
        {
            this.Id = id;
            this.Username = username;
            this.PasswordHash = passwordHash;
            this.Role = role;
        }
    }

    // 2. كلاس التعامل المباشر مع قاعدة البيانات
    public class UserData
    {
        // دالة للبحث عن مستخدم عن طريق اسم المستخدم (Username) - ضرورية جداً للـ Login
        public static UserDTO GetUserByUsername(string username)
        {
            using (SqlConnection connection = new SqlConnection(DatabaseSettings.ConnectionString))
            {
                string query = "SELECT Id, Username, PasswordHash, Role FROM Users WHERE Username = @Username";

                using (SqlCommand comm = new SqlCommand(query, connection))
                {
                    // استخدام SqlDbType بدل AddWithValue
                    comm.Parameters.Add("@Username", SqlDbType.NVarChar, 50).Value = username;

                    connection.Open();
                    using (var reader = comm.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new UserDTO(
                                reader.GetInt32(reader.GetOrdinal("Id")),
                                reader.GetString(reader.GetOrdinal("Username")),
                                reader.GetString(reader.GetOrdinal("PasswordHash")),
                                reader.GetString(reader.GetOrdinal("Role"))
                            );
                        }
                        return null;
                    }
                }
            }
        }

        // دالة للبحث عن مستخدم باستخدام الـ Id الأساسي
        public static UserDTO GetUserById(int id)
        {
            using (SqlConnection connection = new SqlConnection(DatabaseSettings.ConnectionString))
            {
                string query = "SELECT Id, Username, PasswordHash, Role FROM Users WHERE Id = @Id";

                using (SqlCommand comm = new SqlCommand(query, connection))
                {
                    comm.Parameters.Add("@Id", SqlDbType.Int).Value = id;

                    connection.Open();
                    using (var reader = comm.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new UserDTO(
                                reader.GetInt32(reader.GetOrdinal("Id")),
                                reader.GetString(reader.GetOrdinal("Username")),
                                reader.GetString(reader.GetOrdinal("PasswordHash")),
                                reader.GetString(reader.GetOrdinal("Role"))
                            );
                        }
                        return null;
                    }
                }
            }
        }

        // دالة لإضافة مستخدم جديد - مهمة جداً لعملية التسجيل (Register)
        public static int AddUser(UserDTO userDTO)
        {
            using (SqlConnection connection = new SqlConnection(DatabaseSettings.ConnectionString))
            {
                // بنستخدم SCOPE_IDENTITY عشان نرجع الـ ID اللي اتعمله Generate لليوزر الجديد
                // وده هنحتاجه عشان نربط الطالب (Student) باليوزر ده
                string query = @"INSERT INTO Users (Username, PasswordHash, Role) 
                                 VALUES (@Username, @PasswordHash, @Role);
                                 SELECT SCOPE_IDENTITY();";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.Add("@Username", SqlDbType.NVarChar, 50).Value = userDTO.Username;
                    command.Parameters.Add("@PasswordHash", SqlDbType.NVarChar, 256).Value = userDTO.PasswordHash;
                    command.Parameters.Add("@Role", SqlDbType.NVarChar, 20).Value = userDTO.Role;

                    connection.Open();

                    object result = command.ExecuteScalar();
                    return Convert.ToInt32(result);
                }
            }
        }

        // دالة لتحديث بيانات الدخول (مثلاً تغيير الباسورد)
        public static bool UpdateUser(UserDTO userDTO)
        {
            using (SqlConnection connection = new SqlConnection(DatabaseSettings.ConnectionString))
            {
                string query = @"UPDATE Users 
                                 SET Username = @Username, 
                                     PasswordHash = @PasswordHash, 
                                     Role = @Role 
                                 WHERE Id = @Id";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.Add("@Id", SqlDbType.Int).Value = userDTO.Id;
                    command.Parameters.Add("@Username", SqlDbType.NVarChar, 50).Value = userDTO.Username;
                    command.Parameters.Add("@PasswordHash", SqlDbType.NVarChar, 256).Value = userDTO.PasswordHash;
                    command.Parameters.Add("@Role", SqlDbType.NVarChar, 20).Value = userDTO.Role;

                    connection.Open();
                    int rowsAffected = command.ExecuteNonQuery();
                    return rowsAffected > 0;
                }
            }
        }

        // دالة لحذف المستخدم (لو مسحناه، جدول الطلاب هيمسح بيانات الطالب المرتبطة بيه بسبب ON DELETE CASCADE)
        public static bool DeleteUser(int id)
        {
            using (SqlConnection connection = new SqlConnection(DatabaseSettings.ConnectionString))
            {
                string query = "DELETE FROM Users WHERE Id = @Id";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.Add("@Id", SqlDbType.Int).Value = id;
                    connection.Open();

                    int rowsAffected = command.ExecuteNonQuery();
                    return rowsAffected > 0;
                }
            }
        }
    }
}