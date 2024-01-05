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
        public int ModelYear { get; set; }
        public string Manufacturer { get; set; }
        public string Model { get; set; }
        public float Condition { get; set; }
        public bool IsAutomatic { get; set; }
        public Colour Paint { get; set; }
        public Registration Reg { get; set; }
        public string RentedBy { get; set; }

        // Price of the vehicle is calculated during runtime, to account for changing values, etc. (Accounts for vehicle conditions changing e.g. better scope)
        protected double price; // This is protected so that it can be used with sub-classes and so that JSON can't serialise it.

        public string DisplayColour()
        {
            return $" R:{Paint.Red} G:{Paint.Green} B:{Paint.Blue}";
        }

        public string DisplayReg()
        {
            return $"{Reg.Reg}";
        }

        public Vehicle()
        {

        }

        public Vehicle(string type, int modelYear, string manufacturer, string model, int condition, bool isAutomatic, Colour paint, Registration reg, string rentedBy)
        {
            this.type = type;
            this.ModelYear = modelYear;
            this.Manufacturer = manufacturer;
            this.Model = model;
            this.Condition = condition;
            this.IsAutomatic = isAutomatic;
            this.Paint = paint;
            this.Reg = reg;
            this.RentedBy = rentedBy;
        }

        public virtual void CalculatePrice()
        {
            double price = 0;
            price += ModelYear;
            price /= Condition / 10;
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
