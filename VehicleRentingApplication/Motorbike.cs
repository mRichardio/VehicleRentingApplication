using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VehicleRentingApplication
{
    internal class Motorbike : Vehicle
    {

        public Motorbike()
        {
        }

        public Motorbike(int modelYear, bool isAutomatic, string manufacturer, string model, Colour paint, Registration reg, float condition)
        {
            this.Manufacturer = manufacturer;
            this.Model = model;
            this.ModelYear = modelYear;
            this.IsAutomatic = isAutomatic;
            this.Paint = paint;
            this.Reg = reg;
            this.Condition = condition;
        }

        public override string GetVehicleType()
        {
            return "Motorbike";
        }

        public override Motorbike CreateVehicle()
        {
            Console.WriteLine("---| Motorbike Creation |---\n");

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

            Motorbike motorbike = new Motorbike(year, isAuto, manufacturer, model, colour, reg, condition);

            return motorbike;
        }
    }
}
