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
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;

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
            // Command Line Interface [In Progress]
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

            // Command Line Interface

            if (args.Length > 0)
            {
                if (args[0] == "available")
                {
                    Console.Clear();
                    DisplayAvailableVehicles();
                }
                else if (args[0] == "rented")
                {
                    if (args.Length == 2)
                    {
                        Console.Clear();
                        string userCode = args[1];
                        bool IsVerified = VerifyIdentity(userCode);
                        if (IsVerified) { DisplayedRentedVehicles(); }
                        else { Console.Clear(); Console.WriteLine("\n\nThe access code your provided is not found...\n\n"); }
                    }
                    else { Console.Clear(); Console.WriteLine("\n\n[ERROR] Invalid command usage, please provide your access code (e.g 'rented {access code}')\n\n"); }
                }
                else if (args[0] == "rent")
                {
                    if (args.Length == 4)
                    {
                        Console.Clear();

                        string userCode = args[1];
                        string rentVehicleType = args[2].ToLower().Trim();
                        string rentID = args[3].ToUpper().Trim();
                        bool IsVerified = VerifyIdentity(userCode);
                        if (IsVerified) { RentVehicles(rentVehicleType, rentID); }
                        else { Console.Clear(); Console.WriteLine("\n\nThe access code your provided is not found...\n\n"); }
                    }
                    else { Console.Clear(); Console.WriteLine("\n\n[ERROR] Invalid command usage, please provide your access code (e.g 'rented {access code}')\n\n"); }
                }
                else if (args[0] == "return")
                {

                }
            }
            else // Rest of program
            {
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
                    Menu mainMenu = new Menu(new string[] { "View Vehicles", "Rent Vehicle", "Return Vehicle", "Your Vehicles", "View Profile", "Staff Menu" });
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

                            DisplayAvailableVehicles();

                            // Filter Vehicles List
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
                            Console.WriteLine("Enter Vehicle ID: ");
                            string rentID = Console.ReadLine().ToUpper().Trim();

                            // Rent Vehicles Functionality
                            RentVehicles(rentVehicleType, rentID);

                            Console.WriteLine("Press ENTER to continue...");
                            Console.ReadLine();
                            break;

                        case 3:
                            Console.Clear();
                            Console.WriteLine("---| Return a Vehicle |---\n\n[Car] [Truck] [Motorbike]\n\nEnter vehicle type: ");
                            string returnVehicleType = Console.ReadLine().ToLower().Trim();

                            ReturnVehicles(returnVehicleType);

                            Console.WriteLine("Press ENTER to continue...");
                            Console.ReadLine();
                            break;

                        case 4:
                            Console.Clear();

                            // View current users rented vehicles
                            DisplayedRentedVehicles();

                            Console.WriteLine("Press ENTER to continue...");
                            Console.ReadLine();
                            break;

                        case 5:
                            Console.Clear();

                            // Displays the current users profile
                            DisplayProfile();

                            Console.ReadLine();
                            break;

                        case 6:
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
                            Console.WriteLine("Invalid input. Please enter a number between 1 and 6.");
                            break;
                    }
                }
            }

            // Program Functions

            //void VehiclesAddCar(Car car) { vehicles.Add($"{car.reg.reg}", car); }
            //void VehiclesAddTruck(Truck truck) { vehicles.Add($"{truck.reg.reg}", truck); }
            //void VehiclesAddMotorbike(Motorbike motorbike) { vehicles.Add($"{motorbike.reg.reg}", motorbike); }
            void AddCar(Car car) { cars.Add($"{car.reg.reg}", car); }
            void AddTruck(Truck truck) { trucks.Add($"{truck.reg.reg}", truck); }
            void AddMotorbike(Motorbike motorbike) { motorbikes.Add($"{motorbike.reg.reg}", motorbike); }

            void DisplayProfile()
            {
                Console.WriteLine($"---| Your Profile |---\nName: {currentUser.firstName} {currentUser.lastName}");
                Console.WriteLine($"\nAccount: {currentUser.GetType()}\nAccess Code: {currentUser.accessCode}");
                Console.WriteLine($"\nRented Vehicles: [{currentUser.vehicleCount}/{currentUser.rentLimit}]\n\nPress ENTER to continue...");
            }

            void DisplayAvailableVehicles()
            {
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
            }

            void DisplayedRentedVehicles()
            {
                Console.WriteLine("---| Currently Rented Vehicles |---");
                foreach (var car in rentedVehicles.rentedCars)
                {
                    Car userVehicle = car;

                    if (userVehicle.rentedBy == currentUser.accessCode)
                    {
                        Console.WriteLine($"Type: {userVehicle.type}\nManufacturer: {userVehicle.manufacturer}\nModel: {userVehicle.model}\nYear: {userVehicle.modelYear}\nColour: {userVehicle.DisplayColour()}\nRegistration: {userVehicle.DisplayReg()}\n\n");
                    }
                }
                // Check if there are no vehicles to display to the user
                if (!rentedVehicles.rentedCars.Any(car => car.rentedBy == currentUser.accessCode))
                {
                    Console.WriteLine("\nYou have no vehicles to display.\n");
                }
            }

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

            void RentVehicles(string rentVehicleType, string rentID)
            {
                switch (rentVehicleType)
                {
                    case "car":
                    case "c":
                        if (cars.ContainsKey(rentID))
                        {
                            Car car = cars[rentID];
                            currentUser.RentCar(car, rentID, rentedVehicles);
                            cars.Remove(rentID);
                            WriteVehiclesToFiles();
                        }
                        else { Console.WriteLine($"\nVehicle {rentID} not found.\n\n"); }
                        break;

                    case "truck":
                    case "t":
                        if (trucks.ContainsKey(rentID))
                        {
                            Truck truck = trucks[rentID];
                            currentUser.RentTruck(truck, rentID, rentedVehicles);
                            trucks.Remove(rentID);
                            WriteVehiclesToFiles();
                        }
                        else { Console.WriteLine($"Vehicle {rentID} not found.\n\n"); }
                        break;

                    case "motorbike":
                    case "m":
                        if (motorbikes.ContainsKey(rentID))
                        {
                            Motorbike motorbike = motorbikes[rentID];
                            currentUser.RentMotorbike(motorbike, rentID, rentedVehicles);
                            motorbikes.Remove(rentID);
                            WriteVehiclesToFiles();
                        }
                        else { Console.WriteLine($"\nVehicle {rentID} not found.\n\n"); }
                        break;

                    default:
                        Console.WriteLine("\nInvalid vehicle type entered.\n\n");
                        break;
                }
            }

            void ReturnVehicles(string returnVehicleType)
            {
                switch (returnVehicleType.ToLower())
                {
                    case "car":
                    case "c":
                        Console.WriteLine("Enter Car Registration Number: ");
                        string regPlateCar = Console.ReadLine().ToUpper().Trim();
                        if (rentedVehicles.rentedCars.Any(car => car.reg.reg == regPlateCar))
                        {
                            Car car = rentedVehicles.rentedCars.FirstOrDefault(car => car.reg.reg == regPlateCar);
                            currentUser.ReturnCar(regPlateCar, rentedVehicles);
                            AddCar(car);
                            WriteVehiclesToFiles();
                        }
                        else
                        {
                            Console.WriteLine($"\nVehicle {regPlateCar} not found.\n\n");
                        }
                        break;

                    case "truck":
                    case "t":
                        Console.WriteLine("Enter Truck Registration Number: ");
                        string regPlateTruck = Console.ReadLine().ToUpper().Trim();
                        if (trucks.Any(truck => truck.Value.reg.reg == regPlateTruck))
                        {
                            Car car = cars.FirstOrDefault(car => car.Value.reg.reg == regPlateTruck).Value;
                            currentUser.ReturnTruck(regPlateTruck, rentedVehicles);
                            if (!cars.Any(v => v.Value.reg.reg == car.reg.reg))
                            {
                                AddCar(car);
                            }
                            WriteVehiclesToFiles();
                        }
                        else
                        {
                            Console.WriteLine($"Vehicle {regPlateTruck} not found.\n\n");
                        }
                        break;

                    case "motorbike":
                    case "m":
                        Console.WriteLine("Enter Motorbike Registration Number: ");
                        string regPlateMotorbike = Console.ReadLine().ToUpper().Trim();
                        if (motorbikes.ContainsKey(regPlateMotorbike))
                        {
                            Motorbike motorbike = motorbikes[regPlateMotorbike];
                            currentUser.ReturnMotorbike(regPlateMotorbike, rentedVehicles);
                            AddMotorbike(motorbike);
                            WriteVehiclesToFiles();
                        }
                        else
                        {
                            Console.WriteLine($"\nVehicle {regPlateMotorbike} not found.\n\n");
                        }
                        break;

                    default:
                        Console.WriteLine("Invalid vehicle type.");
                        break;
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
                        Truck newTruck = new Truck();
                        newTruck = newTruck.CreateTruck();
                        AddTruck(newTruck); // Adds car to car dictionary for writing to json
                        break;

                    case "motorbike":
                        Motorbike newMotorbike = new Motorbike();
                        newMotorbike = newMotorbike.CreateMotorbike();
                        AddMotorbike(newMotorbike); // Adds car to car dictionary for writing to json
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

            void WriteRentedVehicleJSON<T>(List<T> dictionary, string fileName)
            {
                string jsonContent = JsonSerializer.Serialize(dictionary, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(fileName, jsonContent);
            }

            void ReadRentedVehicleJSON<T>(string fileName, List<T> rentedDictionary, string vehicleType) where T : Vehicle
            {
                if (File.Exists(fileName) && new FileInfo(fileName).Length > 0)
                {
                    string jsonContent = File.ReadAllText(fileName);
                    var rentedVehicles = JsonSerializer.Deserialize<List<T>>(jsonContent);

                    foreach (var vehicle in rentedVehicles)
                    {
                        if (!rentedDictionary.Any(v => v.reg.reg == vehicle.reg.reg))
                        {
                            rentedDictionary.Add(vehicle);
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
                ReadRentedVehicleJSON("rentedVehicles.json", rentedVehicles.rentedTrucks, "trucks");
                ReadRentedVehicleJSON("rentedVehicles.json", rentedVehicles.rentedMotorbikes, "motorbikes");

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
                WriteRentedVehicleJSON(rentedVehicles.rentedTrucks, "rentedVehicles.json");
                WriteRentedVehicleJSON(rentedVehicles.rentedMotorbikes, "rentedMotorbikes.json");
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

            //void CombineRentedVehicles(List<Car> cars, List<Truck> trucks, List<Motorbike> motorbikes)
            //{
            //    rentedVehicles.
            //}
        }
    }
}