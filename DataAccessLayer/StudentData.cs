using DataAccessLayer;
using Microsoft.Data.SqlClient;
namespace studentDataAccessLayer
{
    public class StudentDTO
    {
        public int Id { get; set; }
        public int UserId { get; set; } 
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Department { get; set; }
        public decimal? GPA { get; set; } 

        public StudentDTO(int id, int userId, string fullName, string email, string department, decimal? gpa)
        {
            this.Id = id;
            this.UserId = userId;
            this.FullName = fullName;
            this.Email = email;
            this.Department = department;
            this.GPA = gpa;
        }
    }

    public class StudentData
    {

        public static StudentDTO GetStudentByUserId(int userId)
        {
            using (SqlConnection connection = new SqlConnection(DatabaseSettings.ConnectionString))
            {
                string query = "SELECT Id, UserId, FullName, Email, Department, GPA FROM Students WHERE UserId = @UserId";

                using (SqlCommand comm = new SqlCommand(query, connection))
                {
                    comm.Parameters.AddWithValue("@UserId", userId);
                    connection.Open();
                    
                    using (var reader = comm.ExecuteReader())
                    {
                        if (reader.Read())
                        {

                            string department = reader.IsDBNull(reader.GetOrdinal("Department")) ? null : reader.GetString(reader.GetOrdinal("Department"));
                            decimal? gpa = reader.IsDBNull(reader.GetOrdinal("GPA")) ? (decimal?)null : reader.GetDecimal(reader.GetOrdinal("GPA"));

                            return new StudentDTO(
                                reader.GetInt32(reader.GetOrdinal("Id")),
                                reader.GetInt32(reader.GetOrdinal("UserId")),
                                reader.GetString(reader.GetOrdinal("FullName")),
                                reader.GetString(reader.GetOrdinal("Email")),
                                department,
                                gpa
                            );
                        }
                        return null;
                    }
                }
            }
        }

        public static StudentDTO GetStudentById(int id)
        {
            using (SqlConnection connection = new SqlConnection(DatabaseSettings.ConnectionString))
            {
                string query = "SELECT Id, UserId, FullName, Email, Department, GPA FROM Students WHERE Id = @Id";

                using (SqlCommand comm = new SqlCommand(query, connection))
                {

                    comm.Parameters.Add("@Id", System.Data.SqlDbType.Int).Value = id;
                    connection.Open();

                    using (var reader = comm.ExecuteReader())
                    {
                        if (reader.Read())
                        {

                            string department = reader.IsDBNull(reader.GetOrdinal("Department")) ? null : reader.GetString(reader.GetOrdinal("Department"));
                            decimal? gpa = reader.IsDBNull(reader.GetOrdinal("GPA")) ? (decimal?)null : reader.GetDecimal(reader.GetOrdinal("GPA"));

                            return new StudentDTO(
                                reader.GetInt32(reader.GetOrdinal("Id")),
                                reader.GetInt32(reader.GetOrdinal("UserId")),
                                reader.GetString(reader.GetOrdinal("FullName")),
                                reader.GetString(reader.GetOrdinal("Email")),
                                department,
                                gpa
                            );
                        }
                        return null;
                    }
                }
            }
        }
        // 2. جلب كل الطلاب (للأدمن)
        public static List<StudentDTO> GetAllStudents()
        {
            var studentsList = new List<StudentDTO>();
            using (SqlConnection connection = new SqlConnection(DatabaseSettings.ConnectionString))
            {
                string query = "SELECT Id, UserId, FullName, Email, Department, GPA FROM Students";
                
                using (SqlCommand cmd = new SqlCommand(query, connection))
                {
                    connection.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string department = reader.IsDBNull(reader.GetOrdinal("Department")) ? null : reader.GetString(reader.GetOrdinal("Department"));
                            decimal? gpa = reader.IsDBNull(reader.GetOrdinal("GPA")) ? (decimal?)null : reader.GetDecimal(reader.GetOrdinal("GPA"));

                            studentsList.Add(new StudentDTO(
                                reader.GetInt32(reader.GetOrdinal("Id")),
                                reader.GetInt32(reader.GetOrdinal("UserId")),
                                reader.GetString(reader.GetOrdinal("FullName")),
                                reader.GetString(reader.GetOrdinal("Email")),
                                department,
                                gpa
                            ));
                        }
                    }
                }
            }
            return studentsList;
        }

        // 3. إضافة طالب جديد
        public static int AddStudent(StudentDTO studentDTO)
        {
            using (SqlConnection connection = new SqlConnection(DatabaseSettings.ConnectionString))
            {
                string query = @"INSERT INTO Students (UserId, FullName, Email, Department, GPA) 
                                 VALUES (@UserId, @FullName, @Email, @Department, @GPA);
                                 SELECT SCOPE_IDENTITY();"; // عشان نرجع الـ Id الجديد

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@UserId", studentDTO.UserId);
                    command.Parameters.AddWithValue("@FullName", studentDTO.FullName);
                    command.Parameters.AddWithValue("@Email", studentDTO.Email);
                    command.Parameters.AddWithValue("@Department", (object)studentDTO.Department ?? DBNull.Value);
                    command.Parameters.AddWithValue("@GPA", (object)studentDTO.GPA ?? DBNull.Value);

                    connection.Open();
                    object result = command.ExecuteScalar();
                    return Convert.ToInt32(result);
                }
            }
        }

        // 4. تعديل بيانات طالب
        public static bool UpdateStudent(StudentDTO studentDTO)
        {
            using (SqlConnection connection = new SqlConnection(DatabaseSettings.ConnectionString))
            {
                string query = @"UPDATE Students 
                                 SET FullName = @FullName, 
                                     Email = @Email, 
                                     Department = @Department, 
                                     GPA = @GPA 
                                 WHERE Id = @Id";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", studentDTO.Id);
                    command.Parameters.AddWithValue("@FullName", studentDTO.FullName);
                    command.Parameters.AddWithValue("@Email", studentDTO.Email);
                    command.Parameters.AddWithValue("@Department", (object)studentDTO.Department ?? DBNull.Value);
                    command.Parameters.AddWithValue("@GPA", (object)studentDTO.GPA ?? DBNull.Value);

                    connection.Open();
                    int rowsAffected = command.ExecuteNonQuery();
                    return rowsAffected > 0;
                }
            }
        }

        // 5. حذف طالب
        public static bool DeleteStudent(int studentId)
        {
            using (SqlConnection connection = new SqlConnection(DatabaseSettings.ConnectionString))
            {

                string query = "DELETE FROM Students WHERE Id = @Id";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", studentId);
                    connection.Open();
                    
                    int rowsAffected = command.ExecuteNonQuery();
                    return rowsAffected > 0;
                }
            }
        }
    }
}