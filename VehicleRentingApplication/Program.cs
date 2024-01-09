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
using System.Reflection.Metadata.Ecma335;
using System.Runtime.CompilerServices;

namespace VehicleRentingApplication
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // ---[ Topic Demonstration ]---

            // Add condition to available vehicle displays

            // Properly Comment on each section that is important in the code to explain why I have implemented something in a certain way.
            // Take a final look through all of the code and the systems functionalities and make sure everything works as intended.
            // Format all of the code better and make sure everything is nice and readable.

            // ---[ Video ]---

            // Make sure to combine topics
            // Also talk about why you have done something in a certain way

            int selected; // For menu selection
            // Provides validation and functionality to the currently logged in customer in the system.
            Customer currentUser = null;

            HashSet<Customer> customers = new HashSet<Customer>();// These are Hash Sets as there can only be 1 type of each account
            HashSet<Staff> staff = new HashSet<Staff>();

            // I decided to use Dictionaries for my vehicle storage as it makes it easier and more performant execute searches/lookups
            // Also using dictionaries helped me store the vehicles registration num as the key, with it's tied vehicle being the value
            Dictionary<string, Car> cars = new Dictionary<string, Car>();
            Dictionary<string, Truck> trucks = new Dictionary<string, Truck>();
            Dictionary<string, Motorbike> motorbikes = new Dictionary<string, Motorbike>();
            Dictionary<string, Vehicle> vehicles = new();

            // Only one objects is instantiated as the lists of currently rented vehicles are stored within the class/object.
            // This object will act as the handler of these lists/rentedVehicles
            RentedVehicles rentedVehicles = new();

            // Reads vehicle data from file
            UpdateVehicleLists();
            UpdateAccounts();

            // Checks if the deserialisation was successful.
            // I initially added this functionality for debugging serialisation, however if the deserialisation does ever fail
            // I think that it is better to keep this in the system so the user knows why it isn't working in the case of failure.
            if (cars != null && motorbikes != null && trucks != null)
            {
                Console.WriteLine("Successfully loaded data!");
            }
            else { Console.WriteLine("Failed to deserialize JSON data."); }

            // I made these functions to serialise data, they are called here to to ensure all json files are up to date
            //WriteVehiclesToFiles();
            //WriteAccountsToFiles();

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
                    // This section will verify the customer code provided by the user to allow or deny access
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
                                Console.WriteLine("Enter your access code (Enter '0 to go back'): ");
                                string userCode = Console.ReadLine().Trim();
                                bool IsVerified = VerifyIdentity(userCode);
                                if (IsVerified)
                                {
                                    break;
                                }
                                else if (userCode == "0")
                                {
                                    break;
                                }
                                else { Console.WriteLine("[ERROR] Access code not found!\nPress ENTER to continue..."); }
                                Console.ReadLine();
                            }
                            break;
                        }
                        else if (userInput == "n" || userInput == "no")
                        {
                            // This is a tuple, makes it easier to combine two functionalities into one. e.g registering the first and last name variables
                            var (firstName, lastName) = RegisterName();
                            Customer newCustomer = new Customer(firstName, lastName);
                            currentUser = newCustomer;
                            customers.Add(currentUser);
                            WriteAccountsToFiles();
                            break;
                        }
                    }

                    // This is here so if the user enters '0' to back out of logging, in the program will go back to
                    // the beginning of the first while loop, looking as the the user has just canceled logging in.
                    if (currentUser == null)
                    {
                        continue;
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
                    // there can't be any issues here otherwise the program wouldn't work. Hence why I am trying to catch a lot of exceptions here.
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
                            // I provided lots of different conditions to navigate the menu as I think it will improve the UX.
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

            // Used for adding vehicles into the system (staff)
            void AddCar(Car car) { cars.Add($"{car.Reg.Reg}", car); }
            void AddTruck(Truck truck) { trucks.Add($"{truck.Reg.Reg}", truck); }
            void AddMotorbike(Motorbike motorbike) { motorbikes.Add($"{motorbike.Reg.Reg}", motorbike); }

            // Displays the currently logged in customers profile.
            void DisplayProfile()
            {
                Console.ForegroundColor = ConsoleColor.Blue;
                Console.WriteLine($"---| Your Profile |---");
                Console.ResetColor();
                Console.WriteLine($"Name: {currentUser.FirstName} {currentUser.LastName}");
                Console.WriteLine($"\nAccount: {currentUser.GetType()}\nAccess Code: {currentUser.AccessCode}");
                Console.WriteLine($"\nRented Vehicles: [{rentedVehicles.GetVehicleCount(currentUser)}/{currentUser.GetRentLimit()}]\n\nPress ENTER to continue...");
            }

            // This function will display all of the currently available vehicles to rent in the system.
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
                            // The reason why I decided to calculate price during runtime is simply because if changes were to be made to any vehicles properties,
                            // whether it be manually through the text editor or in runtime, then the associated price and price category would need to be manually updated.
                            // So doing it in runtime instead, means that if any changes happened to a vehicle then it's price will always correspond to those changes.
                            // This will use a little bit of system processing power, but I think is a neccessary component to include.
                            vehicle.CalculatePrice();
                            vehicle.SetPriceCategory();
                            Console.WriteLine($"ID: {key}, Vehicle Type: {vehicle.GetVehicleType()}\nYear: {vehicle.ModelYear}\nManufacturer: {vehicle.Manufacturer}\nModel: {vehicle.Model}\nPaint: {vehicle.DisplayColour()}\nRegistration: {vehicle.DisplayReg()}\nPrice: £{vehicle.GetPrice()}/month\nPrice Category: [{vehicle.GetPriceCategory()}]\n");
                        }
                        else if (type == "all")
                        {
                            counter++;
                            vehicle.CalculatePrice();
                            vehicle.SetPriceCategory();
                            Console.WriteLine($"ID: {key}, Vehicle Type: {vehicle.GetVehicleType()}\nYear: {vehicle.ModelYear}\nManufacturer: {vehicle.Manufacturer}\nModel: {vehicle.Model}\nPaint: {vehicle.DisplayColour()}\nRegistration: {vehicle.DisplayReg()}\nPrice: £{vehicle.GetPrice()}/month\nPrice Category: [{vehicle.GetPriceCategory()}]\n");
                        }
                    }
                    Console.WriteLine($"{counter} Results found\n");
                }
                else { Console.WriteLine("No vehicles available to rent."); }
            }

            // This function will display the currently rented vehicles of the currently logged in customer.
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
                        userVehicle.CalculatePrice();
                        userVehicle.SetPriceCategory();
                        Console.WriteLine($"Type: {userVehicle.GetVehicleType()}\nManufacturer: {userVehicle.Manufacturer}\nModel: {userVehicle.Model}\nYear: {userVehicle.ModelYear}\nColour: {userVehicle.DisplayColour()}\nRegistration: {userVehicle.DisplayReg()}\nPrice: £{userVehicle.GetPrice()}/month\nPrice Category: [{userVehicle.GetPriceCategory()}]\n\n");
                    }
                }
                // Check if there are no vehicles to display to the user
                if (!rentedVehicles.CombinedVehicles.Any(vehicle => vehicle.RentedBy == currentUser.AccessCode))
                {
                    Console.WriteLine("\nYou have no vehicles to display.\n");
                }
            }

            // This function is used to register the name of customers, and makes use of a tuple to fit everything into one function.
            (string FirstName, string LastName) RegisterName() // Uses a tuple to return to strings. Easier than creating two functions.
            {
                Console.WriteLine("Enter your firstname: ");
                string fname = Console.ReadLine().Trim();

                Console.WriteLine("Enter your lastname: ");
                string lname = Console.ReadLine().Trim();

                return (fname, lname);
            }

            // Used to verify if a customer code is found in the system or not.
            bool VerifyIdentity(string code)
            {
                // All customers are unique in the system as they are stored in a hash collection, allowing me to use a basic lookup to verify a staff code.
                Customer selectedCustomer = customers.FirstOrDefault(c => c.AccessCode == code);
                if (selectedCustomer != null)
                {
                    currentUser = selectedCustomer;
                    return true;
                }
                else { return false; }
            }

            // Used to verify if a staff code is found in the system or not.
            bool VerifyStaffCode(string code)
            {
                // All staff are unique in the system as they are stored in a hash collection, allowing me to use a basic lookup to verify a staff code.
                Staff selectedStaff = staff.FirstOrDefault(s => s.AccessCode == code);
                if (selectedStaff != null) { return true; }
                else { return false; }
            }

            // Used for looking throw the vehicles dictionary to find a specific vehicle by reg
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

            // Handles remove vehicles from the system. (This is a staff function)
            void removeVehicleByID(string vehID)
            {
                // Finds the first vehicle that has a key matching the reg plate. I can do it like this because I have implementation validation
                // to all vehicle dictionaries to ensure that there can not be a duplicate reg. Also I believe that dictionaries will throw
                // an error if there is a duplicate key. Making this method viable.
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

            // This function will handling the renting of different vehicle types
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

            // This function is for handling the returning of different vehicle types
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

            // This function will be ran if the program is executed through commandline. All of the CL functionality will be handled here
            void RunCommandLine()
            {
                if (args.Contains("-v"))
                {
                    // I chose the version number based of the number of git commits I had made to this repo, and added a 1.00 onto it so if I had 60
                    // commits it would be 1.60 & 100 being 2.0 etc (Thought this was a nice way of doing it).
                    Console.WriteLine("Version Number: 1.60\n\n[If you are trying to run another command remove '-v' from your arguments]");
                }
                else if (args.Contains("-h"))
                {
                    // Provided a -h help command to ensure that users making use of the command line feature will have some clue on what commands they can
                    // make use of.
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

            // This function is similar to the one below however will handle searching, and other filter methods such as manufacturer
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

                    var selectedVehicle = vehicles.FirstOrDefault(v => v.Key == vehID); // Searched the vehicles list to find a vehicle by reg

                    Console.WriteLine(FindVehicleByID(vehID));
                }
                else if (choice == "filter" || choice == "2" || choice == "f")
                {
                    // Used an enumerable here as a temp collection to store a list of filtered vehicles from the main vehicles dictionary.
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
                                Console.WriteLine($"Vehicle Type: {vehicle.GetVehicleType()}\nYear: {vehicle.ModelYear}\nManufacturer: {vehicle.Manufacturer}\nModel: {vehicle.Model}\nRegistration: {vehicle.Reg.Reg}\nCondition: {vehicle.Condition}\nPrice: {vehicle.GetPrice()}\nPrice Category: [{vehicle.GetPriceCategory()}]\n");
                            }
                        }
                        else { Console.WriteLine("No vehicles found"); }
                    }
                    else if (filterChoice == "year" || filterChoice == "2" || filterChoice == "y")
                    {
                        Console.WriteLine("Enter specific year (or try 'before 2005' / 'after 2005'): ");
                        string inputYear = Console.ReadLine().Trim().ToLower();

                        foreach (var vehicle in FindVehiclesByYear(inputYear))
                        {
                            Console.WriteLine($"Vehicle Type: {vehicle.GetVehicleType()}\nYear: {vehicle.ModelYear}\nManufacturer: {vehicle.Manufacturer}\nModel: {vehicle.Model}\nRegistration: {vehicle.Reg.Reg}\nCondition: {vehicle.Condition}\nPrice: {vehicle.GetPrice()}\nPrice Category: [{vehicle.GetPriceCategory()}]\n");
                        }
                    }
                    else if (filterChoice == "value" || filterChoice == "3" || filterChoice == "v")
                    {
                        Vehicle bestValue = FindBestValue(vehicles);

                        if (bestValue != null)
                        {
                            Console.WriteLine($"Best value vehicle based on condition and model year criteria:");
                            Console.WriteLine($"- Type: {bestValue.GetVehicleType()}\n{bestValue.Manufacturer} {bestValue.Model} ({bestValue.ModelYear})\nRegistration: {bestValue.Reg.Reg}\nCondition: {bestValue.Condition}\nPrice: {bestValue.GetPrice()}\nPrice Category: [{bestValue.GetPriceCategory()}]");
                        }
                        else { Console.WriteLine($"No vehicles to filter."); }
                    }
                }
            }

            // This function has been included to allow the user to filter the available vehicle list by year.
            List<Vehicle> FindVehiclesByYear(string year)
            {
                List<Vehicle> foundVehicles = new();
                if (year.Contains("before"))
                {
                    // The below replace methods have been included as the string will = before{year} so if it contains before or after
                    // the program will remove it allowing for the program to convert the integer properly.
                    year = year.Replace("before", "").Trim();
                    int targetYear = 0;
                    try { targetYear = Convert.ToInt32(year); } // The below try catches have been included to validate an exception
                    catch (FormatException)                     // that I was getting when inputting invalid commands
                    {
                        Console.WriteLine("[ERROR] - Invalid command format: (try 'before 2005' or 'before 2020')");
                    }
                    if (targetYear > 0)
                    {
                        // All of the algorithms in this function have been included to search and filter through the vehicles list to find
                        // objects that are meeting the users search criteria.
                        // I used an algorithm here to generate a list of vehicles based off of the search criteria, as I believe that it is the simplest
                        // way to implement this search, using the least amount of required code. Also this is much faster than running a foreach loop here
                        // to run conditions on each vehicle. This does a similar thing but faster and with less code.
                        foundVehicles = vehicles
                            .Where(v => v.Value.ModelYear < targetYear)
                            .Select(v => v.Value)
                            .ToList();
                    }
                    else { Console.WriteLine("Invalid year: (Year must be greater than 0)"); }
                }
                else if (year.Contains("after"))
                {
                    year = year.Replace("after", "").Trim();
                    int targetYear = 0;
                    try { targetYear = Convert.ToInt32(year); }
                    catch (FormatException)
                    {
                        Console.WriteLine("[ERROR] - Invalid command format: (try 'after 2005' or 'after 2020')");
                    }
                    if (targetYear > 0)
                    {
                        foundVehicles = vehicles
                            .Where(v => v.Value.ModelYear > targetYear)
                            .Select(v => v.Value)
                            .ToList();
                    }
                    else { Console.WriteLine("Invalid year: (Year must be greater than 0)");  }
                }
                else
                {
                    int targetYear = 0;
                    try { targetYear = Convert.ToInt32(year); }
                    catch (FormatException)
                    {
                        Console.WriteLine("[ERROR] - Invalid command format: (try '2005' or '2020')");
                    }
                    if (targetYear > 0)
                    {
                        foundVehicles = vehicles
                            .Where(v => v.Value.ModelYear == targetYear)
                            .Select(v => v.Value)
                            .ToList();
                    }
                    else { Console.WriteLine("Invalid year: (Year must be greater than 0)"); }
                }
                return foundVehicles;
            }

            // This function is for creating new vehicle and has been made to handle different types of vehicle type inputs,
            // so that the user can create different types of vehicles.
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

            // Function reads from the json file containing staff data and serialises it into a hash
            void ReadStaffJSON(string fileName, HashSet<Staff> hash)
            {
                string jsonContent = File.ReadAllText(fileName);
                var items = JsonSerializer.Deserialize<HashSet<Staff>>(jsonContent);

                foreach (var item in items)
                {
                    hash.Add(item);
                }
            }

            // Function reads from the json file containing customer data.
            void ReadCustomerJSON(string fileName, HashSet<Customer> hash)
            {
                string jsonContent = File.ReadAllText(fileName);
                var items = JsonSerializer.Deserialize<HashSet<Customer>>(jsonContent);

                foreach (var item in items)
                {
                    hash.Add(item);
                }
            }

            // Function writes different types of rented vehicles to their respective json files.
            void WriteRentedVehicleJSON<T>(List<T> dictionary, string fileName)
            {
                string jsonContent = JsonSerializer.Serialize(dictionary, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(fileName, jsonContent);
            }

            // Function been created in a modular way, which will allow me to read different types of vehicles from different json files.
            // This makes this function reusable.
            void ReadRentedVehicleJSON<T>(string fileName, List<T> rentedList, string vehicleType) where T : Vehicle
            {
                if (File.Exists(fileName) && new FileInfo(fileName).Length > 0)
                {
                    rentedList.Clear();
                    string jsonContent = File.ReadAllText(fileName);
                    var rentedVehicles = JsonSerializer.Deserialize<List<T>>(jsonContent);

                    foreach (var vehicle in rentedVehicles)
                    {
                        // This checks if the reg that is trying to be added to the list already exists, if it is, then the vehicle will
                        // not be added. This is to prevent duplicated data.
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

            // Sums up the vehicles weighted value based off of it's condition and year it was made
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
                        // This basically creates a weighted value from the vehicles condition and it's model year and then runs the
                        // calculation through the function and then will return the weighted value then that will be used to find the
                        // the vehicle that is closest to that value.
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
                    // I added this condition to check is a vehicle with the same reg number already existed and stopped it from being
                    // added to stop duplicated data and help improve system performance.
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
                vehicles.Clear(); // I clear the list here to ensure that when the list is updated durin runtime, I am getting a properly updated list from the file as the data is wrote before it is updated
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