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

        public int DoorCount { get; set; }

        private string priceCategory;



        //public override string type => "Car"; // Originally I had this method setting each type of vehicle, however,
        // I didn't want the type of vehicle to be changable so instead I set
        // the type to be protected within the vehicle abstract class and have
        // provided a function for each vehicle to recieve, the type

        public Car()
        {
        }


        public Car(int modelYear, bool isAutomatic, int doorCount, string manufacturer, string model, Colour paint, Registration reg, float condition)
        {
            this.ModelYear = modelYear;
            this.Manufacturer = manufacturer;
            this.Model = model;
            this.DoorCount = doorCount;
            this.IsAutomatic = isAutomatic;
            this.Paint = paint;
            this.Reg = reg;
            this.Condition = condition;
        }

        public override string GetVehicleType()
        {
            return "Car";
        }

        public string GetPriceCategory()
        {
            return priceCategory;
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
