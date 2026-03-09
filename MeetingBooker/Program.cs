namespace MeetingBooker
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // Opret et repository, der er ansvarlig for at indlæse/gemme bookinger i en JSON-fil
            var repository = new BookingRepository("bookings.json");
            // Indlæs eksisterende bookinger fra lageret og fjern udløbne møder
            repository.Load();

            // Opret servicelag med business logik (booking, validering, annullering osv.)
            var service = new BookingService(repository);
            // Opret et UI-lag, der håndterer al konsol-interaktion
            var ui = new ConsoleUI(service);
            // Lad brugeren vælge, hvilken konto der skal logges ind som
            User currentUser = ui.SelectUser();
            // Start MainMenu-løkken for den valgte bruger
            ui.ShowMainMenu(currentUser);

            Console.ReadLine();
        }
    }
}