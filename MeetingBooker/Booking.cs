namespace MeetingBooker
{
    class Booking
    {
        // Navn på brugeren, der oprettede bookingen
        public string User { get; set; }
        // ID af meeting room
        public int Room { get; set; }
        // Mødets starttidspunkt
        public DateTime Start { get; set; }
        // Mødets varighed i timer
        public float Duration { get; set; }
        // Beregner mødets sluttidspunkt ud fra start og varighed
        public DateTime End => Start.AddHours(Duration);
        // Returnerer true, hvis mødet allerede er afsluttet (sluttidspunkt er passeret)
        public bool IsExpired => End <= DateTime.Now;
        // Tjekker om en ny booking overlapper med dette møde i samme lokale.
        // To møder overlapper, hvis det ene starter før det andet er slut.
        public bool ConflictsWith(int roomId, DateTime start, float durationHours)
        {
            if (Room != roomId) return false;

            DateTime newEnd = start.AddHours(durationHours);
            return start < End && Start < newEnd;
        }
        // Styrer hvordan en booking vises som tekst, f.eks. i annulleringsmenuen
        public override string ToString()
        {
            return $"Room {Room} | {Start:yyyy-MM-dd HH:mm} - {End:HH:mm} ({User})";
        }
    }
}