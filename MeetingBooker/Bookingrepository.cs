using System.Text.Json;

namespace MeetingBooker
{
    class BookingRepository
    {
        private readonly string _dataFile;
        private List<Booking> _bookings;

        public IReadOnlyList<Booking> All => _bookings.AsReadOnly();

        public BookingRepository(string dataFile = "bookings.json")
        {
            _dataFile = dataFile;
            _bookings = new List<Booking>();
        }

        public void Load()
        {
            if (File.Exists(_dataFile))
            {
                string json = File.ReadAllText(_dataFile);
                _bookings = JsonSerializer.Deserialize<List<Booking>>(json)
                            ?? new List<Booking>();
            }

            RemoveExpired();
            Save();
        }

        public void Add(Booking booking)
        {
            _bookings.Add(booking);
            Save();
        }

        public void Remove(Booking booking)
        {
            _bookings.Remove(booking);
            Save();
        }

        public bool HasConflict(int roomId, DateTime start, float durationHours)
        {
            return _bookings.Any(b => b.ConflictsWith(roomId, start, durationHours));
        }

        public List<Booking> GetSortedByStart()
        {
            return _bookings.OrderBy(b => b.Start).ToList();
        }

        public IEnumerable<IGrouping<int, Booking>> GetGroupedByRoom()
        {
            return _bookings.GroupBy(b => b.Room).OrderBy(g => g.Key);
        }

        private void RemoveExpired()
        {
            _bookings = _bookings.Where(b => !b.IsExpired).ToList();
        }

        private void Save()
        {
            var options = new JsonSerializerOptions { WriteIndented = true };
            string json = JsonSerializer.Serialize(_bookings, options);
            File.WriteAllText(_dataFile, json);
        }
    }
}