using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace VehicleRentingApplication
{
    internal class Colour
    {
        private int r;
        private int g;
        private int b;

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
                red = int.Parse(Console.ReadLine());
                if (red > 255 || red < 0) { Console.WriteLine("[ERROR] Colour value must be within the range of (0-255)"); }
                else
                {
                    try { red = Convert.ToInt32(Console.ReadLine()); }
                    catch (FormatException) { Console.WriteLine("[Error]: Invalid input (make sure to only enter integers)"); }
                    break;
                }
            }
            while (true)
            {
                Console.WriteLine("Enter RGB colour values:\nG:");
                green = int.Parse(Console.ReadLine());
                if (red > 255 || red < 0) { Console.WriteLine("[ERROR] Colour value must be within the range of (0-255)"); }
                else
                {
                    try { green = Convert.ToInt32(Console.ReadLine()); }
                    catch (FormatException) { Console.WriteLine("[Error]: Invalid input (make sure to only enter integers)"); }
                    break;
                }
            }
            while (true)
            {
                Console.WriteLine("Enter RGB colour values:\nB:");
                blue = int.Parse(Console.ReadLine());
                if (red > 255 || red < 0) { Console.WriteLine("[ERROR] Colour value must be within the range of (0-255)"); }
                else
                {
                    try { blue = Convert.ToInt32(Console.ReadLine()); }
                    catch (FormatException) { Console.WriteLine("[Error]: Invalid input (make sure to only enter integers)"); }
                    break;
                }
            }
            return new Colour(red, green, blue);
        }

        public int GetRed() { return this.r; }
        public int GetGreen() { return this.g; }
        public int GetBlue() { return this.b; }
    }
}
