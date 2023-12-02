using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VehicleRentingApplication
{
    abstract internal class Vehicle
    {
        protected string type = "null"; // Set this in sub-class (e.g type = car)
        protected int modelYear = 0;
        protected string manufacturer;
        protected string model;
        protected int engineSize = 0; // BHP
        protected bool isAutomatic;
        protected Colour paint;
        protected Registration reg;
    }
}
