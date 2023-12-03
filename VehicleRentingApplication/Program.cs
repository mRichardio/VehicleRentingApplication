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

            // Reads vehicle data from file
            UpdateVehicleLists();

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

            WriteToFiles();




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
                            UpdateVehicleLists();
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
                                WriteToFiles();
                                UpdateVehicleLists();
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

            void VehiclesAddCar(Car car) { vehicles.Add($"C-{cars.Count+1}", car); }
            void AddCar(Car car) { cars.Add($"C-{cars.Count+1}", car); }
            void VehiclesAddTruck(Truck truck) { vehicles.Add($"T-{trucks.Count+1}", truck); }
            void AddTruck(Truck truck) { trucks.Add($"T-{trucks.Count+1}", truck); }
            void VehiclesAddMotorbike(Motorbike motorbike) { vehicles.Add($"M-{motorbikes.Count+1}", motorbike); }
            void AddMotorbike(Motorbike motorbike) { motorbikes.Add($"M-{motorbikes.Count+1}", motorbike); }

            void HandleVehicleInput(string vehicleType)
            {
                switch (vehicleType)
                {
                    case "car":
                        Car newCar = new Car();
                        newCar = newCar.CreateCar();
                        VehiclesAddCar(newCar); // Adds car to vehicle dictionary
                        AddCar(newCar); // Adds car to car dictionary for writing to json
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

            void WriteDictionaryToJsonFile<T>(Dictionary<string, T> dictionary, string fileName)
            {
                string jsonContent = JsonSerializer.Serialize(dictionary, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(fileName, jsonContent);
            }

            // Reads objects from json file, then places them in their dictionary
            void ReadAndDeserializeJsonFile<T>(string fileName, Dictionary<string, T> vehicleDictionary, string vehicleType) where T : Vehicle
            {
                try
                {
                    string jsonContent = File.ReadAllText(fileName);
                    var vehicles = JsonSerializer.Deserialize<Dictionary<string, T>>(jsonContent);

                    foreach (var vehicle in vehicles)
                    {
                        if (!vehicleDictionary.ContainsKey(vehicle.Key))
                        {
                            vehicleDictionary.Add(vehicle.Key, vehicle.Value);
                        }
                    }
                }
                catch (JsonException)
                {
                    Console.WriteLine($"[ERROR]: No {vehicleType} found...");
                }
            }

            void UpdateVehicleLists()
            {
                // Read Vehicles from file.
                ReadAndDeserializeJsonFile("cars.json", cars, "cars");
                ReadAndDeserializeJsonFile("trucks.json", trucks, "trucks");
                ReadAndDeserializeJsonFile("motorbikes.json", motorbikes, "motorbikes");

                // Combines all vehicles into one dictionary.
                AddVehiclesToDictionary(vehicles, cars, trucks, motorbikes);
            }

            void WriteToFiles()
            {
                // Write Vehicles to file.
                WriteDictionaryToJsonFile(cars, "cars.json");
                WriteDictionaryToJsonFile(trucks, "trucks.json");
                WriteDictionaryToJsonFile(motorbikes, "motorbikes.json");
            }

            // Combines all separate dictionaries into one. (I had to do it this way as 'Vehicle' is an abstract class)
            void AddVehiclesToDictionary(Dictionary<string, Vehicle> vehicles, Dictionary<string, Car> cars, Dictionary<string, Truck> trucks, Dictionary<string, Motorbike> motorbikes)
            {
                cars.ToList().ForEach(car => vehicles[car.Key] = car.Value);
                trucks.ToList().ForEach(truck => vehicles[truck.Key] = truck.Value);
                motorbikes.ToList().ForEach(motorbike => vehicles[motorbike.Key] = motorbike.Value);
            }

            // return users.Exists(customer => customer.username == user && customer.password == pass); // Good for finding people!
        }
    }
}