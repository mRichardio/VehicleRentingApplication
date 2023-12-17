using System.Transactions;
using System.Linq;
using System.Reflection.Emit;
using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Diagnostics;
using System.Reflection.Metadata;
using System.ComponentModel.Design;

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
            // Add remove vehicle functionality
            // Start working on renting functionality (before a car is rented a unique id for the user should be used, to add up total rented count.)
            // Add functionality to view profile

            // ---[ Side Quests ]---
            // Hide staff menu behind a staff password
            // Add a Min and Max year for vehicle in filters
            // Test what happens if the json files become null

            // ---[ Topic Demonstration ]---
            // Dealing with data // Collection [DONE] // Algorithms [In Progress] - Need a multi-line algorithm
            // Command Line Interface [TODO]
            // Robustness [TODO] Refer to Topic Demonstration tasks on robustness
            // Object-Oriented Programming [Think DONE but Check this!!] Look at topic demonstration tasks on OOP to double check I have included everything
            // Data Persistence [In Progress (Fully used JSON Serialisation, but need to ask about binary!!!!!)] IMPORTANT <<<<<<<
            // Writing Fast Code [TODO]

            // Make sure to combine topics
            // Also talk about why you have done something in a certain way

            // ---[ Important Things that need to happen :D ]---
            // Make another interface to demonstrate that a class can inherit from two different interfaces


            int selected; // For menu selection
            Customer currentUser = null;

            HashSet<Customer> customers = new HashSet<Customer>();// These are Hash Sets as there can only be 1 type of each account
            HashSet<Staff> staff = new HashSet<Staff>();

            Dictionary<string, Car> cars = new Dictionary<string, Car>();
            Dictionary<string, Truck> trucks = new Dictionary<string, Truck>();
            Dictionary<string, Motorbike> motorbikes = new Dictionary<string, Motorbike>();
            Dictionary<string, Vehicle> vehicles = new();

            // Reads vehicle data from file
            UpdateVehicleLists();
            UpdateAccounts();

            // Check if the deserialization was successful
            if (cars != null && motorbikes != null && trucks != null)
            {
                Console.WriteLine("Successfully loaded data!");
            }
            else
            {
                Console.WriteLine("Failed to deserialize JSON data.");
            }

            WriteVehiclesToFiles();
            WriteAccountsToFiles();

            // Program Code

            while (true)
            {
                //Identity Verification
                while (currentUser == null)
                {
                    Console.Clear();
                    Console.WriteLine("Do you have an account?: ");
                    string userInput = Console.ReadLine().ToLower().Trim();
                    if (userInput == "y" || userInput == "yes")
                    {
                        while (true)
                        {
                            Console.Clear();
                            Console.WriteLine("Enter your access code: ");
                            string userCode = Console.ReadLine().Trim();
                            bool IsVerified = VerifyIdentity(userCode);
                            if (IsVerified) { break; }
                        }
                        break;
                    }
                    else if (userInput == "n" || userInput == "no")
                    {
                        var (firstName, lastName) = RegisterName();
                        Customer newCustomer = new Customer(firstName, lastName);
                        currentUser = newCustomer;
                        customers.Add(currentUser);
                        WriteAccountsToFiles();
                        break;
                    }
                }

                // Main Program
                Console.Clear();
                // Displays the main menu
                Menu mainMenu = new Menu(new string[] {"View Vehicles", "Your Vehicles", "View Profile", "Staff Menu" });
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

                        if (vehicles != null)
                        {
                            Console.WriteLine($"\n{vehicles.Count} Results found\n");
                            UpdateVehicleLists();

                            foreach (var (key, vehicle) in vehicles)
                            {
                                Console.WriteLine($"ID: {key}, Vehicle Type: {vehicle.type}\nYear: {vehicle.modelYear}\nManufacturer: {vehicle.manufacturer}\nModel: {vehicle.model}\n");
                            }
                        }
                        else
                        {
                            Console.WriteLine("No vehicles available to rent.");
                        }
                        Console.WriteLine("\n[Search] [Filter] [Back]");
                        string choice = Console.ReadLine().Trim().ToLower();
                        if (choice == "search" || choice == "1" || choice == "s")
                        {
                            Console.Clear();
                            Console.WriteLine("Enter Vehicle ID");
                            string vehID = Console.ReadLine().Trim().ToUpper();

                            var selectedVehicle = vehicles.FirstOrDefault(v => v.Key == vehID);

                            Console.WriteLine(FindVehicleByID(vehID));

                            Console.WriteLine("\nPress ENTER to continue...");
                            Console.ReadLine();
                        }
                        else if (choice == "filter" || choice == "2" || choice == "f")
                        {
                            IEnumerable<Vehicle> filteredVehicles;
                            Console.Clear();
                            Console.WriteLine("Select filter type: \n[Manufacturer] [Year] [Cancel]");

                            string filterChoice = Console.ReadLine().Trim().ToLower();
                            if (filterChoice == "manufacturer" || filterChoice == "1" || filterChoice == "m")
                            {
                                Console.WriteLine("Enter manufacturer: ");
                                string inputManufacturer = Console.ReadLine().Trim().ToLower();

                                filteredVehicles = vehicles
                                        .Where(vehicle => vehicle.Value.manufacturer.ToLower() == inputManufacturer)
                                        .Select(vehicle => vehicle.Value);

                                foreach (var vehicle in filteredVehicles)
                                {
                                    Console.WriteLine($"Vehicle Type: {vehicle.type}\nYear: {vehicle.modelYear}\nManufacturer: {vehicle.manufacturer}\nModel: {vehicle.model}\n");
                                }

                                Console.WriteLine("\nPress ENTER to continue...");
                                Console.ReadLine();
                            }
                            else if (filterChoice == "year" || filterChoice == "2" || filterChoice == "y")
                            {
                                Console.WriteLine("Enter specific year (or try 'before 2005' / 'after 2005'): ");
                                string inputYear = Console.ReadLine().Trim().ToLower();

                                foreach (var vehicle in FindVehiclesByYear(inputYear))
                                {
                                    Console.WriteLine($"Vehicle Type: {vehicle.type}\nYear: {vehicle.modelYear}\nManufacturer: {vehicle.manufacturer}\nModel: {vehicle.model}\n");
                                }

                                Console.WriteLine("\nPress ENTER to continue...");
                                Console.ReadLine();
                            }
                        }
                        else { Console.WriteLine($"{choice} not found."); }
                        Console.Clear();
                        break;

                    case 2:
                        Console.Clear();
                        Console.WriteLine("---| Currently Rented Vehicles |---");
                        if (currentUser.GetRentedVehicles() != null)
                        {
                            foreach (var vehicle in currentUser.GetRentedVehicles())
                            {
                                Console.WriteLine($"Type: {vehicle.GetType()}\nManufacturer: {vehicle.manufacturer}\nModel: {vehicle.model}\nYear: {vehicle.modelYear}\nRegistration: {vehicle.reg}\nColour: {vehicle.paint}\n\n");
                            }
                        }
                        else
                        {
                            Console.WriteLine("\nYou have no vehicles to display.\n");
                        }
                        Console.WriteLine("Press ENTER to continue...");
                        Console.ReadLine();
                        break;  

                    case 3:
                        Console.Clear();
                        Console.WriteLine($"---| Your Profile |---\nName: {currentUser.firstName} {currentUser.lastName}");
                        Console.WriteLine($"\nAccount: {currentUser.GetType()}\nAccess Code: {currentUser.accessCode}");
                        Console.WriteLine($"\nRented Vehicles: [{currentUser.vehicleCount}/{currentUser.rentLimit}]\n\nPress ENTER to continue...");
                        Console.ReadLine();
                        break;

                    case 4:
                        Console.Clear();
                        Console.WriteLine("---| Staff Menu |---");
                        
                        // Displays the staff menu
                        Menu staffMenu = new Menu(new string[] { "Add Vehicle", "Remove Vehicle", "View Staff", "View Customers" });
                        staffMenu.DisplayMenu();

                        int staffOption = int.Parse(Console.ReadLine());

                        switch (staffOption)
                        {
                            case 1:
                                Console.Clear();
                                Console.WriteLine("Enter vehicle type: ");
                                string vehicleType = Console.ReadLine().ToLower().Trim();
                                HandleVehicleInput(vehicleType);
                                WriteVehiclesToFiles();
                                break;

                            case 2:
                                Console.Clear();
                                Console.WriteLine("You entered 2. Remove Vehicle");
                                Console.WriteLine("Enter vehicle ID: ");
                                string inputID = Console.ReadLine().ToUpper().Trim();
                                if (vehicles.ContainsKey(inputID))
                                {
                                    removeVehicleByID(inputID);
                                    WriteVehiclesToFiles();
                                    Console.WriteLine($"Vehicle: {inputID}, has been removed from the system.\nPress ENTER to continue...");
                                    Console.ReadLine();
                                }
                                else { Console.WriteLine($"Vehicle with ID: {inputID} not found.\nPress ENTER to continue..."); Console.ReadLine(); }
                                break;

                            case 3:
                                Console.Clear();
                                Console.WriteLine("You entered 2. View Staff");
                                break;

                            case 4:
                                Console.Clear();
                                Console.WriteLine("You entered 2. View Customers");
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

            //void AddUser(Users user) { users.Add(users.Count + 1, user); }

            void VehiclesAddCar(Car car) { vehicles.Add($"C-{cars.Count + 1}", car); }
            void AddCar(Car car) { cars.Add($"C-{cars.Count + 1}", car); }
            void VehiclesAddTruck(Truck truck) { vehicles.Add($"T-{trucks.Count + 1}", truck); }
            void AddTruck(Truck truck) { trucks.Add($"T-{trucks.Count + 1}", truck); }
            void VehiclesAddMotorbike(Motorbike motorbike) { vehicles.Add($"M-{motorbikes.Count + 1}", motorbike); }
            void AddMotorbike(Motorbike motorbike) { motorbikes.Add($"M-{motorbikes.Count + 1}", motorbike); }

            (string FirstName, string LastName) RegisterName() // Uses a tuple to return to strings. Easier than creating two functions.
            {
                Console.WriteLine("Enter your firstname: ");
                string fname = Console.ReadLine().Trim();

                Console.WriteLine("Enter your lastname: ");
                string lname = Console.ReadLine().Trim();

                return (fname, lname);
            }

            bool VerifyIdentity(string code)
            {
                Customer selectedCustomer = customers.FirstOrDefault(c => c.accessCode == code);
                if (selectedCustomer != null)
                {
                    currentUser = selectedCustomer;
                    return true;
                }
                else { return false; }
            }

            string FindVehicleByID(string vehID)
            {
                var selectedVehicle = vehicles.FirstOrDefault(v => v.Key == vehID);

                if (selectedVehicle.Key != null)
                {
                    string key = selectedVehicle.Key;
                    Vehicle vehicle = selectedVehicle.Value;

                    return $"ID: {key}, Vehicle Type: {vehicle.type}\n" +
                                      $"Year: {vehicle.modelYear}\n" +
                                      $"Manufacturer: {vehicle.manufacturer}\n" +
                                      $"Model: {vehicle.model}\n";
                }
                else
                {
                    return $"No vehicle found with ID: {vehID}";
                }
            }

            void removeVehicleByID(string vehID)
            {
                if (vehID.StartsWith("C"))
                {
                    var selectedVehicle = cars.FirstOrDefault(v => v.Key == vehID);
                    if (selectedVehicle.Key != null) { cars.Remove(selectedVehicle.Key); }
                }
                if (vehID.StartsWith("T"))
                {
                    var selectedVehicle = trucks.FirstOrDefault(v => v.Key == vehID);
                    if (selectedVehicle.Key != null) { trucks.Remove(selectedVehicle.Key); }
                }
                if (vehID.StartsWith("M"))
                {
                    var selectedVehicle = motorbikes.FirstOrDefault(v => v.Key == vehID);
                    if (selectedVehicle.Key != null) { motorbikes.Remove(selectedVehicle.Key); }
                }
            }

            List<Vehicle> FindVehiclesByYear(string year)
            {
                List<Vehicle> foundVehicles = new();
                try
                {
                    if (year.Contains("before"))
                    {
                        year = year.Replace("before", "").Trim();
                        int targetYear = Convert.ToInt32(year);
                        foundVehicles = vehicles
                            .Where(v => v.Value.modelYear < targetYear)
                            .Select(v => v.Value)
                            .ToList();
                    }
                    else if (year.Contains("after"))
                    {
                        year = year.Replace("after", "").Trim();
                        int targetYear = Convert.ToInt32(year);
                        foundVehicles = vehicles
                            .Where(v => v.Value.modelYear > targetYear)
                            .Select(v => v.Value)
                            .ToList();
                    }
                    else
                    {
                        int targetYear = Convert.ToInt32(year);
                        foundVehicles = vehicles
                            .Where(v => v.Value.modelYear == targetYear)
                            .Select(v => v.Value)
                            .ToList();
                    }
                }
                catch (Exception) { Console.WriteLine("TEMP ERROR"); }
                return foundVehicles;

            }

            void HandleVehicleInput(string vehicleType)
            {
                switch (vehicleType)
                {
                    case "car":
                        Car newCar = new Car();
                        newCar = newCar.CreateCar();
                        //VehiclesAddCar(newCar); // Adds car to vehicle dictionary
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

            static void ReadAccountsJSON<T>(string fileName, HashSet<T> hash)
            {
                string jsonContent = File.ReadAllText(fileName);
                var items = JsonSerializer.Deserialize<HashSet<T>>(jsonContent);

                foreach (var item in items)
                {
                    hash.Add(item);
                }
            }

            void WriteAccountJSON<T>(HashSet<T> hash, string fileName)
            {
                string jsonContent = JsonSerializer.Serialize(hash, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(fileName, jsonContent);
            }

            void WriteVehicleJSON<T>(Dictionary<string, T> dictionary, string fileName)
            {
                string jsonContent = JsonSerializer.Serialize(dictionary, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(fileName, jsonContent);
            }

            // Reads objects from json file, then places them in their dictionary
            void ReadVehicleJSON<T>(string fileName, Dictionary<string, T> vehicleDictionary, string vehicleType) where T : Vehicle
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

            void UpdateVehicleLists()
            {
                vehicles.Clear();

                // Read Vehicles from file.
                ReadVehicleJSON("cars.json", cars, "cars");
                ReadVehicleJSON("trucks.json", trucks, "trucks");
                ReadVehicleJSON("motorbikes.json", motorbikes, "motorbikes");

                // Combines all vehicles into one dictionary.
                AddVehiclesToDictionary(vehicles, cars, trucks, motorbikes);
            }

            void UpdateAccounts()
            {
                // Read Accounts from file.
                ReadAccountsJSON("customers.json", customers);
                ReadAccountsJSON("staff.json", staff);
            }

            void WriteVehiclesToFiles()
            {
                // Write Vehicles to file.
                WriteVehicleJSON(cars, "cars.json");
                WriteVehicleJSON(trucks, "trucks.json");
                WriteVehicleJSON(motorbikes, "motorbikes.json");
            }

            void WriteAccountsToFiles()
            {
                // Write Accounts to file.
                WriteAccountJSON(customers, "customers.json");
                WriteAccountJSON(staff, "staff.json");
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