using System.Transactions;
using System.Linq;
using System.Reflection.Emit;
using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Diagnostics;
using System.Reflection.Metadata;
using System.ComponentModel.Design;
using System.Runtime.ConstrainedExecution;

namespace VehicleRentingApplication
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // ---[ Main Quests ]---
            // - Use Parallel Execution (If I use this then I need to explain why I have used it.
            // E.g. I use single thread and got a response time slower than when using parallel execution.))
            // Add remove vehicle functionality

            // ---[ Side Quests ]---
            // Hide staff menu behind a staff password
            // Add a Min and Max year for vehicle in filters
            // Test what happens if the json files become null

            // ---[ Topic Demonstration ]---
            // Dealing with data // Collection [DONE] // Algorithms [In Progress] - Need a multi-line algorithm
            // Command Line Interface [TODO]
            // Robustness [TODO] Refer to Topic Demonstration tasks on robustness
            // Object-Oriented Programming [Think DONE but Check this!!] Look at topic demonstration tasks on OOP to double check I have included everything
            // Data Persistence [DONE]
            // Writing Fast Code [IN PROGRESS SORT OF]

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
            RentedVehicles rentedVehicles = new();
            // Inside the Main method


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

                //RentedVehicleHandler rentedVehicleHandler = new RentedVehicleHandler(currentUser.rentLimit, currentUser);

                // Main Program
                Console.Clear();
                // Displays the main menu
                Menu mainMenu = new Menu(new string[] {"View Vehicles", "Rent Vehicle", "Your Vehicles", "View Profile", "Staff Menu" });
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
                                Console.WriteLine($"ID: {key}, Vehicle Type: {vehicle.type}\nYear: {vehicle.modelYear}\nManufacturer: {vehicle.manufacturer}\nModel: {vehicle.model}\nPaint: {vehicle.DisplayColour()}\nRegistration: {vehicle.DisplayReg()}\n");
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
                        Console.WriteLine("---| Rent a Vehicle |---\n\n[Car] [Truck] [Motorbike]\n\nEnter vehicle type: ");
                        string rentVehicleType = Console.ReadLine().ToLower().Trim();
                        if (rentVehicleType == "car" || rentVehicleType == "c")
                        {
                            Console.WriteLine("Enter Car ID: ");
                            string rentCarID = Console.ReadLine().ToUpper().Trim();
                            if (cars.ContainsKey(rentCarID))
                            {
                                Car car = cars[rentCarID];
                                currentUser.RentCar(car, rentCarID, rentedVehicles);
                                WriteVehiclesToFiles();
                            }
                            else { Console.WriteLine($"\nVehicle {rentCarID} not found.\n\n"); }
                        }
                        else if (rentVehicleType == "truck" || rentVehicleType == "t")
                        {
                            Console.WriteLine("Enter Truck ID: ");
                            string rentTruckID = Console.ReadLine().ToUpper().Trim();
                            if (trucks.ContainsKey(rentTruckID))
                            {
                                Truck truck = trucks[rentTruckID];
                                currentUser.RentTruck(truck, rentTruckID, rentedVehicles);
                                WriteVehiclesToFiles();
                            }
                            else { Console.WriteLine($"Vehicle {rentTruckID} not found.\n\n"); }
                        }
                        else if (rentVehicleType == "motorbike" || rentVehicleType == "m")
                        {
                            Console.WriteLine("Enter Motorbike ID: ");
                            string rentMotorbikeID = Console.ReadLine().ToUpper().Trim();
                            if (motorbikes.ContainsKey(rentMotorbikeID))
                            {
                                Motorbike motorbike = motorbikes[rentMotorbikeID];
                                currentUser.RentMotorbike(motorbike, rentMotorbikeID, rentedVehicles);
                                WriteVehiclesToFiles();
                            }
                            else { Console.WriteLine($"\nVehicle {rentMotorbikeID} not found.\n\n"); }
                        }
                        Console.WriteLine("Press ENTER to continue...");
                        Console.ReadLine();
                        break;

                    case 3:
                        Console.Clear();
                        Console.WriteLine("---| Currently Rented Vehicles |---");
                        foreach (var pair in rentedVehicles.rentedCars)
                        {
                            Car userVehicle = pair.Value;

                            if (userVehicle.rentedBy == currentUser.accessCode)
                            {
                                Console.WriteLine($"Type: {userVehicle.type}\nManufacturer: {userVehicle.manufacturer}\nModel: {userVehicle.model}\nYear: {userVehicle.modelYear}\nColour: {userVehicle.DisplayColour()}\nRegistration: {userVehicle.DisplayReg()}\n\n");
                            }
                        }
                        // Check if there are no vehicles to display to the user
                        if (!rentedVehicles.rentedCars.Any(pair => pair.Value.rentedBy == currentUser.accessCode))
                        {
                            Console.WriteLine("\nYou have no vehicles to display.\n");
                        }

                        Console.WriteLine("Press ENTER to continue...");
                        Console.ReadLine();
                        break;

                    case 4:
                        Console.Clear();
                        Console.WriteLine($"---| Your Profile |---\nName: {currentUser.firstName} {currentUser.lastName}");
                        Console.WriteLine($"\nAccount: {currentUser.GetType()}\nAccess Code: {currentUser.accessCode}");
                        Console.WriteLine($"\nRented Vehicles: [{currentUser.vehicleCount}/{currentUser.rentLimit}]\n\nPress ENTER to continue...");
                        Console.ReadLine();
                        break;

                    case 5:
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
                                Console.WriteLine("---| Staff List |---");
                                if (staff != null)
                                {
                                    foreach (var s in staff)
                                    {
                                        Console.WriteLine($"\nName: {s.firstName} {s.lastName}\nAccount: {s.GetType()}\nAccess Code: {s.accessCode}");
                                    }
                                }
                                else
                                {
                                    Console.WriteLine($"\nStaff list is empty...\n");    
                                }
                                Console.WriteLine("Press ENTER to continue...");
                                Console.ReadLine();
                                break;

                            case 4:
                                Console.Clear();
                                Console.WriteLine("---| Customer List |---");
                                if (customers != null)
                                {
                                    foreach (var c in customers)
                                    {
                                        Console.WriteLine($"\nName: {c.firstName} {c.lastName}\nAccount: {c.GetType()}\nAccess Code: {c.accessCode}");
                                    }
                                }
                                else
                                {
                                    Console.WriteLine($"\nCustomer list is empty...\n");
                                }
                                Console.WriteLine("\nPress ENTER to continue...");
                                Console.ReadLine();
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

            // Program Functions

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

            static void ReadStaffJSON(string fileName, HashSet<Staff> hash)
            {
                string jsonContent = File.ReadAllText(fileName);
                var items = JsonSerializer.Deserialize<HashSet<Staff>>(jsonContent);

                foreach (var item in items)
                {
                    hash.Add(item);
                }
            }

            static void ReadCustomerJSON(string fileName, HashSet<Customer> hash)
            {
                string jsonContent = File.ReadAllText(fileName);
                var items = JsonSerializer.Deserialize<HashSet<Customer>>(jsonContent);

                foreach (var item in items)
                {
                    hash.Add(item);
                }
            }

            void WriteRentedVehicleJSON<T>(Dictionary<string, T> dictionary, string fileName)
            {
                string jsonContent = JsonSerializer.Serialize(dictionary, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(fileName, jsonContent);
            }

            void ReadRentedVehicleJSON<T>(string fileName, Dictionary<string, T> rentedDictionary, string vehicleType) where T : Vehicle
            {
                if (File.Exists(fileName) && new FileInfo(fileName).Length > 0)
                {
                    string jsonContent = File.ReadAllText(fileName);
                    var rentedObjects = JsonSerializer.Deserialize<Dictionary<string, T>>(jsonContent);

                    foreach (var ro in rentedObjects)
                    {
                        if (!rentedDictionary.ContainsKey(ro.Key))
                        {
                            rentedDictionary.Add(ro.Key, ro.Value);
                        }
                    }
                }
                else
                {
                    Console.WriteLine("The file is empty or does not exist.");
                }
            }

            void WriteStaffJSON(HashSet<Staff> hash, string fileName)
            {
                string jsonContent = JsonSerializer.Serialize(hash, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(fileName, jsonContent);
            }

            void WriteCustomerJSON(HashSet<Customer> hash, string fileName)
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
                ReadRentedVehicleJSON("rentedVehicles.json", rentedVehicles.rentedCars, "cars");

                // Combines all vehicles into one dictionary.
                AddVehiclesToDictionary(vehicles, cars, trucks, motorbikes);
            }

            void UpdateAccounts()
            {
                // Read Accounts from file.
                ReadCustomerJSON("customers.json", customers);
                ReadStaffJSON("staff.json", staff);
            }

            void WriteVehiclesToFiles()
            {
                // Write Vehicles to file.
                WriteVehicleJSON(cars, "cars.json");
                WriteVehicleJSON(trucks, "trucks.json");
                WriteVehicleJSON(motorbikes, "motorbikes.json");
                WriteRentedVehicleJSON(rentedVehicles.rentedCars, "rentedVehicles.json");
            }

            void WriteAccountsToFiles()
            {
                // Write Accounts to file.
                WriteCustomerJSON(customers, "customers.json");
                WriteStaffJSON(staff, "staff.json");
            }

            // Combines all separate dictionaries into one. (I had to do it this way as 'Vehicle' is an abstract class)
            void AddVehiclesToDictionary(Dictionary<string, Vehicle> vehicles, Dictionary<string, Car> cars, Dictionary<string, Truck> trucks, Dictionary<string, Motorbike> motorbikes)
            {
                cars.ToList().ForEach(car => vehicles[car.Key] = car.Value);
                trucks.ToList().ForEach(truck => vehicles[truck.Key] = truck.Value);
                motorbikes.ToList().ForEach(motorbike => vehicles[motorbike.Key] = motorbike.Value);
            }
        }
    }
}