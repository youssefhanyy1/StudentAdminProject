using studentDataAccessLayer;
using BusinessLogicLayer.Helpers; 

namespace BusinessLogicLayer
{
    public class Users
    {
        public enum enMode { AddNew = 0, Update = 1 };
        public enMode Mode = enMode.AddNew;

        public int Id { get; set; }
        public string Username { get; set; }
        public string PasswordHash { get; set; }
        public string Role { get; set; }

        public UserDTO UDTO
        {
            get { return new UserDTO(this.Id, this.Username, this.PasswordHash, this.Role); }
        }

        public Users()
        {
            this.Mode = enMode.AddNew;
        }

        private Users(UserDTO dto, enMode mode = enMode.AddNew)
        {
            this.Id = dto.Id;
            this.Username = dto.Username;
            this.PasswordHash = dto.PasswordHash;
            this.Role = dto.Role;
            this.Mode = mode;
        }


        public void SetPassword(string plainPassword)
        {
            if (!string.IsNullOrWhiteSpace(plainPassword))
            {

                this.PasswordHash = PasswordHasher.HashPassword(plainPassword);
            }
        }


        public bool VerifyPassword(string plainPassword)
        {
            if (string.IsNullOrEmpty(this.PasswordHash))
                return false;


            return PasswordHasher.VerifyPassword(plainPassword, this.PasswordHash);
        }

        public static Users Find(int id)
        {
            UserDTO dto = UserData.GetUserById(id);
            if (dto != null) return new Users(dto, enMode.Update);
            return null;
        }

        public static Users FindByUsername(string username)
        {
            UserDTO dto = UserData.GetUserByUsername(username);
            if (dto != null) return new Users(dto, enMode.Update);
            return null;
        }

        public bool Save()
        {
            switch (Mode)
            {
                case enMode.AddNew:
                    this.Id = UserData.AddUser(this.UDTO);
                    if (this.Id > 0)
                    {
                        Mode = enMode.Update;
                        return true;
                    }
                    return false;

                case enMode.Update:
                    return UserData.UpdateUser(this.UDTO);
            }
            return false;
        }

        public static bool DeleteUser(int id)
        {
            return UserData.DeleteUser(id);
        }
    }
}