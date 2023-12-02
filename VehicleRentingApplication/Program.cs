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
            // TODO

            // - Use Parallel Execution (If I use this then I need to explain why I have used it.
            // E.g. I use single thread and got a response time slower than when using parallel execution.))

            // - JSON Serialisation for all lists e.g vehicles need to be stored in json files along with customers
            // These lists should be dumped and loaded to and from the txt file each time a new process is done e.g a new customer is added.

            // Better hierarchy {In Progress}

            // IMPORTANT

            // FIX TRUCK/MOTORBIKE CLASS TO BE MORE INLINE WITH CAR CLASS <<<<< // todo
            // MAKE TRUCK/MOTORBIKES READABLE AND WRITABLE THEN ADD THOSE OBJECTS TO THE VEHICLES LIST. <<<<< // todo

            int selected; // For menu selection
            Customer currentUser = null;
            
            Dictionary<int, Users> users = new Dictionary<int, Users>(); // Stores all of the users of the system e.g. Staff, Customers

            Dictionary<string, Car> cars = new Dictionary<string, Car>();
            //Dictionary<string, Truck> trucks = new Dictionary<string, Truck>();
            //Dictionary<string, Motorbike> motorbikes = new Dictionary<string, Motorbike>();
            Dictionary<string, Vehicle> vehicles = new();

            AddUser(new Customer("Matthew", "Richards")); // REMOVE AFTER TESTING

            //JsonSerializerOptions options = new JsonSerializerOptions
            //{
            //    Converters = { new VehicleConverter() },
            //    WriteIndented = true
            //};

            // Read

            string jsonReadCars = File.ReadAllText("cars.json");

            cars = JsonSerializer.Deserialize<Dictionary<string, Car>>(jsonReadCars/*, options*/);

            //AddCar(new Car(2002, 4, 5, true, "Audi", "A3", new Colour(255, 255, 255), new Registration("BD51 SMR"))); // REMOVE AFTER TESTING
            //AddCar(new Car(2015, 4, 5, false, "Nissan", "GTR", new Colour(255, 0, 255), new Registration("RG38 KJE"))); // REMOVE AFTER TESTING
            //AddCar(new Car(2022, 4, 3, true, "Ford", "Focus", new Colour(255, 0, 0), new Registration("JU24 OPE"))); // REMOVE AFTER TESTING


            foreach (var car in cars)
            {
                vehicles.Add(car.Key, car.Value);
            }

            //// Check if the deserialization was successful
            //if (cars != null)
            //{
            //    foreach (var kvp in vehicles)
            //    {
            //        string key = kvp.Key;
            //        Vehicle vehicle = kvp.Value;

            //        Console.WriteLine($"Key: {key}, Type: {vehicle.type}, ModelYear: {vehicle.modelYear}, Manufacturer: {vehicle.manufacturer}, Model: {vehicle.model}");
            //    }
            //}
            //else
            //{
            //    Console.WriteLine("Failed to deserialize JSON data.");
            //}

            // Write
            string jsonWrite = JsonSerializer.Serialize(cars, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText("cars.json", jsonWrite);

            // Program
            while (true)
            {
                Menu mainMenu = new Menu(new string[] { "View Vehicles", "Your Vehicles", "View Profile" });
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

                        if (cars != null)
                        {
                            foreach (var kvp in vehicles)
                            {
                                string key = kvp.Key;
                                Vehicle vehicle = kvp.Value;

                                Console.WriteLine($"Key: {key}, Type: {vehicle.type}, ModelYear: {vehicle.modelYear}, Manufacturer: {vehicle.manufacturer}, Model: {vehicle.model}");
                            }
                        }
                        else
                        {
                            Console.WriteLine("They are currentlty no vehicles available to rent.");
                        }
                        Console.WriteLine("Press Any button to continue....");
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

                    default:
                        Console.Clear();
                        Console.WriteLine("Invalid input. Please enter a number between 1 and 3.");
                        break;
                }
            }

            // Functions

            void AddUser(Users user) { users.Add(users.Count + 1, user); }

            void AddCar(Car car) { cars.Add($"C-{cars.Count+1}", car); }
            //void AddVehicle(Vehicle vehicle) { vehicles.Add(vehicles.Count + 1, vehicle); }
            //void AddVehicle(Vehicle vehicle) { vehicles.Add(vehicles.Count + 1, vehicle); }

            // return users.Exists(customer => customer.username == user && customer.password == pass); // Good for finding people!
        }

        //internal class VehicleConverter : JsonConverter<Vehicle>
        //{
        //    public override Vehicle Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        //    {
        //        // Read the JSON data and determine the concrete type (e.g., Car)
        //        using (JsonDocument doc = JsonDocument.ParseValue(ref reader))
        //        {
        //            JsonElement root = doc.RootElement;
        //            string type = root.GetProperty("type").GetString();

        //            return type switch
        //            {
        //                "Car" => JsonSerializer.Deserialize<Car>(root.GetRawText(), options),
        //                _ => throw new JsonException($"Unsupported vehicle type: {type}")
        //            };
        //        }
        //    }

        //    public override void Write(Utf8JsonWriter writer, Vehicle value, JsonSerializerOptions options)
        //    {
        //        // Serialize the vehicle object
        //        JsonSerializer.Serialize(writer, value, value.GetType(), options);
        //    }
        //}
    }
}