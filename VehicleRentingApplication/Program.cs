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
using System.Net.Http.Json;
using System.Xml;
using System.Diagnostics.Metrics;

namespace VehicleRentingApplication
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // ---[ Topic Demonstration ]---

            // Topic 1 [DONE]
            // Topic 2 [DONE]
            // Topic 3 [In Progress]

            // ALSO Some of the filters are still only filtering cars

            // ---[ Video ]---

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
                        Console.WriteLine("Do you have an account? (Y/N): ");
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

                    // I used a try catch here as this is one of the main conversions that will be used in the program
                    // there can't be any issues here otherwise the program wouldn't work.
                    try { selected = Convert.ToInt32(Console.ReadLine()); }
                    catch (FormatException) { Console.WriteLine("[ERROR]: The string that you have inputted is in the incorrect format, (Try, '1', '2' etc)..."); continue; }
                    catch (OverflowException) { Console.WriteLine("[ERROR]: Your input number is how of the range of conversion. (Please enter a number correlating to one of the menu options...)"); continue; }
                    catch (ArgumentNullException) { Console.WriteLine("[ERROR]: Your input was null. (Make sure your input isn't empty!)"); continue; }

                    switch (selected)
                    {
                        case 1:
                            Console.Clear();
                            Console.Write("Types:");
                            Console.ForegroundColor = ConsoleColor.Yellow;
                            Console.Write("[All] [Car] [Truck] [Motorbike]");
                            Console.ResetColor(); 
                            Console.WriteLine("\nEnter vehicle type you are looking for: ");
                            string vehType = Console.ReadLine().Trim().ToLower(); Console.Clear();
                            if (vehType != null && vehType == "car" || vehType == "truck" || vehType == "motorbike" || vehType == "all")
                            {
                                DisplayAvailableVehicles(vehType);
                                FilterAvailableVehicles();
                            }
                            else { Console.WriteLine("Invalid Vehicle Type: [Try, 'car', 'truck', 'motorbike', 'all']"); }
                            Console.WriteLine("Press ENTER to continue...");
                            Console.ReadLine();
                            break;

                        case 2: // Rent Vehicle Functionality
                            Console.Clear();
                            Console.ForegroundColor = ConsoleColor.Blue;
                            Console.WriteLine("---| Rent a Vehicle |---");
                            Console.ForegroundColor = ConsoleColor.Yellow;
                            Console.WriteLine("\n[Car] [Truck] [Motorbike]");
                            Console.ResetColor();
                            Console.WriteLine("\nEnter vehicle type: ");
                            string rentVehicleType = "";
                            try { rentVehicleType = Console.ReadLine().ToLower().Trim(); }
                            catch (Exception) { Console.WriteLine("[ERROR] Invalid Vehicle Type (Available Types: 'Car' 'Truck' 'Motorbike')"); }
                            if (rentVehicleType == "car" || rentVehicleType == "c" || rentVehicleType == "truck" || rentVehicleType == "t" || rentVehicleType == "motorbike" || rentVehicleType == "m")
                            {
                                Console.WriteLine("Enter Vehicle Registration Number: ");
                                string regPlateRent = Console.ReadLine().ToUpper().Trim();
                                RentVehicles(rentVehicleType, regPlateRent);
                                Console.WriteLine("Press ENTER to continue...");
                                Console.ReadLine();
                                break;
                            }
                            else 
                            {
                                Console.WriteLine("Invalid vehicle type, must be: 'Car', 'Truck' or 'Motorbike'\nPress ENTER to continue...");
                                Console.ReadLine();
                                break;
                            }

                        case 3: // Return Vehicle Functionality
                            Console.Clear();
                            Console.ForegroundColor = ConsoleColor.Blue;
                            Console.WriteLine("---| Return a Vehicle |---");
                            Console.ForegroundColor = ConsoleColor.Yellow;
                            Console.WriteLine("\n[Car] [Truck] [Motorbike]");
                            Console.ResetColor();
                            Console.WriteLine("\nEnter vehicle type: ");
                            string returnVehicleType = "";
                            try { returnVehicleType = Console.ReadLine().ToLower().Trim(); }
                            catch (Exception) { Console.WriteLine("[ERROR] Invalid Vehicle Type (Available Types: 'Car' 'Truck' 'Motorbike')"); }
                            if (returnVehicleType == "car" || returnVehicleType == "truck" || returnVehicleType == "motorbike")
                            {
                                Console.WriteLine("Enter Vehicle Registration Number: ");
                                string regPlateReturn = Console.ReadLine().ToUpper().Trim();
                                ReturnVehicles(returnVehicleType, regPlateReturn);
                                Console.WriteLine("Press ENTER to continue...");
                                Console.ReadLine();
                                break;
                            }
                            else
                            {
                                Console.WriteLine("Invalid vehicle type, must be: 'Car', 'Truck' or 'Motorbike'\nPress ENTER to continue...");
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
                            Console.WriteLine("Enter a staff code to view this menu: ");
                            string staffCode = Console.ReadLine().Trim();
                            bool isStaff = VerifyStaffCode(staffCode);
                            if (!isStaff) 
                            { 
                                Console.WriteLine("[ERROR] Invalid staff code\nPress ENTER to continue...");
                                Console.ReadLine();
                                break;
                            }
                            else
                            {
                                while (isStaff == true)
                                {
                                    Console.ForegroundColor = ConsoleColor.Blue;
                                    Console.WriteLine("---| Staff Menu |---");
                                    Console.ResetColor();

                                    // Displays the staff menu
                                    Menu staffMenu = new Menu(new string[] { "Add Vehicle", "Remove Vehicle", "View Staff", "View Customers", "Add Staff", "[Back]" });
                                    staffMenu.DisplayMenu();
                                    int staffOption = 0;
                                    try { staffOption = int.Parse(Console.ReadLine()); }// Need a try catch here // Also need to go back                            }
                                    catch (Exception)
                                    {
                                        Console.WriteLine("[ERROR] Invalid input. Make sure to choose one of the the options from the menu. (e.g 1,2)\nPress ENTER to continue...");
                                        Console.ReadLine();
                                        Console.Clear();
                                        continue;
                                    }

                                    switch (staffOption)
                                    {
                                        case 1:
                                            Console.Clear();
                                            Console.ForegroundColor = ConsoleColor.Blue;
                                            Console.WriteLine("---| Add Vehicle |---");
                                            Console.ResetColor();
                                            Console.ForegroundColor = ConsoleColor.Yellow;
                                            Console.WriteLine("\n[Car] [Truck] [Motorbike]");
                                            Console.ResetColor();
                                            Console.WriteLine("Enter vehicle type:");
                                            string vehicleType = Console.ReadLine().ToLower().Trim();
                                            HandleVehicleInput(vehicleType);
                                            WriteVehiclesToFiles();
                                            Console.Clear();
                                            break;

                                        case 2:
                                            Console.Clear();
                                            Console.ForegroundColor = ConsoleColor.Blue;
                                            Console.WriteLine("---| Remove Vehicle |---");
                                            Console.ResetColor();
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
                                            Console.Clear();
                                            break;

                                        case 3:
                                            Console.Clear();
                                            Console.ForegroundColor = ConsoleColor.Blue;
                                            Console.WriteLine("---| Staff List |---");
                                            Console.ResetColor();
                                            if (staff != null)
                                            {
                                                foreach (var s in staff)
                                                {
                                                    Console.WriteLine($"\nName: {s.FirstName} {s.LastName}\nAccount: {s.GetType()}\nAccess Code: {s.GetAccessCode()}\nPast Customer?: {s.PastCustomerCheck()}");
                                                }
                                            }
                                            else
                                            {
                                                Console.WriteLine($"\nStaff list is empty...\n");
                                            }
                                            Console.WriteLine("Press ENTER to continue...");
                                            Console.ReadLine();
                                            Console.Clear();
                                            break;

                                        case 4:
                                            Console.Clear();
                                            Console.ForegroundColor = ConsoleColor.Blue;
                                            Console.WriteLine("---| Customer List |---");
                                            Console.ResetColor();
                                            if (customers != null)
                                            {
                                                foreach (var c in customers)
                                                {
                                                    Console.WriteLine($"\nName: {c.FirstName} {c.LastName}\nAccount: {c.GetType()}\nAccess Code: {c.AccessCode}");
                                                }
                                            }
                                            else
                                            {
                                                Console.WriteLine($"\nCustomer list is empty...\n");
                                            }
                                            Console.WriteLine("\nPress ENTER to continue...");
                                            Console.ReadLine();
                                            Console.Clear();
                                            break;

                                        case 5:
                                            Console.WriteLine("Is this person an existing customer?");
                                            string choice = Console.ReadLine().Trim().ToLower();
                                            if (choice == "y" || choice == "yes")
                                            {
                                                Console.WriteLine("Enter the customers access code: ");
                                                string customerCode = Console.ReadLine().Trim();
                                                Customer customer = customers.FirstOrDefault(customer => customer.AccessCode == customerCode);
                                                Staff newStaff = new Staff(customer);
                                                staff.Add(newStaff);
                                                customers.Remove(customer);
                                                Console.WriteLine("Successfully created new staff member!\nPress ENTER to continue...");
                                                Console.ReadLine();
                                                WriteAccountsToFiles();
                                            }
                                            else if (choice == "n" || choice == "no")
                                            {
                                                Staff newStaff = new();
                                                newStaff.RegisterStaff(staff);
                                                WriteAccountsToFiles();
                                            }
                                            else { Console.WriteLine($"Invalid choice: {choice} not found\nPress ENTER to continue..."); Console.ReadLine(); }
                                            Console.Clear();
                                            break;
                                        case 6: // Back button in staff menu
                                            isStaff = false; // Exits staff loop
                                            break;

                                        default:
                                            Console.Clear();
                                            Console.WriteLine("Invalid input. Please enter a number between 1 and 6.");
                                            break;
                                    }
                                }
                                break;
                            }

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
            void AddCar(Car car) { cars.Add($"{car.Reg.Reg}", car); }
            void AddTruck(Truck truck) { trucks.Add($"{truck.Reg.Reg}", truck); }
            void AddMotorbike(Motorbike motorbike) { motorbikes.Add($"{motorbike.Reg.Reg}", motorbike); }

            void DisplayProfile()
            {
                Console.ForegroundColor = ConsoleColor.Blue;
                Console.WriteLine($"---| Your Profile |---");
                Console.ResetColor();
                Console.WriteLine($"Name: {currentUser.FirstName} {currentUser.LastName}");
                Console.WriteLine($"\nAccount: {currentUser.GetType()}\nAccess Code: {currentUser.AccessCode}");
                Console.WriteLine($"\nRented Vehicles: [{rentedVehicles.GetVehicleCount(currentUser)}/{currentUser.GetRentLimit()}]\n\nPress ENTER to continue...");
            }

            void DisplayAvailableVehicles(string type)
            {
                Console.ForegroundColor = ConsoleColor.Blue;
                Console.WriteLine("---| Available Vehicles |---\n");
                Console.ResetColor();

                if (vehicles != null && vehicles.Count > 0)
                {
                    UpdateVehicleLists();
                    int counter = 0;
                    foreach (var (key, vehicle) in vehicles)
                    {
                        string vehicleType = vehicle.GetVehicleType().ToLower();
                        if (vehicleType == type)
                        {
                            counter++;
                            // Price is purely cosmetic in this implementation (could easily be added in but it was functionality that wasn't relevant to the assignment)
                            vehicle.CalculatePrice();
                            Console.WriteLine($"ID: {key}, Vehicle Type: {vehicle.GetVehicleType()}\nYear: {vehicle.ModelYear}\nManufacturer: {vehicle.Manufacturer}\nModel: {vehicle.Model}\nPaint: {vehicle.DisplayColour()}\nRegistration: {vehicle.DisplayReg()}\nPrice: £{vehicle.GetPrice()}/month\n");
                        }
                        else if (type == "all")
                        {
                            counter++;
                            vehicle.CalculatePrice();
                            Console.WriteLine($"ID: {key}, Vehicle Type: {vehicle.GetVehicleType()}\nYear: {vehicle.ModelYear}\nManufacturer: {vehicle.Manufacturer}\nModel: {vehicle.Model}\nPaint: {vehicle.DisplayColour()}\nRegistration: {vehicle.DisplayReg()}\nPrice: £{vehicle.GetPrice()}/month\n");
                        }
                    }
                    Console.WriteLine($"{counter} Results found\n");
                }
                else { Console.WriteLine("No vehicles available to rent."); }
            }

            void DisplayedRentedVehicles()
            {
                Console.ForegroundColor = ConsoleColor.Blue;
                Console.WriteLine("---| Currently Rented Vehicles |---");
                Console.ResetColor();
                foreach (var vehicle in rentedVehicles.CombinedVehicles)
                {
                    Vehicle userVehicle = vehicle;
                    if (userVehicle.RentedBy == currentUser.AccessCode)
                    {
                        Console.WriteLine($"Type: {userVehicle.GetVehicleType()}\nManufacturer: {userVehicle.Manufacturer}\nModel: {userVehicle.Model}\nYear: {userVehicle.ModelYear}\nColour: {userVehicle.DisplayColour()}\nRegistration: {userVehicle.DisplayReg()}\n\n");
                    }
                }
                // Check if there are no vehicles to display to the user
                if (!rentedVehicles.CombinedVehicles.Any(vehicle => vehicle.RentedBy == currentUser.AccessCode))
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
                Customer selectedCustomer = customers.FirstOrDefault(c => c.AccessCode == code);
                if (selectedCustomer != null)
                {
                    currentUser = selectedCustomer;
                    return true;
                }
                else { return false; }
            }

            bool VerifyStaffCode(string code)
            {
                Staff selectedStaff = staff.FirstOrDefault(s => s.AccessCode == code);
                if (selectedStaff != null) { return true; }
                else { return false; }
            }

            string FindVehicleByID(string vehID)
            {
                var selectedVehicle = vehicles.FirstOrDefault(v => v.Key == vehID);

                if (selectedVehicle.Key != null)
                {
                    string key = selectedVehicle.Key;
                    Vehicle vehicle = selectedVehicle.Value;

                    return $"ID: {key}, Vehicle Type: {vehicle.GetVehicleType()}\n" +
                                      $"Year: {vehicle.ModelYear}\n" +
                                      $"Manufacturer: {vehicle.Manufacturer}\n" +
                                      $"Model: {vehicle.Model}\n";
                }
                else
                {
                    return $"No vehicle found with Reg Number: {vehID}";
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
                        if (rentedVehicles.RentedCars.Any(car => car.Reg.Reg == regPlate))
                        {
                            Car car = rentedVehicles.RentedCars.FirstOrDefault(car => car.Reg.Reg == regPlate);
                            currentUser.ReturnCar(regPlate, rentedVehicles);
                            AddCar(car);
                            WriteVehiclesToFiles();
                            UpdateVehicleLists();
                        }
                        else { Console.WriteLine($"\nVehicle {regPlate} not found.\n\n"); }
                        break;

                    case "truck":
                    case "t":
                        if (rentedVehicles.RentedTrucks.Any(truck => truck.Reg.Reg == regPlate))
                        {
                            Truck truck = rentedVehicles.RentedTrucks.FirstOrDefault(truck => truck.Reg.Reg == regPlate);
                            currentUser.ReturnTruck(regPlate, rentedVehicles);
                            AddTruck(truck);
                            WriteVehiclesToFiles();
                            UpdateVehicleLists();
                        }
                        else { Console.WriteLine($"Vehicle {regPlate} not found.\n\n"); }
                        break;

                    case "motorbike":
                    case "m":
                        if (rentedVehicles.RentedMotorbikes.Any(motorbike => motorbike.Reg.Reg == regPlate))
                        {
                            Motorbike motorbike = rentedVehicles.RentedMotorbikes.FirstOrDefault(motorbike => motorbike.Reg.Reg == regPlate);
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
                if (args.Contains("-v"))
                {
                    Console.WriteLine("Version Number: 1.12\n\n[If you are trying to run another command remove '-v' from your arguments]");
                }
                else if (args.Contains("-h"))
                {
                    Console.ForegroundColor = ConsoleColor.Blue;
                    Console.WriteLine("\n---| Vehicle Renting Application |---");
                    Console.ResetColor();
                    Console.WriteLine("\nUseful commands: ");
                    Console.WriteLine("-v : Shows the current version number of the application");
                    Console.WriteLine("-h : Displays a list of the current available commands");
                    Console.WriteLine("available : Gets all of the vehicles that are available to rent");
                    Console.WriteLine("rented {access code}: This will get a list of a customers currentl rented cars using code as their access code");
                    Console.WriteLine("rent {access code} {vehicle type} {reg number}: Allows users to rent one of the available vehicles");
                    Console.WriteLine("return {access code} {vehicle type} {reg number}: Returns any currently rented vehicles");
                    Console.WriteLine("filter value: Displays the current vehicle with the best value");
                    Console.WriteLine("filter oldest: Displays available vehicles in order of oldest -> newest");
                    Console.WriteLine("filter newest: Displays available vehicles in order of newest -> oldest\n--------------------------------\n");
                }
                else
                {
                    if (args[0] == "available")
                    {
                        try // This try catch is ensuring that there is definately a second argument given
                        {
                            Console.Clear();
                            args[1].ToLower();
                            DisplayAvailableVehicles(args[1]);
                        }
                        catch (IndexOutOfRangeException)
                        {
                            Console.WriteLine("Invalid command usage... (try 'available all', 'car' 'truck' 'motorbike')");
                        }
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
                    else if (args[0] == "filter" )
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
                    Console.WriteLine("Enter Vehicle Registration Number:");
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
                                .Where(vehicle => vehicle.Value.Manufacturer.ToLower() == inputManufacturer)
                                .Select(vehicle => vehicle.Value);

                        if (filteredVehicles.Any())
                        {
                            foreach (var vehicle in filteredVehicles)
                            {
                                Console.WriteLine($"Vehicle Type: {vehicle.GetVehicleType()}\nYear: {vehicle.ModelYear}\nManufacturer: {vehicle.Manufacturer}\nModel: {vehicle.Model}\n");
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
                            Console.WriteLine($"Vehicle Type: {vehicle.GetVehicleType()}\nYear: {vehicle.ModelYear}\nManufacturer: {vehicle.Manufacturer}\nModel: {vehicle.Model}\n");
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
                            Console.WriteLine($"- {bestValue.Manufacturer} {bestValue.Model} ({bestValue.ModelYear})\nRegistration: {bestValue.Reg.Reg}");
                        }
                        else { Console.WriteLine($"No vehicles to filter."); }
                        Console.WriteLine("Press ENTER to continue...");
                        Console.ReadLine();
                    }
                }
                //else { Console.WriteLine($"Choice: {choice} not found."); }
            }

            List<Vehicle> FindVehiclesByYear(string year)
            {
                List<Vehicle> foundVehicles = new();
                if (year.Contains("before"))
                {
                    // I use a .replace here as I am also using this function in my commandline
                    //year = year.Replace("before", "").Trim();
                    int targetYear = Convert.ToInt32(year);
                    foundVehicles = vehicles
                        .Where(v => v.Value.ModelYear < targetYear)
                        .Select(v => v.Value)
                        .ToList();
                }
                else if (year.Contains("after"))
                {
                    //year = year.Replace("after", "").Trim();
                    int targetYear = Convert.ToInt32(year);
                    foundVehicles = vehicles
                        .Where(v => v.Value.ModelYear > targetYear)
                        .Select(v => v.Value)
                        .ToList();
                }
                else
                {
                    int targetYear = Convert.ToInt32(year);
                    foundVehicles = vehicles
                        .Where(v => v.Value.ModelYear == targetYear)
                        .Select(v => v.Value)
                        .ToList();
                }
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
                        if (!rentedList.Any(v => v.Reg.Reg == vehicle.Reg.Reg))
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


            // This function was implemented to cater for commandline filters.
            List<Vehicle> FilterVehicles(Dictionary<string, Vehicle> vehicles, string filterType)
            {
                UpdateVehicleLists();
                switch (filterType.ToLower())
                {
                    case "oldest":
                        return vehicles.Values.OrderBy(v => v.ModelYear).ToList();
                    case "newest": // Used OrderByDesc to ensure that the list is filtered oldest to newest so that the user can find older vehicles
                        return vehicles.Values.OrderByDescending(v => v.ModelYear).ToList();
                    case "bestcondition": // Used OrderByDesc to ensure that the list is filtered newest to oldest so that the user can find newer vehicles
                        return vehicles.Values.OrderByDescending(v => v.Condition).ToList();
                    default:
                        return null;
                }
            }

            // This function was included to deal with the filtering in command line
            void RunFilter()
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
                            Console.ForegroundColor = ConsoleColor.Blue;
                            Console.WriteLine($"\nBest value vehicle based on condition and model year criteria:");
                            Console.ResetColor();
                            Console.WriteLine($"- {bestValue.Manufacturer} {bestValue.Model} ({bestValue.ModelYear})\n");
                        }
                        else { Console.WriteLine($"No vehicles to filter."); }
                    }
                    else
                    {
                        try
                        {
                            List<Vehicle> filteredVehicles = FilterVehicles(vehicles, filterType);
                            if (filteredVehicles.Count > 0)
                            {
                                Console.ForegroundColor = ConsoleColor.Blue;
                                Console.WriteLine($"\nFiltered vehicles based on '{filterType}':");
                                Console.ResetColor();

                                foreach (var filteredVehicle in filteredVehicles)
                                {
                                    Console.WriteLine($"- {filteredVehicle.Manufacturer} {filteredVehicle.Model} ({filteredVehicle.ModelYear}) (Condition: {filteredVehicle.Condition}%) - {filteredVehicle.GetVehicleType()}");
                                }
                                Console.WriteLine("");
                            }
                            else { Console.WriteLine($"No vehicles to filter."); }
                        }
                        catch (NullReferenceException)
                        {
                            Console.WriteLine("The filter you are trying to use is not supported, (Try: 'newest' 'oldest' 'value' 'bestcondition')");
                        }
                    }
                }
                else { Console.WriteLine("Not enough arguments for the 'filter' action. (Filters: 'newest' 'oldest' 'value' 'bestcondition')"); }
            }

            // Multi Line Algorithm
            Vehicle FindBestValue(Dictionary<string, Vehicle> vehicles)
            {
                // This isn't the most performant way of handling this, however I did it this way
                // as it was the simplest way I could think of to run this function on each vehicle
                var bestVehicle = vehicles.Values
                    .OrderByDescending(v =>
                    {
                        var weight = CalculateWeightedValue(v.Condition, v.ModelYear);
                        return weight;
                    })
                    .FirstOrDefault();

                return bestVehicle;
            }

            // Writes staff hash to staff.json
            void WriteStaffJSON(HashSet<Staff> hash, string fileName)
            {
                string jsonContent = JsonSerializer.Serialize(hash, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(fileName, jsonContent);
            }

            // Writes customer hash to customer.json
            void WriteCustomerJSON(HashSet<Customer> hash, string fileName)
            {
                string jsonContent = JsonSerializer.Serialize(hash, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(fileName, jsonContent);
            }

            // Multipurpose vehicle writing function can write all the different types of vehicles
            // hence why I have made use of the <T> type. This function is modular so will allow
            // to write different dictionaries to different json files.
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

            // This function is used to read from json files and update lists. I need this to ensure that the program updates during runtime.
            void UpdateVehicleLists()
            {
                vehicles.Clear();
                rentedVehicles.CombinedVehicles.Clear();

                // Read Vehicles from file.
                ReadVehicleJSON("cars.json", cars, "cars");
                ReadVehicleJSON("trucks.json", trucks, "trucks");
                ReadVehicleJSON("motorbikes.json", motorbikes, "motorbikes");
                ReadRentedVehicleJSON("rentedCars.json", rentedVehicles.RentedCars, "cars");
                ReadRentedVehicleJSON("rentedTrucks.json", rentedVehicles.RentedTrucks, "trucks");
                ReadRentedVehicleJSON("rentedMotorbikes.json", rentedVehicles.RentedMotorbikes, "motorbikes");

                // Combines all vehicles into one dictionary.
                AddVehiclesToDictionary(vehicles, cars, trucks, motorbikes);
                AddRentedVehiclesToList(rentedVehicles.CombinedVehicles, rentedVehicles.RentedCars, rentedVehicles.RentedTrucks, rentedVehicles.RentedMotorbikes);
            }

            // Reads json data to update all account details.
            void UpdateAccounts()
            {
                // Read Accounts from file.
                ReadCustomerJSON("customers.json", customers);
                ReadStaffJSON("staff.json", staff);
            }

            // Writes all vehiciles to files. I needed this function to allow for saving after making a change during runtime. e.g creating vehicles
            void WriteVehiclesToFiles()
            {
                // Write Vehicles to file.
                WriteVehicleJSON(cars, "cars.json");
                WriteVehicleJSON(trucks, "trucks.json");
                WriteVehicleJSON(motorbikes, "motorbikes.json");
                WriteRentedVehicleJSON(rentedVehicles.RentedCars, "rentedCars.json");
                WriteRentedVehicleJSON(rentedVehicles.RentedTrucks, "rentedTrucks.json");
                WriteRentedVehicleJSON(rentedVehicles.RentedMotorbikes, "rentedMotorbikes.json");
            }

            // Writes all the account data to files. Needed for if an account is registered during runtime.
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

            // This function is used to combine all of the rentedvehicles lists together into one which can be used during runtime.
            // I did this in this way as it was hard to serialize and store objects with different types into json files.
            // So instead I created json files for each type of vehicle.
            void AddRentedVehiclesToList(List<Vehicle> rentedVehicles, List<Car> cars, List<Truck> trucks, List<Motorbike> motorbikes)
            {
                // I have made use of the AddRange method to concatenate all of the lists into one.
                rentedVehicles.AddRange(cars);
                rentedVehicles.AddRange(trucks);
                rentedVehicles.AddRange(motorbikes);
            }
        }

    }
}