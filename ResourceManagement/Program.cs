using ResourceManagement.Data;
using ResourceManagement.Models;

class Program
{
    static void Main(string[] args)
    {
        var dataStorage = new DataStorage();
        var shortageManager = new ShortageManager(dataStorage);

        var user = new User { Name = "Test User", IsAdmin = true };

        while (true)
        {
            Console.WriteLine("Enter command:(register, delete, list, filter)");
            var command = Console.ReadLine();

            if (command == "register")
            {
                Console.WriteLine("Enter shortage details (title, name, room(1-meetingroom,2-kitchen,3-bathroom), category(1-electronics,2-food,3-other), priority(1-10)):");
                var details = Console.ReadLine().Split(',');

                if (details.Length != 5)
                {
                    Console.WriteLine("Invalid syntax. Please enter 5 values separated by commas.");
                    continue;
                }

                var title = details[0].Trim();
                var name = details[1].Trim();

                if (string.IsNullOrWhiteSpace(title) || int.TryParse(title, out _))
                {
                    Console.WriteLine("Invalid title. Title cannot be empty or a number.");
                    continue;
                }
                
                if (string.IsNullOrWhiteSpace(name) || int.TryParse(name, out _))
                {
                    Console.WriteLine("Invalid name. Name cannot be empty or a number.");
                    continue;
                }

                if (!int.TryParse(details[2].Trim(), out int roomInput) || roomInput < 1 || roomInput > 3 || !Enum.IsDefined(typeof(RoomType), roomInput - 1))
                {
                    Console.WriteLine("Invalid room. Please enter a number 1-MeetingRoom, 2-Kitchen, 3-Bathroom.");
                    continue;
                }

                if (!int.TryParse(details[3].Trim(), out int categoryInput) || categoryInput < 1 || categoryInput > 3 || !Enum.IsDefined(typeof(CategoryType), categoryInput - 1))
                {
                    Console.WriteLine("Invalid category. Please enter a number 1-Electronics, 2-Food, 3-Other.");
                    continue;
                }

                var roomType = (RoomType)(roomInput - 1);
                var categoryType = (CategoryType)(categoryInput - 1);

                if (!int.TryParse(details[4].Trim(), out int priority) || priority < 1 || priority > 10)
                {
                    Console.WriteLine("Invalid priority. Please enter a number between 1 and 10.");
                    continue;
                }

                var shortage = new Shortage
                {
                    Title = title,
                    Name = name,
                    Room = roomType,
                    Category = categoryType,
                    Priority = priority,
                    CreatedOn = DateTime.Now,
                    CreatedBy = user.Name
                };

                shortageManager.AddShortage(shortage);
            }
            else if (command == "delete")
            {
                Console.WriteLine("Enter shortage title, room:");
                var details = Console.ReadLine().Split(',');

                if (details.Length != 2)
                {
                    Console.WriteLine("Invalid syntax. Please enter 2 values separated by commas.");
                    continue;
                }

                shortageManager.DeleteShortage(details[0].Trim(), details[1].Trim(), user);
            }
            else if (command == "list")
            {
                var shortages = shortageManager.GetAllShortages(user);
                foreach (var shortage in shortages)
                {
                    Console.WriteLine($"Title: {shortage.Title}, Name: {shortage.Name}, Room: {shortage.Room}, Category: {shortage.Category}, Priority: {shortage.Priority}, CreatedOn: {shortage.CreatedOn}, CreatedBy: {shortage.CreatedBy}");
                }
            }
            else if (command == "filter")
            {
                Console.WriteLine("Enter filter type (title, date, room, category):");
                var filterType = Console.ReadLine();
                List<Shortage> filteredShortages = null;

                if (filterType == "title")
                {
                    Console.WriteLine("Enter title:");
                    var titleFilter = Console.ReadLine();
                    if (string.IsNullOrWhiteSpace(titleFilter))
                    {
                        Console.WriteLine("Invalid title. Title cannot be empty.");
                        continue;
                    }
                    filteredShortages = shortageManager.GetAllShortages(user, titleFilter);

                    if (!filteredShortages.Any())
                    {
                        Console.WriteLine("No shortages found with this title.");
                        continue;
                    }
                }
                else if (filterType == "date")
                {
                    Console.WriteLine("Enter start date (yyyy-mm-dd):");
                    var startDateInput = Console.ReadLine();

                    if (!DateTime.TryParse(startDateInput, out var startDate))
                    {
                        Console.WriteLine("Invalid date format.");
                        continue;
                    }

                    Console.WriteLine("Enter end date (yyyy-mm-dd):");
                    var endDateInput = Console.ReadLine();

                    if (!DateTime.TryParse(endDateInput, out var endDate))
                    {
                        Console.WriteLine("Invalid date format.");
                        continue;
                    }

                    filteredShortages = shortageManager.GetAllShortages(user, null, startDate, endDate);
                }
                else if (filterType == "room")
                {
                    Console.WriteLine("Enter room (1-3):");
                    var roomInput = Console.ReadLine();

                    if (!int.TryParse(roomInput, out int roomValue) || roomValue < 1 || roomValue > 3)
                    {
                        Console.WriteLine("Invalid room. Please enter a number between 1 and 3.");
                        continue;
                    }

                    filteredShortages = shortageManager.GetAllShortages(user, null, null, null, (RoomType)(roomValue - 1));
                }
                else if (filterType == "category")
                {
                    Console.WriteLine("Enter category (1-3):");
                    var categoryInput = Console.ReadLine();

                    if (!int.TryParse(categoryInput, out int categoryValue) || categoryValue < 1 || categoryValue > 3)
                    {
                        Console.WriteLine("Invalid category. Please enter a number between 1 and 3.");
                        continue;
                    }

                    filteredShortages = shortageManager.GetAllShortages(user, null, null, null, null, (CategoryType)(categoryValue - 1));
                }
                else
                {
                    Console.WriteLine("Invalid filter type.");
                    continue;
                }

                // Display shortages
                foreach (var shortage in filteredShortages)
                {
                    Console.WriteLine($"Title: {shortage.Title}, Name: {shortage.Name}, Room: {shortage.Room}, Category: {shortage.Category}, Priority: {shortage.Priority}, CreatedOn: {shortage.CreatedOn}, CreatedBy: {shortage.CreatedBy}");
                }
            }
            else if (command == "exit")
            {
                break;
            }
            else
            {
                Console.WriteLine("Invalid command.");
            }
        }
    }
}