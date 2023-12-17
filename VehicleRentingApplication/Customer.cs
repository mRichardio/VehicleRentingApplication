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

        private List<Vehicle> rentedVehicles; // I have set this list to private so it can't be altered with.

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
            rentedVehicles = new List<Vehicle>(rentLimit);
            // List made with capacity to make better performance, also increases the scope of the application
            // as if the rent limit were to increase there could be performance issues.
        }

        public override string GetType() { return "Customer"; }
        public List<Vehicle> GetRentedVehicles() { return this.rentedVehicles; }
        public void AddRentedVehicle(Vehicle vehicle) { rentedVehicles.Add(vehicle); }
    }
}
