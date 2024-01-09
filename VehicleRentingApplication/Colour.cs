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
        private int red;
        public int Red
        {
            get
            {
                return red;
            }
            set
            {
                if (value > 255 || value < 0)
                {
                    this.red = 0;
                }
                else { this.red = value; }
            }
        }
        private int green;
        public int Green 
        {
            get
            {
                return green;
            }
            set
            {
                if (value > 255 || value < 0)
                {
                    this.green = 0;
                }
                else { this.green = value; }
            }
        }
        private int blue;
        public int Blue 
        {
            get
            {
                return blue;
            }
            set
            {
                if (value > 255 || value < 0)
                {
                    this.blue = 0;
                }
                else { this.blue = value; }
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

        // This function is used within vehicle creation to ensure that when a colour is being set by a user, that the colour is within a specifc
        // RGB, colour range.
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

        // These two functions basically work together to display within the code. This implementation reduces
        // redundant and duplicated code.
        public Colour CreateColour()
        {
            int red = GetValidColour("R");
            int green = GetValidColour("G");
            int blue = GetValidColour("B");

            return new Colour(red, green, blue);
        }
    }
}
