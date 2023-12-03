using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VehicleRentingApplication
{
    internal class Motorbike : Vehicle
    {
        public int wheelCount { get; set; }

        public Motorbike(string type, int modelYear, bool isAutomatic, int wheelCount, Colour paint, Registration reg)
        {
            this.type = type;
            this.modelYear = modelYear;
            this.isAutomatic = isAutomatic;
            this.wheelCount = wheelCount;
            this.paint = paint;
            this.reg = reg;
        }
    }
}
