using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VehicleRentingApplication
{
    internal class Truck : Vehicle
    {
        public int doorCount { get; set; }
        public int wheelCount { get; set; }
        public int storageCapacity { get; set; }
        public override string type => "Truck";

        public Truck(string type, int modelYear, int doorCount, bool isAutomatic, int wheelCount, int storageCapacity, Colour paint, Registration reg)
        {
            this.type = type;
            this.modelYear = modelYear;
            this.doorCount = doorCount;
            this.isAutomatic = isAutomatic;
            this.wheelCount = wheelCount;
            this.storageCapacity = storageCapacity;
            this.paint = paint;
            this.reg = reg;
        }
    }
}
