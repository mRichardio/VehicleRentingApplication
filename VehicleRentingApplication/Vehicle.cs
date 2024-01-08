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
        protected virtual string type { get; set; } // virtual so it can be overridden in derived classes.
        public int ModelYear { get; set; }
        public string Manufacturer { get; set; }
        public string Model { get; set; }
        public float Condition { get; set; }
        public bool IsAutomatic { get; set; }
        public Colour Paint { get; set; } // Validation is handled in the actual Colour class.
        public Registration Reg { get; set; } // Validation is handled in actual Registration class.
        public string RentedBy { get; set; }

        // Price of the vehicle is calculated during runtime, to account for changing values, etc. (Accounts for vehicle conditions changing e.g. better scope)
        protected double price; // This is protected so that it can be used with sub-classes and so that JSON can't serialise it.
        protected virtual string priceCategory { get; set; }


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
        public string DisplayColour()
        {
            return $" R:{Paint.Red} G:{Paint.Green} B:{Paint.Blue}";
        }

        public string DisplayReg()
        {
            return $"{Reg.Reg}";
        }

        public virtual void CalculatePrice()
        {
            double price = 0;
            price += ModelYear;
            price /= Condition / 5;
            price *= 1.2;
            this.price = Math.Round(price, 2);
        }
        public double GetPrice()
        {
            return this.price;
        }

        // Generates the price categeory of vehicles during runtime based off of their properties
        public void SetPriceCategory()
        {
            if (Condition >= 75 && GetPrice() >= 300) { this.priceCategory = "Luxury"; }
            else if (Condition >= 50 && Condition < 75 && GetPrice() < 300) { this.priceCategory = "Premium"; }
            else if (Condition >= 0 && Condition < 50 && GetPrice() < 170) { this.priceCategory = "Cheap"; }
            else { this.priceCategory = "Standard"; }
        }

        public string GetPriceCategory()
        {
            return this.priceCategory;
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
