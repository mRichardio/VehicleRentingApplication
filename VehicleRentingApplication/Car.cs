using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace VehicleRentingApplication
{
    internal class Car : Vehicle
    {
        public int doorCount { get; set; }
        public int wheelCount { get; set; }

        // Override the type property from the base class
        public override string type => "Car";

        public Car(int modelYear, int wheelCount, int doorCount, bool isAutomatic, string manufacturer, string model, Colour paint, Registration reg)
        {
            this.modelYear = modelYear;
            this.manufacturer = manufacturer;
            this.model = model;
            this.wheelCount = wheelCount;
            this.doorCount = doorCount;
            this.isAutomatic = isAutomatic;
            this.paint = paint;
            this.reg = reg;
        }
    }
}
