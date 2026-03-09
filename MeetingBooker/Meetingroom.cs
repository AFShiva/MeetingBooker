namespace MeetingBooker
{
    class MeetingRoom
    {
        // Unikt ID for rummet
        public int Id { get; }
        // Display-navn brugt i UI, f.eks. "Meeting Room 1"
        public string Name => $"Meeting Room {Id}";
        // Kontorarbejdstid: møder må kun bookes mellem 08:00 og 18:00
        public static readonly TimeSpan WorkStart = new TimeSpan(8, 0, 0);
        public static readonly TimeSpan WorkEnd = new TimeSpan(18, 0, 0);
        // Constructor: opretter et mødelokale med det givne ID
        public MeetingRoom(int id)
        {
            Id = id;
        }
        // Opretter et array med de foruddefinerede mødelokaler (standard: 3 rum)
        public static MeetingRoom[] GetAllRooms(int count = 3)
        {
            var rooms = new MeetingRoom[count];
            for (int i = 0; i < count; i++)
                rooms[i] = new MeetingRoom(i + 1);
            return rooms;
        }
        // Tjekker om et møde starter og slutter inden for kontorarbejdstiden (08:00–18:00)
        public bool IsWithinWorkHours(DateTime start, float durationHours)
        {
            return start.TimeOfDay >= WorkStart &&
                   start.TimeOfDay < WorkEnd &&
                   start.AddHours(durationHours).TimeOfDay <= WorkEnd;
        }
        // Styrer hvordan MeetingRoom udskrives i konsollen (returnerer Name)
        public override string ToString() => Name;
    }
}