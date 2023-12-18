using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace VehicleRentingApplication
{
    internal class Customer : Account
    {
        public int rentLimit {get; private set;}
        public int vehicleCount { get; private set; }
        public List<Vehicle> rentedVehicles { get; set; }


        [JsonConstructor]
        public Customer() : base()
        {
            // Initialize properties if needed
            this.rentLimit = 3;
            this.vehicleCount = 0;
            rentedVehicles = new List<Vehicle>();
        }

        //public Customer(/* other parameters */)
        //{
        //    this.rentLimit = 3;
        //    // Initialize the rented vehicles list
        //    rentedVehicles = new List<Vehicle>();
        //}

        public Customer(string fname, string lname)
        {
            this.accessCode = GenerateAccessCode();
            this.firstName = fname;
            this.lastName = lname;
            this.rentLimit = 3;
            this.vehicleCount = 0;
            rentedVehicles = new List<Vehicle>();
            //this.rentedVehicles = new RentedVehicleHandler(this.rentLimit);
            // List made with capacity to make better performance, also increases the scope of the application
            // as if the rent limit were to increase there could be performance issues.
        }

        public override string GetType() { return "Customer"; }

        public List<Vehicle> GetRentedVehicles()
        {
            return this.rentedVehicles;
        }

        public void RentVehicle(Vehicle vehicle)
        {
            // Check if the customer has reached the rent limit
            if (rentedVehicles.Count < rentLimit)
            {
                rentedVehicles.Add(vehicle);
                vehicleCount++;
                Console.WriteLine("Vehicle rented successfully.");
            }
            else
            {
                Console.WriteLine($"You have reached the rent limit of {rentLimit}. Cannot rent more vehicles.");
            }
        }

        //public void RentVehicle(Vehicle vehicle)
        //{
        //    this.rentedVehicles.AddVehicle(vehicle);
        //}
    }
}
