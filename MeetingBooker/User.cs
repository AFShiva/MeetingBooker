namespace MeetingBooker
{
    class User
    {
        public string Name { get; }

        public User(string name)
        {
            Name = name;
        }

        public override string ToString() => Name;

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