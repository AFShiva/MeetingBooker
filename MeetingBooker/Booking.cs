namespace MeetingBooker
{
    class Booking
    {
        public string User { get; set; }
        public int Room { get; set; }
        public DateTime Start { get; set; }
        public float Duration { get; set; }

        public DateTime End => Start.AddHours(Duration);
        public bool IsExpired => End <= DateTime.Now;

        public bool ConflictsWith(int roomId, DateTime start, float durationHours)
        {
            if (Room != roomId) return false;

            DateTime newEnd = start.AddHours(durationHours);
            return start < End && Start < newEnd;
        }

        public override string ToString()
        {
            return $"Room {Room} | {Start:yyyy-MM-dd HH:mm} - {End:HH:mm} ({User})";
        }
    }
}