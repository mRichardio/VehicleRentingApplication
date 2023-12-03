using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VehicleRentingApplication
{
    internal class Colour
    {
        public int r { get; set; }
        public int g { get; set; }
        public int b { get; set; }

        public Colour() { /*Default*/ }

        public Colour(int r, int g, int b)
        {
            this.r = r;
            this.g = g;
            this.b = b;
        }

        public Colour CreateColour()
        {
            int red = 0;
            int green = 0;
            int blue = 0;

            while (true)
            {
                Console.WriteLine("Enter RGB colour values:\nR:");
                try { red = Convert.ToInt32(Console.ReadLine()); }
                catch (FormatException) { Console.WriteLine("[Error]: Invalid input (make sure to only enter integers)"); }
                break;
            }
            while (true)
            {
                Console.WriteLine("Enter RGB colour values:\nR:");
                try { green = Convert.ToInt32(Console.ReadLine()); }
                catch (FormatException) { Console.WriteLine("[Error]: Invalid input (make sure to only enter integers)"); }
                break;
            }
            while (true)
            {
                Console.WriteLine("Enter RGB colour values:\nR:");
                try { blue = Convert.ToInt32(Console.ReadLine()); }
                catch (FormatException) { Console.WriteLine("[Error]: Invalid input (make sure to only enter integers)"); }
                break;
            }
            return new Colour(red, green, blue);
        }
    }
}
