using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VehicleRentingApplication
{
    internal class Car : Vehicle
    {
        private int doorCount;
        private int wheelCount; // This is not in base because some vehicles don't have wheels!

        public Car(int modelYear, int wheelCount, int doorCount, bool isAutomatic, Colour paint, Registration reg, string manufacturer, string model)
        {
            type = "Car";
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
