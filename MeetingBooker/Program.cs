namespace MeetingBooker
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var repository = new BookingRepository("bookings.json");
            repository.Load();

            var service = new BookingService(repository);
            var ui = new ConsoleUI(service);

            User currentUser = ui.SelectUser();
            ui.ShowMainMenu(currentUser);

            Console.ReadLine();
        }
    }
}