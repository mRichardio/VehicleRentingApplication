using System.Transactions;
using System.Linq;
using System.Reflection.Emit;
using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Diagnostics;

namespace VehicleRentingApplication
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // ---[ Main Quests ]---
            // - Use Parallel Execution (If I use this then I need to explain why I have used it.
            // E.g. I use single thread and got a response time slower than when using parallel execution.))

            // Better hierarchy {In Progress}

            // Ensure that when a new car is created, it is also wrote to the json file.

            // Add all read and write functionality into separate void functions!

            // Add remove vehicle functionality

            // Add an interface.

            // Create user read/write from json file. (Need to think of a way for the system to recognise a new user. MAYBE UNIQUE CODE??)

            // Start working on renting functionality (before a car is rented a unique id for the user should be used, to add up total rented count.)

            // ---[ Side Quests ]---
            //  Hide staff menu behind a staff password


            int selected; // For menu selection
            Customer currentUser = null;
            
            Dictionary<int, Users> users = new Dictionary<int, Users>(); // Stores all of the users of the system e.g. Staff, Customers

            Dictionary<string, Car> cars = new Dictionary<string, Car>();
            Dictionary<string, Truck> trucks = new Dictionary<string, Truck>();
            Dictionary<string, Motorbike> motorbikes = new Dictionary<string, Motorbike>();
            Dictionary<string, Vehicle> vehicles = new();

            AddUser(new Customer("Matthew", "Richards")); // REMOVE AFTER TESTING

            // Read

            // Cars Read

            try
            {
                string jsonReadCars = File.ReadAllText("cars.json");
                cars = JsonSerializer.Deserialize<Dictionary<string, Car>>(jsonReadCars);
                foreach (var car in cars) { vehicles.Add(car.Key, car.Value); }
            }
            catch (JsonException e) { Console.WriteLine("[ERROR]: No cars found..."); }

            try
            {
                // Trucks Read
                string jsonReadTrucks = File.ReadAllText("trucks.json");
                trucks = JsonSerializer.Deserialize<Dictionary<string, Truck>>(jsonReadTrucks);
                foreach (var truck in trucks) { vehicles.Add(truck.Key, truck.Value); }
            }
            catch (JsonException e) { Console.WriteLine("[ERROR]: No trucks found..."); }

            try
            {
                // Motorbikes Read
                string jsonReadMotorbikes = File.ReadAllText("motorbikes.json");
                motorbikes = JsonSerializer.Deserialize<Dictionary<string, Motorbike>>(jsonReadMotorbikes);
                foreach (var motorbike in motorbikes) { vehicles.Add(motorbike.Key, motorbike.Value); }
            }
            catch (JsonException e) { Console.WriteLine("[ERROR]: No motorbikes found..."); }


            //AddCar(new Car(2002, 4, 5, true, "Audi", "A3", new Colour(255, 255, 255), new Registration("BD51 SMR"))); // REMOVE AFTER TESTING
            //AddCar(new Car(2015, 4, 5, false, "Nissan", "GTR", new Colour(255, 0, 255), new Registration("RG38 KJE"))); // REMOVE AFTER TESTING
            //AddCar(new Car(2022, 4, 3, true, "Ford", "Focus", new Colour(255, 0, 0), new Registration("JU24 OPE"))); // REMOVE AFTER TESTING


            // Check if the deserialization was successful
            if (cars != null && motorbikes != null && trucks != null)
            {
                Console.WriteLine("Successfully loaded data!");
            }
            else
            {
                Console.WriteLine("Failed to deserialize JSON data.");
            }

            // Write

            // Cars Write
            string jsonWriteCars = JsonSerializer.Serialize(cars, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText("cars.json", jsonWriteCars);

            // Trucks Write
            string jsonWriteTrucks = JsonSerializer.Serialize(trucks, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText("trucks.json", jsonWriteTrucks);

            // Motorbikes Write
            string jsonWriteMotorbikes = JsonSerializer.Serialize(motorbikes, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText("motorbikes.json", jsonWriteMotorbikes);

            // Program
            while (true)
            {
                Menu mainMenu = new Menu(new string[] { "View Vehicles", "Your Vehicles", "View Profile", "Staff Menu" });
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
                        Console.WriteLine("---| Available Vehicles |---\n");

                        if (cars != null)
                        {
                            foreach (var kvp in vehicles)
                            {
                                string key = kvp.Key;
                                Vehicle vehicle = kvp.Value;
                                Console.WriteLine($"ID: {key}, Vehicle Type: {vehicle.type}\nYear: {vehicle.modelYear}\nManufacturer: {vehicle.manufacturer}\nModel: {vehicle.model}\n");
                            }
                        }
                        else
                        {
                            Console.WriteLine("They are currentlty no vehicles available to rent.");
                        }
                        Console.WriteLine("\nPress ENTER to continue...");
                        Console.ReadLine();
                        Console.Clear();
                        break;

                    case 2:
                        Console.Clear();
                        Console.WriteLine("You entered 2.");
                        break;

                    case 3:
                        Console.Clear();
                        Console.WriteLine("You entered 3.");
                        break;

                    case 4:
                        Console.Clear();
                        Console.WriteLine("---| Staff Menu |---");

                        Menu staffMenu = new Menu(new string[] { "Add Vehicle", "Remove Vehicle" });
                        staffMenu.DisplayMenu();

                        int staffOption = int.Parse(Console.ReadLine());

                        switch (staffOption)
                        {

                            case 1:
                                Console.Clear();
                                Console.WriteLine("Enter vehicle type: ");
                                string vehicleType = Console.ReadLine().ToLower().Trim();
                                HandleVehicleInput(vehicleType);
                                break;

                            case 2:
                                Console.Clear();
                                Console.WriteLine("You entered 2.");
                                break;

                            default:
                                Console.Clear();
                                Console.WriteLine("Invalid input. Please enter a number between 1 and 3.");
                                break;
                        }
                        break;

                    default:
                        Console.Clear();
                        Console.WriteLine("Invalid input. Please enter a number between 1 and 4.");
                        break;
                }
            }

            // Functions

            void AddUser(Users user) { users.Add(users.Count + 1, user); }

            void AddCar(Car car) { vehicles.Add($"C-{cars.Count+1}", car); }
            void AddTruck(Truck truck) { vehicles.Add($"T-{trucks.Count+1}", truck); }
            void AddMotorbike(Motorbike motorbike) { vehicles.Add($"M-{motorbikes.Count+1}", motorbike); }

            void HandleVehicleInput(string vehicleType)
            {
                switch (vehicleType)
                {
                    case "car":
                        Car newCar = new Car();
                        newCar = newCar.CreateCar();
                        AddCar(newCar);
                        break;

                    case "truck":
                        // Handle truck input
                        break;

                    case "motorbike":
                        // Handle motorbike input
                        break;

                    default:
                        Console.WriteLine("Unknown vehicle type. (try: car, truck, motorbike)");
                        break;
                }
            }

            // return users.Exists(customer => customer.username == user && customer.password == pass); // Good for finding people!
        }
    }
}