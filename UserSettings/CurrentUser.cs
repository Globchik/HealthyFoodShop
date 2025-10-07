namespace HealthyFood_Shop
{
    public static class CurrentUser
    {
        public static Classes.User current_user = new Classes.User();
        static public bool Authenticate(in string login, in string pass)
        {
            return SQLite_Interaction.LogIntoUser(login, pass, out current_user);
        }

        public static void LogOut()
        {
            current_user = new Classes.User();
        }
    }
}
