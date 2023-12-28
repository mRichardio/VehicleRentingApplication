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
            // Work on validation
            // Polish up code and make useability better
            // Design/Make look nice

            // ---[ Topic Demonstration ]---
            // Robustness [In Progress] Refer to Topic Demonstration tasks on robustness
            // Writing Fast Code [IN PROGRESS SORT OF]

            // Make sure to combine topics
            // Also talk about why you have done something in a certain way

            int selected; // For menu selection
            Customer currentUser = null;

            HashSet<Customer> customers = new HashSet<Customer>();// These are Hash Sets as there can only be 1 type of each account
            HashSet<Staff> staff = new HashSet<Staff>();

            Dictionary<string, Car> cars = new Dictionary<string, Car>();
            Dictionary<string, Truck> trucks = new Dictionary<string, Truck>();
            Dictionary<string, Motorbike> motorbikes = new Dictionary<string, Motorbike>();
            Dictionary<string, Vehicle> vehicles = new();
            RentedVehicles rentedVehicles = new();

            Stopwatch stopwatch = new Stopwatch();

            // Reads vehicle data from file
            UpdateVehicleLists();
            UpdateAccounts();

            // Check if the deserialization was successful
            if (cars != null && motorbikes != null && trucks != null)
            {
                Console.WriteLine("Successfully loaded data!");
            }
            else { Console.WriteLine("Failed to deserialize JSON data."); }

            WriteVehiclesToFiles();
            WriteAccountsToFiles();

            // Command Line Interface
            if (args.Length > 0)
            {
                RunCommandLine();
            }
            else // Rest of program
            {
                while (true)
                {
                    // Identity Verification
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
                                else { Console.WriteLine("[ERROR] Access code not found!\nPress ENTER to continue..."); }
                                Console.ReadLine();
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
                    Menu mainMenu = new Menu(new string[] { "View Vehicles", "Rent Vehicle", "Return Vehicle", "Your Vehicles", "View Profile", "Staff Menu", "[LOGOUT]" });
                    Console.ForegroundColor = ConsoleColor.Blue;
                    Console.WriteLine("---| Main Menu |---");
                    Console.ResetColor();
                    mainMenu.DisplayMenu();

                    try { selected = Convert.ToInt32(Console.ReadLine()); }
                    catch (Exception e)
                    {
                        Console.WriteLine($"[Error]: {e.Message}\n"); // This message won't be seen due to the Clear console.
                        continue;                                       // (It's there to stop program crashes incase of any bugs.)
                    }

                    switch (selected)
                    {
                        case 1:
                            Console.Clear();
                            DisplayAvailableVehicles(); // Display Vehicle List
                            FilterAvailableVehicles(); // Filter Vehicles List
                            Console.Clear();
                            break;

                        case 2: // Rent Vehicle Functionality
                            Console.Clear();
                            Console.WriteLine("---| Rent a Vehicle |---\n\n[Car] [Truck] [Motorbike]\n\nEnter vehicle type: ");
                            string rentVehicleType = "";
                            try { rentVehicleType = Console.ReadLine().ToLower().Trim(); }
                            catch (Exception) { Console.WriteLine("[ERROR] Invalid Vehicle Type (Available Types: 'Car' 'Truck' 'Motorbike')"); }
                            if (rentVehicleType != "Car" || rentVehicleType != "Truck" || rentVehicleType != "Motorbike")
                            {
                                Console.WriteLine("Invalid vehicle type, must be: 'Car', 'Truck' or 'Motorbike'\nPress ENTER to continue...");
                                Console.ReadLine();
                                break;
                            }
                            else 
                            {
                                Console.WriteLine("Enter Vehicle Registration Number: ");
                                string regPlateRent = Console.ReadLine().ToUpper().Trim();
                                RentVehicles(rentVehicleType, regPlateRent);
                                Console.WriteLine("Press ENTER to continue...");
                                Console.ReadLine();
                                break;
                            }

                        case 3: // Return Vehicle Functionality
                            Console.Clear();
                            Console.WriteLine("---| Return a Vehicle |---\n\n[Car] [Truck] [Motorbike]\n\nEnter vehicle type: ");
                            string returnVehicleType = "";
                            try { returnVehicleType = Console.ReadLine().ToLower().Trim(); }
                            catch (Exception) { Console.WriteLine("[ERROR] Invalid Vehicle Type (Available Types: 'Car' 'Truck' 'Motorbike')"); }
                            if (returnVehicleType != "Car" || returnVehicleType != "Truck" || returnVehicleType != "Motorbike")
                            {
                                Console.WriteLine("Invalid vehicle type, must be: 'Car', 'Truck' or 'Motorbike'\nPress ENTER to continue...");
                                Console.ReadLine();
                                break;
                            }
                            else
                            {
                                Console.WriteLine("Enter Vehicle Registration Number: ");
                                string regPlateReturn = Console.ReadLine().ToUpper().Trim();
                                ReturnVehicles(returnVehicleType, regPlateReturn);
                                Console.WriteLine("Press ENTER to continue...");
                                Console.ReadLine();
                                break;
                            }

                        case 4: // Display Rented Vehicles Functionality
                            Console.Clear();
                            DisplayedRentedVehicles();
                            Console.WriteLine("Press ENTER to continue...");
                            Console.ReadLine();
                            break;

                        case 5: // Display Current Users Profile
                            Console.Clear();
                            DisplayProfile();
                            Console.ReadLine();
                            break;

                        case 6:
                            Console.Clear(); // Staff Menu Section
                            Console.ForegroundColor = ConsoleColor.Blue;
                            Console.WriteLine("---| Staff Menu |---");
                            Console.ResetColor();

                            // Displays the staff menu
                            Menu staffMenu = new Menu(new string[] { "Add Vehicle", "Remove Vehicle", "View Staff", "View Customers", "[Back]" });
                            staffMenu.DisplayMenu();
                            int staffOption = 0;
                            try { staffOption = int.Parse(Console.ReadLine()); }// Need a try catch here // Also need to go back                            }
                            catch (Exception)
                            {
                                Console.WriteLine("[ERROR] Invalid input. Make sure to choose one of the the options from the menu. (e.g 1,2)\nPress ENTER to continue...");
                                Console.ReadLine();
                            }

                            switch (staffOption)
                            {
                                case 1:
                                    Console.Clear();
                                    Console.WriteLine("Enter vehicle type: \n[ Car | Truck | Motorbike ]");
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

                                case 5: // Back button in staff menu
                                    break; 

                                default:
                                    Console.Clear();
                                    Console.WriteLine("Invalid input. Please enter a number between 1 and 3.");
                                    break;
                            }
                            break;

                        case 7:
                            currentUser = null;
                            break;

                        default:
                            Console.Clear();
                            Console.WriteLine("Invalid input. Please enter a number between 1 and 6.");
                            break;
                    }
                }
            }

            // Program Functions
            void AddCar(Car car) { cars.Add($"{car.reg.reg}", car); }
            void AddTruck(Truck truck) { trucks.Add($"{truck.reg.reg}", truck); }
            void AddMotorbike(Motorbike motorbike) { motorbikes.Add($"{motorbike.reg.reg}", motorbike); }

            void DisplayProfile()
            {
                Console.WriteLine($"---| Your Profile |---\nName: {currentUser.firstName} {currentUser.lastName}");
                Console.WriteLine($"\nAccount: {currentUser.GetType()}\nAccess Code: {currentUser.accessCode}");
                Console.WriteLine($"\nRented Vehicles: [{rentedVehicles.GetVehicleCount(currentUser)}/{currentUser.GetRentLimit()}]\n\nPress ENTER to continue...");
            }

            void DisplayAvailableVehicles()
            {
                Console.WriteLine("---| Available Vehicles |---\n");

                if (vehicles != null)
                {
                    UpdateVehicleLists();
                    Console.WriteLine($"{vehicles.Count} Results found\n");

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
                foreach (var vehicle in rentedVehicles.combinedVehicles)
                {
                    Vehicle userVehicle = vehicle;
                    if (userVehicle.rentedBy == currentUser.accessCode)
                    {
                        Console.WriteLine($"Type: {userVehicle.type}\nManufacturer: {userVehicle.manufacturer}\nModel: {userVehicle.model}\nYear: {userVehicle.modelYear}\nColour: {userVehicle.DisplayColour()}\nRegistration: {userVehicle.DisplayReg()}\n\n");
                    }
                }
                // Check if there are no vehicles to display to the user
                if (!rentedVehicles.combinedVehicles.Any(vehicle => vehicle.rentedBy == currentUser.accessCode))
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
                            UpdateVehicleLists();
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
                            UpdateVehicleLists();
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
                            UpdateVehicleLists();
                        }
                        else { Console.WriteLine($"\nVehicle {rentID} not found.\n\n"); }
                        break;

                    default:
                        Console.WriteLine("\nInvalid vehicle type entered.\n\n");
                        break;
                }
            }

            void ReturnVehicles(string returnVehicleType, string regPlate)
            {
                switch (returnVehicleType.ToLower())
                {
                    case "car":
                    case "c":
                        if (rentedVehicles.rentedCars.Any(car => car.reg.reg == regPlate))
                        {
                            Car car = rentedVehicles.rentedCars.FirstOrDefault(car => car.reg.reg == regPlate);
                            currentUser.ReturnCar(regPlate, rentedVehicles);
                            AddCar(car);
                            WriteVehiclesToFiles();
                            UpdateVehicleLists();
                        }
                        else { Console.WriteLine($"\nVehicle {regPlate} not found.\n\n"); }
                        break;

                    case "truck":
                    case "t":
                        if (rentedVehicles.rentedTrucks.Any(truck => truck.reg.reg == regPlate))
                        {
                            Truck truck = rentedVehicles.rentedTrucks.FirstOrDefault(truck => truck.reg.reg == regPlate);
                            currentUser.ReturnTruck(regPlate, rentedVehicles);
                            AddTruck(truck);
                            WriteVehiclesToFiles();
                            UpdateVehicleLists();
                        }
                        else { Console.WriteLine($"Vehicle {regPlate} not found.\n\n"); }
                        break;

                    case "motorbike":
                    case "m":
                        if (rentedVehicles.rentedMotorbikes.Any(motorbike => motorbike.reg.reg == regPlate))
                        {
                            Motorbike motorbike = rentedVehicles.rentedMotorbikes.FirstOrDefault(motorbike => motorbike.reg.reg == regPlate);
                            currentUser.ReturnMotorbike(regPlate, rentedVehicles);
                            AddMotorbike(motorbike);
                            WriteVehiclesToFiles();
                            UpdateVehicleLists();
                        }
                        else { Console.WriteLine($"\nVehicle {regPlate} not found.\n\n"); }
                        break;

                    default:
                        Console.WriteLine("Invalid vehicle type.");
                        break;
                }
            }

            void RunCommandLine()
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
                    else { Console.Clear(); Console.WriteLine("\n\n[ERROR] Invalid command usage, please provide your access code (e.g 'rent {access code} {vehicle type} {reg number}')\n\n"); }
                }
                else if (args[0] == "return")
                {
                    if (args.Length == 4)
                    {
                        string userCode = args[1];
                        string returnVehicleType = args[2].ToLower().Trim();
                        string regPlate = args[3].ToUpper().Trim();
                        bool IsVerified = VerifyIdentity(userCode);
                        if (IsVerified) { ReturnVehicles(returnVehicleType, regPlate); }
                        else { Console.Clear(); Console.WriteLine("\n\nThe access code your provided is not found...\n\n"); }
                    }
                    else { Console.Clear(); Console.WriteLine("\n\n[ERROR] Invalid command usage, please provide your access code (e.g 'return {access code} {vehicle type} {reg number}')\n\n"); }
                }
                if (args.Length >= 2)
                {
                    if (args[0] == "filter")
                    {
                        RunFilter();
                    }
                }
            }

            void FilterAvailableVehicles()
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("\n1.[Search] 2.[Filter] 3.[Back]");
                Console.ResetColor();
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
                    Console.WriteLine("Select filter type: ");
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("1.[Manufacturer] 2.[Year] 3.[Value] 4.[Cancel]");
                    Console.ResetColor();

                    string filterChoice = Console.ReadLine().Trim().ToLower();
                    if (filterChoice == "manufacturer" || filterChoice == "1" || filterChoice == "m")
                    {
                        Console.WriteLine("Enter manufacturer: ");
                        string inputManufacturer = Console.ReadLine().Trim().ToLower();

                        filteredVehicles = vehicles
                                .Where(vehicle => vehicle.Value.manufacturer.ToLower() == inputManufacturer)
                                .Select(vehicle => vehicle.Value);

                        if (filteredVehicles.Any())
                        {
                            foreach (var vehicle in filteredVehicles)
                            {
                                Console.WriteLine($"Vehicle Type: {vehicle.type}\nYear: {vehicle.modelYear}\nManufacturer: {vehicle.manufacturer}\nModel: {vehicle.model}\n");
                            }
                        }
                        else { Console.WriteLine("No vehicles found"); }

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
                    else if (filterChoice == "value" || filterChoice == "3" || filterChoice == "v")
                    {
                        Vehicle bestValue = FindBestValue(vehicles);

                        if (bestValue != null)
                        {
                            Console.WriteLine($"Best value vehicle based on condition and model year criteria:");
                            Console.WriteLine($"- {bestValue.manufacturer} {bestValue.model} ({bestValue.modelYear})\nRegistration: {bestValue.reg.reg}");
                        }
                        else { Console.WriteLine($"No vehicles to filter."); }
                        Console.WriteLine("Press ENTER to continue...");
                        Console.ReadLine();
                    }
                }
                else { Console.WriteLine($"{choice} not found."); }
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
                catch (Exception) { Console.WriteLine("[ERROR] Couldn't find any vehicles"); }
                return foundVehicles;

            }

            void HandleVehicleInput(string vehicleType)
            {
                switch (vehicleType)
                {
                    case "car":
                    case "c":
                        Car newCar = new Car();
                        newCar = newCar.CreateVehicle();
                        AddCar(newCar); // Adds car to car dictionary for writing to json
                        break;

                    case "truck":
                    case "t":
                        Truck newTruck = new Truck();
                        newTruck = newTruck.CreateVehicle();
                        AddTruck(newTruck); // Adds car to car dictionary for writing to json
                        break;

                    case "motorbike":
                    case "m":
                        Motorbike newMotorbike = new Motorbike();
                        newMotorbike = newMotorbike.CreateVehicle();
                        AddMotorbike(newMotorbike); // Adds car to car dictionary for writing to json
                        break;

                    default:
                        Console.WriteLine("Unknown vehicle type. (try: car, truck, motorbike\nPress ENTER to continue...)");
                        Console.ReadLine();
                        break;
                }
            }

            void ReadStaffJSON(string fileName, HashSet<Staff> hash)
            {
                string jsonContent = File.ReadAllText(fileName);
                var items = JsonSerializer.Deserialize<HashSet<Staff>>(jsonContent);

                foreach (var item in items)
                {
                    hash.Add(item);
                }
            }

            void ReadCustomerJSON(string fileName, HashSet<Customer> hash)
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

            void ReadRentedVehicleJSON<T>(string fileName, List<T> rentedList, string vehicleType) where T : Vehicle
            {
                if (File.Exists(fileName) && new FileInfo(fileName).Length > 0)
                {
                    rentedList.Clear();
                    string jsonContent = File.ReadAllText(fileName);
                    var rentedVehicles = JsonSerializer.Deserialize<List<T>>(jsonContent);

                    foreach (var vehicle in rentedVehicles)
                    {
                        if (!rentedList.Any(v => v.reg.reg == vehicle.reg.reg))
                        {
                            rentedList.Add(vehicle);
                        }
                    }
                }
                else
                {
                    Console.WriteLine("The file is empty or does not exist.");
                }
            }

            // Sums up the vehicles value based off of it's condition and year it was made
            float CalculateWeightedValue(float condition, int modelYear)
            {
                return condition * 0.7f + modelYear * 0.3f;
            }


            List<Vehicle> FilterVehicles(Dictionary<string, Vehicle> vehicles, string filterType)
            {
                UpdateVehicleLists();
                switch (filterType.ToLower())
                {
                    case "oldest":
                        return vehicles.Values.OrderBy(v => v.modelYear).ToList();
                    case "newest":
                        return vehicles.Values.OrderByDescending(v => v.modelYear).ToList();
                    case "bestcondition":
                        return vehicles.Values.OrderByDescending(v => v.condition).ToList();
                    default:
                        Console.WriteLine("Invalid filter type.");
                        return vehicles.Values.ToList();
                }
            }

            void RunFilter()
            {
                if (args.Length >= 2)
                {
                    if (args[0] == "filter")
                    {
                        if (args.Length >= 2)
                        {
                            string filterType = args[1].ToLower();

                            // Filters the vehicle list to find the vehicle with best value.
                            if (filterType == "value")
                            {
                                Vehicle bestValue = FindBestValue(vehicles);

                                if (bestValue != null)
                                {
                                    Console.WriteLine($"Best value vehicle based on condition and model year criteria:");
                                    Console.WriteLine($"- {bestValue.manufacturer} {bestValue.model} ({bestValue.modelYear})");
                                }
                                else { Console.WriteLine($"No vehicles to filter."); }
                            }
                            else
                            {
                                List<Vehicle> filteredVehicles = FilterVehicles(vehicles, filterType);

                                if (filteredVehicles.Count > 0)
                                {
                                    Console.WriteLine($"Filtered vehicles based on '{filterType}':");

                                    foreach (var filteredVehicle in filteredVehicles)
                                    {
                                        Console.WriteLine($"- {filteredVehicle.manufacturer} {filteredVehicle.model} ({filteredVehicle.modelYear})");
                                    }
                                }
                                else { Console.WriteLine($"No vehicles to filter."); }
                            }
                        }
                        else { Console.WriteLine("Not enough arguments for the 'filter' action."); }
                    }
                    else { Console.WriteLine("Invalid command. Use 'filter' to filter vehicles."); }
                }
                else { Console.WriteLine("No command specified."); }
            }

            // Multi Line Algorithm
            Vehicle FindBestValue(Dictionary<string, Vehicle> vehicles)
            {
                stopwatch.Start();
                //My example of a multi - line lambda expression for finding the best valued vehicle

               //var bestVehicle = vehicles.Values
               //    .OrderByDescending(v =>
               //    {
               //        var weight = CalculateWeightedValue(v.condition, v.modelYear);
               //        return weight;
               //    })
               //    .FirstOrDefault();

                var bestVehicle = vehicles.Values
                    .AsParallel()
                    .OrderByDescending(v =>
                    {
                        var weight = CalculateWeightedValue(v.condition, v.modelYear);
                        return weight;
                    })
                    .FirstOrDefault();
                stopwatch.Stop();
                Console.WriteLine($"Time taken: {stopwatch.ElapsedMilliseconds}");
                Console.ReadLine();

                return bestVehicle;
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
                vehicleDictionary.Clear();
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
                rentedVehicles.combinedVehicles.Clear();

                // Read Vehicles from file.
                ReadVehicleJSON("cars.json", cars, "cars");
                ReadVehicleJSON("trucks.json", trucks, "trucks");
                ReadVehicleJSON("motorbikes.json", motorbikes, "motorbikes");
                ReadRentedVehicleJSON("rentedCars.json", rentedVehicles.rentedCars, "cars");
                ReadRentedVehicleJSON("rentedTrucks.json", rentedVehicles.rentedTrucks, "trucks");
                ReadRentedVehicleJSON("rentedMotorbikes.json", rentedVehicles.rentedMotorbikes, "motorbikes");

                // Combines all vehicles into one dictionary.
                AddVehiclesToDictionary(vehicles, cars, trucks, motorbikes);
                AddRentedVehiclesToList(rentedVehicles.combinedVehicles, rentedVehicles.rentedCars, rentedVehicles.rentedTrucks, rentedVehicles.rentedMotorbikes);
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
                WriteRentedVehicleJSON(rentedVehicles.rentedCars, "rentedCars.json");
                WriteRentedVehicleJSON(rentedVehicles.rentedTrucks, "rentedTrucks.json");
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
                vehicles.Clear();
                cars.ToList().ForEach(car => vehicles[car.Key] = car.Value);
                trucks.ToList().ForEach(truck => vehicles[truck.Key] = truck.Value);
                motorbikes.ToList().ForEach(motorbike => vehicles[motorbike.Key] = motorbike.Value);
            }

            void AddRentedVehiclesToList(List<Vehicle> rentedVehicles, List<Car> cars, List<Truck> trucks, List<Motorbike> motorbikes)
            {
                rentedVehicles.AddRange(cars);
                rentedVehicles.AddRange(trucks);
                rentedVehicles.AddRange(motorbikes);
            }

        }
    }
}