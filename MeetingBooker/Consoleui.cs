namespace MeetingBooker
{
    class ConsoleUI
    {
        // Reference til business logik-laget, brugt til booking-operationer
        private readonly BookingService _service;
        // Tilgængelig meeting rooms
        private readonly MeetingRoom[] _rooms;
        // Konstanter brugt i UI header
        private const string AppTitle = "########## Meeting Booker ##########";
        private const string OfficeName = "Kontor Nord";
        // Constructor: modtager service og henter alle tilgængelige mødelokaler
        public ConsoleUI(BookingService service)
        {
            _service = service;
            _rooms = MeetingRoom.GetAllRooms();
        }

        ////////////////// Header //////////////////
        // Udskriver header, kan anvendes så vi ikke gentager kode
        // Rydder konsollen og udskriver app-titlen.
        public void PrintHeader(User user = null)
        {
            Console.Clear();
            Console.WriteLine(AppTitle);

            if (user != null)
                Console.WriteLine($"Welcome {user.Name}!");
        }

        // Input helpers
        // Sikrer at brugeren indtaster et gyldigt tal inden for et givet interval
        public int GetValidatedNumber(int min, int max)
        {
            while (true)
            {
                string input = Console.ReadLine();
                if (int.TryParse(input, out int number) && number >= min && number <= max)
                    return number;

                Console.Write($"Please enter a number between {min} and {max}: ");
            }
        }
        // Viser login-skærmen med en nummereret liste over tilgængelige brugere.
        // Returnerer den bruger, som blev valgt.
        public User SelectUser()
        {
            User[] users = User.GetDefaultUsers();

            Console.Clear();
            Console.WriteLine(AppTitle);
            Console.WriteLine(OfficeName);
            Console.WriteLine("Please choose your login:");

            for (int i = 0; i < users.Length; i++)
                Console.WriteLine($"{i + 1}. {users[i].Name}");

            Console.Write("Enter number: ");
            int choice = GetValidatedNumber(1, users.Length);
            return users[choice - 1];
        }

        ////////////////// Main menu //////////////////
        // Viser hovedmenuen og kalder den relevante metode baseret på brugerens valg.
        public void ShowMainMenu(User user)
        {
            PrintHeader(user);
            Console.WriteLine("Please select an option:");
            Console.WriteLine("1. Book meeting");
            Console.WriteLine("2. View bookings");
            Console.WriteLine("3. Cancel meeting");
            Console.WriteLine("4. Close");
            Console.Write("Enter number: ");

            int choice = GetValidatedNumber(1, 4);

            switch (choice)
            {
                case 1: ShowBookMeeting(user); break;
                case 2: ShowViewBookings(user); break;
                case 3: ShowCancelMeeting(user); break;
                case 4: Console.WriteLine("Closing program..."); break;
            }
        }

        ////////////////// Book meeting //////////////////
        // Viser en liste over tilgængelige mødelokaler og lader brugeren vælge ét.
        public void ShowBookMeeting(User user)
        {
            PrintHeader(user);
            Console.WriteLine("Please select the meeting room for booking:");

            for (int i = 0; i < _rooms.Length; i++)
                Console.WriteLine($"{i + 1}. {_rooms[i].Name}");

            Console.WriteLine($"{_rooms.Length + 1}. Go back");
            Console.Write("Enter number: ");

            int choice = GetValidatedNumber(1, _rooms.Length + 1);

            if (choice == _rooms.Length + 1)
            {
                ShowMainMenu(user);
                return;
            }
            // Videresend til booking-skærmen med det valgte lokale (ingen dato/varighed endnu)
            ShowBookRoom(user, _rooms[choice - 1], null, null);
        }

        // Håndterer den trinvise bookingproces for et specifikt mødelokale.
        private void ShowBookRoom(User user, MeetingRoom room, DateTime? date, float? duration)
        {
            PrintHeader(user);
            Console.WriteLine($"Selected room: {room.Name}");
            Console.WriteLine(date.HasValue
                ? $"Selected date and start time: {date:yyyy-MM-dd HH:mm}"
                : "1. Input date");

            Console.WriteLine(duration.HasValue
                ? $"Selected duration: {duration} hours"
                : "2. Input duration in hours");

            // "Confirm"-knappen vises kun, når både dato og varighed er valgt
            bool canConfirm = date.HasValue && duration.HasValue;
            int maxOption;

            if (!canConfirm)
            {
                Console.WriteLine("3. Go back");
                maxOption = 3;
            }
            else
            {
                Console.WriteLine("3. Confirm booking");
                Console.WriteLine("4. Go back");
                maxOption = 4;
            }

            Console.Write("Enter number: ");
            int choice = GetValidatedNumber(1, maxOption);
            // Bruger ønsker at indtaste dato
            if (choice == 1 && !date.HasValue)
            {
                date = PromptDate();
                ShowBookRoom(user, room, date, duration);
                return;
            }
            // Bruger ønsker at indtaste varighed
            if (choice == 2 && !duration.HasValue)
            {
                duration = PromptDuration(date);
                ShowBookRoom(user, room, date, duration);
                return;
            }
            // Bruger ønsker at gå tilbage

            if (choice == 3 && !canConfirm)
            {
                ShowBookMeeting(user);
                return;
            }
            // Bruger bekræfter booking – forsøg at gemme via service
            if (choice == 3 && canConfirm)
            {
                if (_service.TryBook(user, room, date.Value, duration.Value, out string error))
                {
                    // Booking lykkedes – vis bekræftelse
                    Console.WriteLine("Meeting booked and saved!");
                    Console.WriteLine($"Room: {room.Name}");
                    Console.WriteLine($"Start: {date:yyyy-MM-dd HH:mm}");
                    Console.WriteLine($"Duration: {duration} hours");
                }
                else
                {
                    // Booking mislykkedes (f.eks. konflikt) – vis fejl og lad brugeren prøve igen
                    Console.WriteLine($"\nBooking failed: {error}");
                    Console.WriteLine("Press any key to choose a different time...");
                    Console.ReadKey();
                    ShowBookRoom(user, room, null, null);
                    return;
                }

                Console.ReadKey();
                ShowMainMenu(user);
                return;
            }
            // Bruger vælger "Go back" efter at have udfyldt alle felter
            if (choice == 4)
            {
                ShowBookMeeting(user);
            }
        }
        // Beder brugeren om at indtaste en dato og starttidspunkt i formatet yyyy-MM-dd HH:mm.
        // Validerer at datoen er i fremtiden og ligger inden for kontorarbejdstiden.
        private DateTime? PromptDate()
        {
            while (true)
            {
                Console.Write("Enter date and start time (yyyy-MM-dd HH:mm): ");
                string input = Console.ReadLine();
                // Tjek at formatet er korrekt
                if (!DateTime.TryParseExact(input, "yyyy-MM-dd HH:mm", null,
                        System.Globalization.DateTimeStyles.None, out DateTime parsed))
                {
                    Console.WriteLine("Invalid format. Please use yyyy-MM-dd HH:mm.");
                    continue;
                }
                // Mødet skal ligge i fremtiden
                if (parsed <= DateTime.Now)
                {
                    Console.WriteLine("Date and time must be in the future.");
                    continue;
                }
                // Mødet skal starte inden for working hours

                if (parsed.TimeOfDay < MeetingRoom.WorkStart || parsed.TimeOfDay >= MeetingRoom.WorkEnd)
                {
                    Console.WriteLine("Meetings must start between 08:00 and 18:00.");
                    continue;
                }

                return parsed;
            }
        }
        // Beder brugeren om at indtaste mødets varighed i timer.
        // Validerer at varigheden er positiv, max 12 timer, og at mødet slutter inden 18:00.
        private float? PromptDuration(DateTime? date)
        {
            while (true)
            {
                Console.Write("Enter duration in hours: ");
                string input = Console.ReadLine();
                // Tjek at input kan parses som et decimaltal
                if (!float.TryParse(input, out float parsed))
                {
                    Console.WriteLine("Invalid number. Please enter a valid duration.");
                    continue;
                }
                // Varighed skal være positiv og højst 12 timer
                if (parsed <= 0 || parsed > 12)
                {
                    Console.WriteLine("Duration must be between 0 and 12 hours.");
                    continue;
                }
                // Hvis dato er valgt, tjek at mødet slutter inden working hours
                if (date.HasValue && date.Value.AddHours(parsed).TimeOfDay > MeetingRoom.WorkEnd)
                {
                    Console.WriteLine("Meeting would end after office hours (18:00). Choose shorter duration.");
                    continue;
                }

                return parsed;
            }
        }

        ////////////////// View bookings //////////////////
        // Viser alle aktive bookinger grupperet efter mødelokale og sorteret efter starttidspunkt.
        public void ShowViewBookings(User user)
        {
            PrintHeader(user);
            Console.WriteLine("Showing reserved bookings by room:");
            // Tjek om der overhovedet er nogen bookinger
            if (!_service.GetAllBookings().Any())
            {
                Console.WriteLine("No bookings found.");
                Console.ReadKey();
                ShowMainMenu(user);
                return;
            }
            // Udskriv bookinger grupperet pr. lokale, sorteret efter starttidspunkt
            foreach (var roomGroup in _service.GetBookingsGroupedByRoom())
            {
                Console.WriteLine($"\nRoom {roomGroup.Key}");

                foreach (var booking in roomGroup.OrderBy(b => b.Start))
                    Console.WriteLine($"  {booking.Start:yyyy-MM-dd HH:mm} - {booking.End:HH:mm} ({booking.User})");
            }

            Console.WriteLine("\nPress any key to return...");
            Console.ReadKey();
            ShowMainMenu(user);
        }

        ////////////////// Cancel meeting //////////////////
        // Viser en liste over alle bookinger og lader brugeren vælge én at annullere.
        public void ShowCancelMeeting(User user)
        {
            PrintHeader(user);
            Console.WriteLine("Select a meeting to cancel:\n");
            // Hent alle bookinger sorteret efter starttidspunkt
            var sorted = _service.GetBookingsSortedByStart();
            // Hvis ingen bookinger findes, gå tilbage til hovedmenuen
            if (!sorted.Any())
            {
                Console.WriteLine("No bookings available.");
                Console.ReadKey();
                ShowMainMenu(user);
                return;
            }
            // Vis nummereret liste over bookinger
            for (int i = 0; i < sorted.Count; i++)
                Console.WriteLine($"{i + 1}. {sorted[i]}");

            Console.WriteLine($"{sorted.Count + 1}. Go back");
            Console.Write("\nEnter number: ");

            int choice = GetValidatedNumber(1, sorted.Count + 1);
            // Bruger valgte "Go back"
            if (choice == sorted.Count + 1)
            {
                ShowMainMenu(user);
                return;
            }

            Booking selected = sorted[choice - 1];
            // Forsøg at annullere den valgte booking via service
            if (_service.TryCancelBooking(user, selected, out string error))
            {
                Console.WriteLine("\nMeeting cancelled successfully.");
            }
            else
            {
                // Annullering mislykkedes (f.eks. ikke ejeren) – vis fejl og prøv igen
                Console.WriteLine($"\n{error}");
                Console.WriteLine("Press any key to continue...");
                Console.ReadKey();
                ShowCancelMeeting(user);
                return;
            }

            Console.ReadKey();
            ShowMainMenu(user);
        }
    }
}