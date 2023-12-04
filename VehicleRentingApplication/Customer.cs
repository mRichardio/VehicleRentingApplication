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

        public Customer() // constructor for registering customers
        {
            this.accessCode = GenerateAccessCode();
            this.rentLimit = 3;
            this.vehicleCount = 0;
        }

        public Customer(string fname, string lname, string accessCode)
        {
            this.accessCode = accessCode;
            this.firstName = fname;
            this.lastName = lname;
            this.rentLimit = 3;
            this.vehicleCount = 0;
        }

        public override string GetType() { return "Customer"; }

        public void RegisterName() // This function is only used here as staff wont be able to register themselves.
        {
            Console.WriteLine("Enter your firstname: ");
            this.firstName = Console.ReadLine().Trim();

            Console.WriteLine("Enter your lastname: ");
            this.lastName = Console.ReadLine().Trim();
        }
    }
}
