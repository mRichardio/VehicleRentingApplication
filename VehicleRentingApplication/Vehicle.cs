using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace VehicleRentingApplication
{
    internal abstract class Vehicle
    {
        public virtual string type { get; set; } // Make it virtual so it can be overridden in derived classes
        public int modelYear { get; set; }
        public string manufacturer { get; set; }
        public string model { get; set; }
        public float condition { get; set; }
        public bool isAutomatic { get; set; }
        public Colour paint { get; set; }
        public Registration reg { get; set; }
        public string rentedBy { get; set; }

        public string DisplayColour()
        {
            return $" R:{paint.GetRed()} G:{paint.GetGreen()} B:{paint.GetBlue()}";
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

        public virtual Vehicle CreateVehicle()
        {
            Console.WriteLine("---| Vehicle Creation |---\n");

            // Virtual member function for vehicle creation.

            return null;
        }
    }
}
