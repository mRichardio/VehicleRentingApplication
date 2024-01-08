using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace VehicleRentingApplication
{
    internal class Colour
    {
        // Added validation to these values, even though I have included validation in the create colour form,
        // this protects the colours from being out of range if some other implementation interacts with these values as they are public.
        public int Red
        {
            get
            {
                return Red;
            }
            set
            {
                if (Red > 255 || Red < 0)
                {
                    Red = 0;
                }
                else { Red = value; }
            }
        }
        public int Green 
        {
            get
            {
                return Green;
            }
            set
            {
                if (Green > 255 || Green < 0)
                {
                    Red = 0;
                }
                else { Green = value; }
            }
        }
        public int Blue 
        {
            get
            {
                return Blue;
            }
            set
            {
                if (Blue > 255 || Blue < 0)
                {
                    Blue = 0;
                }
                else { Blue = value; }
            }
        }

        public Colour()
        {
            // Default constructor for deserialization
        }

        public Colour(int r, int g, int b)
        {
            Red = r;
            Green = g;
            Blue = b;
        }

        private int GetValidColour(string colorComponent)
        {
            while (true)
            {
                Console.WriteLine($"Enter {colorComponent} colour value (RGB): ");
                try
                {
                    int colorValue = Convert.ToInt32(Console.ReadLine());
                    if (colorValue > 255 || colorValue < 0)
                    {
                        Console.WriteLine("[ERROR] Colour value must be within the range of (0-255)");
                    }
                    else
                    {
                        return colorValue;
                    }
                }
                catch (FormatException)
                {
                    Console.WriteLine("[Error]: Invalid input (make sure to only enter integers)");
                }
            }
        }

        public Colour CreateColour()
        {
            int red = GetValidColour("R");
            int green = GetValidColour("G");
            int blue = GetValidColour("B");

            return new Colour(red, green, blue);
        }

        public int GetRed() { return this.Red; }
        public int GetGreen() { return this.Green; }
        public int GetBlue() { return this.Blue; }
    }
}
