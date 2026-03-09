namespace MeetingBooker
{
    class MeetingRoom
    {
        public int Id { get; }
        public string Name => $"Meeting Room {Id}";

        public static readonly TimeSpan WorkStart = new TimeSpan(8, 0, 0);
        public static readonly TimeSpan WorkEnd = new TimeSpan(18, 0, 0);

        public MeetingRoom(int id)
        {
            Id = id;
        }

        public static MeetingRoom[] GetAllRooms(int count = 3)
        {
            var rooms = new MeetingRoom[count];
            for (int i = 0; i < count; i++)
                rooms[i] = new MeetingRoom(i + 1);
            return rooms;
        }

        public bool IsWithinWorkHours(DateTime start, float durationHours)
        {
            return start.TimeOfDay >= WorkStart &&
                   start.TimeOfDay < WorkEnd &&
                   start.AddHours(durationHours).TimeOfDay <= WorkEnd;
        }

        public override string ToString() => Name;
    }
}