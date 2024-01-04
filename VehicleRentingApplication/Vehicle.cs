using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace VehicleRentingApplication
{
    internal abstract class Vehicle
    {
        protected virtual string type { get; set; } // virtual so it can be overridden in derived classes
        public int modelYear { get; set; }
        public string manufacturer { get; set; }
        public string model { get; set; }
        public float condition { get; set; }
        public bool isAutomatic { get; set; }
        public Colour paint { get; set; }
        public Registration reg { get; set; }
        public string rentedBy { get; set; }

        // Price of the vehicle is calculated during runtime, to account for changing values, etc. (Accounts for vehicle conditions changing e.g. better scope)
        protected double price; // This is protected so that it can be used with sub-classes and so that JSON can't serialise it.

        public string DisplayColour()
        {
            return $" R:{paint.Red} G:{paint.Green} B:{paint.Blue}";
        }

        public string DisplayReg()
        {
            return $"{reg.reg}";
        }

        public Vehicle()
        {

        }

        public Vehicle(string type, int modelYear, string manufacturer, string model, int condition, bool isAutomatic, Colour paint, Registration reg, string rentedBy)
        {
            this.type = type;
            this.modelYear = modelYear;
            this.manufacturer = manufacturer;
            this.model = model;
            this.condition = condition;
            this.isAutomatic = isAutomatic;
            this.paint = paint;
            this.reg = reg;
            this.rentedBy = rentedBy;
        }

        public virtual void CalculatePrice()
        {
            double price = 0;
            price += modelYear;
            price /= condition / 10;
            price *= 1.2;
            this.price = Math.Round(price, 2);
        }

        public double GetPrice()
        {
            return this.price;
        }

        public virtual Vehicle CreateVehicle()
        {
            Console.WriteLine("---| Vehicle Creation |---\n");

            // Virtual member function for vehicle creation.

            return null;
        }

        public virtual string GetVehicleType() // Virtual so that deriving classes can alter this functionality.
        {
            return "Vehicle";
        }
    }
}
