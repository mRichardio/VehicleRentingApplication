using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace VehicleRentingApplication
{
    internal class Car : Vehicle
    {

        public int doorCount { get; set; }

        // Override the type property from the base class
        public override string type => "Car";

        public Car()
        {
            //CalculatePrice();
        }


        public Car(int modelYear, bool isAutomatic, int doorCount, string manufacturer, string model, Colour paint, Registration reg, float condition)
        {
            this.modelYear = modelYear;
            this.manufacturer = manufacturer;
            this.model = model;
            this.doorCount = doorCount;
            this.isAutomatic = isAutomatic;
            this.paint = paint;
            this.reg = reg;
            this.condition = condition;
        }

        public override Car CreateVehicle()
        {
            Console.WriteLine("---| Car Creation |---\n");

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

            Car car = new Car(year, isAuto, doors, manufacturer, model, colour, reg, condition);

            return car;
        }
    }
}
