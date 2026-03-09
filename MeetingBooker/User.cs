namespace MeetingBooker
{
    class User
    {
        // userns navn
        public string Name { get; }
        // Constructor: opretter et nyt User-objekt med det givne navn
        public User(string name)
        {
            Name = name;
        }
        // Styrer hvordan User udskrives i konsollen
        public override string ToString() => Name;
        // Returnerer users, der er tilgængelige i systemet
        public static User[] GetDefaultUsers()
        {
            return new[]
            {
                new User("Sofie"),
                new User("Amir"),
                new User("Jonas"),
                new User("Louise"),
                new User("Mette"),
                new User("Henrik")
            };
        }
    }
}