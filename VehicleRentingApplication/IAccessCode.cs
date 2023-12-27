using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VehicleRentingApplication
{
    internal interface IAccessCode
    {
        // Generates Access Code
        public string GenerateAccessCode() { return "GeneratedCode"; }
    }
}
