using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VehicleRentingApplication
{
    internal class Motorbike : Vehicle
    {
        public override string type => "Motorbike";

        public Motorbike()
        {

        }

        public Motorbike(int modelYear, bool isAutomatic, string manufacturer, string model, Colour paint, Registration reg)
        {
            this.manufacturer = manufacturer;
            this.model = model;
            this.modelYear = modelYear;
            this.isAutomatic = isAutomatic;
            this.paint = paint;
            this.reg = reg;
        }

        public Motorbike CreateMotorbike()
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

            Colour colour = new Colour();
            colour = colour.CreateColour();

            Registration reg = new Registration();
            reg = reg.CreateReg();

            Motorbike motorbike = new Motorbike(year, isAuto, manufacturer, model, colour, reg);

            return motorbike;
        }
    }
}
