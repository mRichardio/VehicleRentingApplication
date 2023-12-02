using System.Transactions;
using System.Linq;
using System.Reflection.Emit;
using System;

namespace VehicleRentingApplication
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // TODO

            // - Use Parallel Execution (If I use this then I need to explain why I have used it.
            // E.g. I use single thread and got a response time slower than when using parallel execution.))

            // - JSON Serialisation for all lists e.g vehicles need to be stored in json files along with customers
            // These lists should be dumped and loaded to and from the txt file each time a new process is done e.g a new customer is added.

            // Better hierarchy {In Progress}

            // Hello laptop!

            int selected; // For menu selection
            Customer currentUser = null;
            
            Dictionary<int, Users> users = new Dictionary<int, Users>(); // Stores all of the users of the system e.g. Staff, Customers
            AddUser(new Customer("Matthew", "Richards")); // REMOVE AFTER TESTING

            Dictionary<int, Vehicle> vehicles = new Dictionary<int, Vehicle>(); // Stores all the vehicles e.g. Cars, Motorbikes
            AddVehicle(new Car(2002, 4, 5, true, new Colour(255, 255, 255), new Registration("BD51 SMR"), "Audi", "A3")); // REMOVE AFTER TESTING
            AddVehicle(new Car(2015, 4, 5, false, new Colour(255, 0, 255), new Registration("RG38 KJE"), "Nissan", "GTR")); // REMOVE AFTER TESTING
            AddVehicle(new Car(2022, 4, 3, true, new Colour(255, 0, 0), new Registration("JU24 OPE"), "Ford", "Focus")); // REMOVE AFTER TESTING

            // Program
            while (true)
            {
                Menu mainMenu = new Menu(new string[] { "View Cars", "Your Cars", "View Profile" });
                mainMenu.DisplayMenu();
                try { selected = Convert.ToInt32(Console.ReadLine()); }
                catch (Exception e)
                {
                    Console.Clear(); 
                    Console.WriteLine($"[Error]: {e.Message}\n"); // NEEDS CHANGING TO A FRIENDLIER MESSAGE
                    continue;
                }

                switch (selected)
                {
                    case 1:
                        Console.Clear();
                        Console.WriteLine("You entered 1.");
                        break;

                    case 2:
                        Console.Clear();
                        Console.WriteLine("You entered 2.");
                        break;

                    case 3:
                        Console.Clear();
                        Console.WriteLine("You entered 3.");
                        break;

                    default:
                        Console.Clear();
                        Console.WriteLine("Invalid input. Please enter a number between 1 and 3.");
                        break;
                }
            }

            // Functions

            void AddUser(Users user) { users.Add(users.Count + 1, user); }

            void AddVehicle(Vehicle vehicle) { vehicles.Add(vehicles.Count + 1, vehicle); }

            // return users.Exists(customer => customer.username == user && customer.password == pass); // Good for finding people!

            // This is prob gonna need a rework !!!!
            // I decided to create this function that returns the users choice back as it allows for less code duplication as this function will work with any menu.
            //int DisplayMenu(List<Menu> options)
            //{
            //    int choice;
            //    while (true)
            //    {
            //        Console.Clear();

            //        Console.WriteLine("--- Main Menu ---\n");

            //        if (currentCustomer != null) { Console.WriteLine($"[ Name: {currentCustomer.firstName} {currentCustomer.lastName} | Credits: {currentCustomer.credits} | Rented Vehicles: {currentCustomer.vehicleCount}/{currentCustomer.rentLimit} ]\n"); }

            //        foreach (var option in options) { Console.WriteLine($"{option.optionID}. {option.optionInfo}"); }

            //        Console.WriteLine("\nSelect an option from this menu...");
            //        choice = int.Parse(Console.ReadLine());

            //        if (options.Exists(option => option.optionID == choice)) { return choice; }
            //        else { Console.WriteLine("Invalid choice. Please enter a valid option from the list (e.g, '2' or '1')."); }
            //    }
            //} 
        }
    }
}