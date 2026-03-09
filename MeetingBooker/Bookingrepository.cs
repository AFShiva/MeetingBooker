using System.Text.Json;

namespace MeetingBooker
{
    class BookingRepository
    {
        // Sti til JSON-filen, der bruges til lagring
        private readonly string _dataFile;
        // Intern liste over aktive bookinger i hukommelsen
        private List<Booking> _bookings;
        // Viser bookinger som skrivebeskyttet for at forhindre ekstern ændring
        public IReadOnlyList<Booking> All => _bookings.AsReadOnly();
        // Constructor: initialiserer repository med filsti og en tom bookingliste
        public BookingRepository(string dataFile = "bookings.json")
        {
            _dataFile = dataFile;
            _bookings = new List<Booking>();
        }
        // Indlæser bookinger fra JSON-filen.
        // Fjerner automatisk udløbne møder og gemmer den rensede liste tilbage.
        public void Load()
        {
            if (File.Exists(_dataFile))
            {
                string json = File.ReadAllText(_dataFile);
                _bookings = JsonSerializer.Deserialize<List<Booking>>(json)
                            ?? new List<Booking>();
            }
            // Fjern møder, der allerede er afsluttet
            RemoveExpired();
            // Gem den rensede liste tilbage til filen
            Save();
        }
        // Tilføjer en ny booking til listen og gemmer til fil
        public void Add(Booking booking)
        {
            _bookings.Add(booking);
            Save();
        }
        // Fjerner en eksisterende booking fra listen og gemmer til fil
        public void Remove(Booking booking)
        {
            _bookings.Remove(booking);
            Save();
        }
        // Tjekker om der findes en eksisterende booking, der overlapper med det angivne rum og tidsrum
        public bool HasConflict(int roomId, DateTime start, float durationHours)
        {
            return _bookings.Any(b => b.ConflictsWith(roomId, start, durationHours));
        }
        // Returnerer alle bookinger sorteret efter starttidspunkt
        public List<Booking> GetSortedByStart()
        {
            return _bookings.OrderBy(b => b.Start).ToList();
        }
        // Returnerer bookinger grupperet efter mødelokalets ID, sorteret stigende
        public IEnumerable<IGrouping<int, Booking>> GetGroupedByRoom()
        {
            return _bookings.GroupBy(b => b.Room).OrderBy(g => g.Key);
        }
        // Fjerner alle bookinger, hvis sluttidspunkt er passeret
        private void RemoveExpired()
        {
            _bookings = _bookings.Where(b => !b.IsExpired).ToList();
        }
        // Serialiserer og gemmer den aktuelle bookingliste som JSON til filen
        private void Save()
        {
            var options = new JsonSerializerOptions { WriteIndented = true };
            string json = JsonSerializer.Serialize(_bookings, options);
            File.WriteAllText(_dataFile, json);
        }
    }
}