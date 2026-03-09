namespace MeetingBooker
{
    class BookingService
    {
        // Repository brugt til at gemme og hente bookinger
        private readonly BookingRepository _repository;
        // Constructor: modtager repository
        public BookingService(BookingRepository repository)
        {
            _repository = repository;
        }

        // Forsøger at gemme ny booking imens der valideres for business rules
        // Returnerer true hvis booking lykkedes, ellers false med en fejlbesked i 'error'.
        public bool TryBook(User user, MeetingRoom room, DateTime start, float duration, out string error)
        {
            error = string.Empty;
            // Mødet skal ligge i fremtiden
            if (start <= DateTime.Now)
            {
                error = "Date and time must be in the future.";
                return false;
            }

            // Mødet skal starte og slutte inden for kontorets working hours
            if (!room.IsWithinWorkHours(start, duration))
            {
                error = "Meeting must start between 08:00 and 18:00 and end by 18:00.";
                return false;
            }

            // Rummet må ikke allerede være booket i det angivne tidsrum
            if (_repository.HasConflict(room.Id, start, duration))
            {
                error = "The room is already reserved during this time.";
                return false;
            }

            // Alle valideringer bestået.. opret og gem bookingen
            _repository.Add(new Booking
            {
                User = user.Name,
                Room = room.Id,
                Start = start,
                Duration = duration
            });

            return true;
        }
        // Forsøger at annullere en booking.
        // Kun brugeren, der oprettede bookingen, kan annullere den.
        // Returnerer true hvis annullering lykkedes, ellers false med fejlbesked i 'error'.
        public bool TryCancelBooking(User user, Booking booking, out string error)
        {
            error = string.Empty;
            // Tjek at den aktuelle bruger er ejeren af bookingen
            if (booking.User != user.Name)
            {
                error = "You can only cancel your own meetings.";
                return false;
            }

            _repository.Remove(booking);
            return true;
        }
        // Returnerer alle aktive bookinger som en skrivebeskyttet liste
        public IReadOnlyList<Booking> GetAllBookings() => _repository.All;
        // Returnerer alle bookinger sorteret efter starttidspunkt
        public List<Booking> GetBookingsSortedByStart() => _repository.GetSortedByStart();
        // Returnerer bookinger grupperet efter mødelokale (til visning i UI)
        public IEnumerable<IGrouping<int, Booking>> GetBookingsGroupedByRoom() => _repository.GetGroupedByRoom();
    }
}