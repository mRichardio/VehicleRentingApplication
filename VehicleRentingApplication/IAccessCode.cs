using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VehicleRentingApplication
{
    internal interface IAccessCode
    {
        // I added this interface because I wanted to ensure that all classes deriving from the Account class would maintain the access code functionality.
        // Also, I wanted to demonstrate that while interfaces are similar to abstract classes, they have the added benefit of a class being able to,
        // inherit from multiple different interfaces rather than just 1 single abstract class.

        // Generates Access Code
        public string GenerateAccessCode() { return "GeneratedCode"; }
    }
}
