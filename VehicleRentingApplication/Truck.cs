using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace VehicleRentingApplication
{
    internal class Truck : Vehicle
    {
        public float StorageCapacity { get; set; }
        public int DoorCount { get; set; }

        private string priceCategory;

        //public override string type => "Truck"; // This was my old implementation of setting the type. However, I changes this to
        // a public get method in the parent class to prevent changes.

        public Truck()
        {

        }

        public Truck(string manufacturer, string model, int modelYear, bool isAutomatic, int doorCount, float storageCapacity, Colour paint, Registration reg, float condition)
        {
            this.Manufacturer = manufacturer;
            this.Model = model;
            this.ModelYear = modelYear;
            this.IsAutomatic = isAutomatic;
            this.StorageCapacity = storageCapacity;
            this.DoorCount = doorCount;
            this.Paint = paint;
            this.Reg = reg;
            this.Condition = condition;
        }

        public override string GetVehicleType()
        {
            return "Truck";
        }

        public string GetPriceCategory() 
        {
            return priceCategory;
        }

        public override void CalculatePrice() // Truck prices are calculated differently
        {
            double price = 0;
            price += ModelYear;
            price /= Condition / 10;
            price += StorageCapacity / 8;
            price *= 1.2;
            this.price = Math.Round(price, 2);
        }

        public override Truck CreateVehicle()
        {
            Console.WriteLine("---| Truck Creation |---\n");

            Console.WriteLine("Manufacturer:");
            string manufacturer = Console.ReadLine();

            Console.WriteLine("Model:");
            string model = Console.ReadLine();

            Console.WriteLine("Is the vehicle automatic: ");
            bool isAuto;
            while (true)
            {
                string input = Console.ReadLine().Trim().ToLower();
                if (input == "yes" || input == "y")
                {
                    isAuto = true;
                    break;
                }
                else if (input == "no" || input == "n")
                {
                    isAuto = false;
                    break;
                }
                else
                {
                    Console.WriteLine("Invalid input (try 'yes' or 'no')");
                }
            }

            Console.WriteLine("Enter door count: ");
            int doors = 0;
            try
            {
                doors = Convert.ToInt32(Console.ReadLine());
            }
            catch (FormatException)
            {
                Console.WriteLine("[ERROR] Invalid input, please input an integer number");
            }

            Console.WriteLine("Storage Capacity: ");
            float storageCapacity = 0.0f;
            while (true)
            {
                Console.Write("Enter a float number: ");
                string userInput = Console.ReadLine();

                if (float.TryParse(userInput, out float floatValue))
                {
                    Console.WriteLine($"You entered: {floatValue}");

                    if (floatValue >= 0.0f)
                    {
                        storageCapacity = floatValue;
                        break;
                    }
                    else { Console.WriteLine("Input is out of range. (Enter a positive number!)"); }
                }
                else { Console.WriteLine("Invalid input. Please enter a valid float number."); }
            }


            int year = 0;
            while (true)
            {
                try
                {
                    Console.WriteLine("Enter manufacture year: ");
                    year = Convert.ToInt32(Console.ReadLine());
                    break; // Move the break statement here to ensure it breaks only when the input is valid
                }
                catch (FormatException)
                {
                    Console.WriteLine("[Error]: Invalid input (make sure to only enter integers)");
                }
            }

            float condition = 0f;
            while (true)
            {
                Console.WriteLine("What condition is the vehicle in (%)");
                try
                { condition = Convert.ToSingle(Console.ReadLine()); }
                catch (Exception)
                { Console.WriteLine("Invalid conition make sure that the value you enter is a floating value... 1-100%"); }
                if (condition <= 100 && condition >= 0) { break; }
                else { Console.WriteLine("Condition value is out of range.. (range: 1 - 100)"); }
            }

            Colour colour = new Colour();
            colour = colour.CreateColour();

            Registration reg = new Registration();
            reg = reg.CreateReg();

            Truck truck = new Truck(manufacturer, model, year, isAuto, doors, storageCapacity, colour, reg, condition);

            return truck;
        }
    }
}
