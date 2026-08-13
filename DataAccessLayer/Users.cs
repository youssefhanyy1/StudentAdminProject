using DataAccessLayer;
using Microsoft.Data.SqlClient;
using System;
using System.Data;

namespace studentDataAccessLayer
{

    public class UserDTO
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string PasswordHash { get; set; }
        public string Role { get; set; } 

        public UserDTO(int id, string username, string passwordHash, string role)
        {
            this.Id = id;
            this.Username = username;
            this.PasswordHash = passwordHash;
            this.Role = role;
        }
    }


    public class UserData
    {

        public static UserDTO GetUserByUsername(string username)
        {
            using (SqlConnection connection = new SqlConnection(DatabaseSettings.ConnectionString))
            {
                string query = "SELECT Id, Username, PasswordHash, Role FROM Users WHERE Username = @Username";

                using (SqlCommand comm = new SqlCommand(query, connection))
                {

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


        public static int AddUser(UserDTO userDTO)
        {
            using (SqlConnection connection = new SqlConnection(DatabaseSettings.ConnectionString))
            {

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