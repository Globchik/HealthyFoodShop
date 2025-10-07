namespace HealthyFood_Shop.Classes
{
    public class User
    {
        public System.Int64? Id { get; set; }
        public string Login { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string PIB { get; private set; }
        public string PhoneNumber { get; set; }
        public string FK_Position { get; set; }
        public bool IsAdmin { get; set; }

        public User()
        {
            Id = null;
            Login = string.Empty;
            PIB = string.Empty;
            PhoneNumber = string.Empty;
            FK_Position = string.Empty;
            IsAdmin = false;
            Password = string.Empty;
        }

        public User(System.Int64 id, in string pIB, in string phoneNumber, in string position, bool isAdmin)
        {
            Id = id;
            PIB = pIB;
            PhoneNumber = phoneNumber;
            FK_Position = position;
            IsAdmin = isAdmin;
        }
        public User(System.Int64 id, in string login, in string password, in string pIB, in string phoneNumber, in string position, bool isAdmin)
        {
            Id = id;
            Login = login;
            Password = password;
            PIB = pIB;
            PhoneNumber = phoneNumber;
            FK_Position = position;
            IsAdmin = isAdmin;
        }
    }
}
