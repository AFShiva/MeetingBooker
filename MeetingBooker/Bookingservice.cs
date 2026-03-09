namespace MeetingBooker
{
    class BookingService
    {
        private readonly BookingRepository _repository;

        public BookingService(BookingRepository repository)
        {
            _repository = repository;
        }

        public bool TryBook(User user, MeetingRoom room, DateTime start, float duration, out string error)
        {
            error = string.Empty;

            if (start <= DateTime.Now)
            {
                error = "Date and time must be in the future.";
                return false;
            }

            if (!room.IsWithinWorkHours(start, duration))
            {
                error = "Meeting must start between 08:00 and 18:00 and end by 18:00.";
                return false;
            }

            if (_repository.HasConflict(room.Id, start, duration))
            {
                error = "The room is already reserved during this time.";
                return false;
            }

            _repository.Add(new Booking
            {
                User = user.Name,
                Room = room.Id,
                Start = start,
                Duration = duration
            });

            return true;
        }

        public bool TryCancelBooking(User user, Booking booking, out string error)
        {
            error = string.Empty;

            if (booking.User != user.Name)
            {
                error = "You can only cancel your own meetings.";
                return false;
            }

            _repository.Remove(booking);
            return true;
        }

        public IReadOnlyList<Booking> GetAllBookings() => _repository.All;

        public List<Booking> GetBookingsSortedByStart() => _repository.GetSortedByStart();

        public IEnumerable<IGrouping<int, Booking>> GetBookingsGroupedByRoom() => _repository.GetGroupedByRoom();
    }
}