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

        private List<Car> rentedCars; // I have set this list to private so it can't be altered with.
        private List<Truck> rentedTrucks; // I have set this list to private so it can't be altered with.
        private List<Motorbike> rentedMotorbikes; // I have set this list to private so it can't be altered with.

        public Customer()
        {
            this.rentLimit = 3;
            this.rentedCars = new List<Car>(rentLimit);
            this.rentedTrucks = new List<Truck>(rentLimit);
            this.rentedMotorbikes = new List<Motorbike>(rentLimit);
        }

        public Customer(string fname, string lname)
        {
            this.accessCode = GenerateAccessCode();
            this.firstName = fname;
            this.lastName = lname;
            this.rentLimit = 3;
            this.vehicleCount = 0;
            this.rentedCars = new List<Car>(rentLimit);
            this.rentedTrucks = new List<Truck>(rentLimit);
            this.rentedMotorbikes = new List<Motorbike>(rentLimit);
            // List made with capacity to make better performance, also increases the scope of the application
            // as if the rent limit were to increase there could be performance issues.
        }

        public override string GetType() { return "Customer"; }
        public List<Vehicle> GetRentedVehicles() 
        { 
            List<Vehicle> rentedVehicles = new();
            foreach (var car in rentedCars) { rentedVehicles.Add(car); }
            foreach (var truck in rentedTrucks) { rentedVehicles.Add(truck); }
            foreach (var motorbike in rentedMotorbikes) { rentedVehicles.Add(motorbike); }
            return rentedVehicles; 
        }
        public void AddRentedVehicle(Car car) { rentedCars.Add(car); }
        public void AddRentedVehicle(Truck truck) { rentedTrucks.Add(truck); }
        public void AddRentedVehicle(Motorbike motorbike) { rentedMotorbikes.Add(motorbike); }
    }
}
