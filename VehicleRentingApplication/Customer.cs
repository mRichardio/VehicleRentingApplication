using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace VehicleRentingApplication
{
    internal class Customer : Account
    {
        public int rentLimit {get; private set;}
        public int vehicleCount { get; private set; }

        public Customer()
        {
            this.rentLimit = 3;
        }

        public Customer(string fname, string lname)
        {
            this.accessCode = GenerateAccessCode();
            this.firstName = fname;
            this.lastName = lname;
            this.rentLimit = 3;
            this.vehicleCount = 0;
        }

        public override string GetType() { return "Customer"; }
    }
}
