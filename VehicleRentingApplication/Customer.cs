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

        [JsonConstructor]
        public Customer() : base()
        {
            this.rentLimit = 3;
            this.vehicleCount = 0;
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
        public void IncreaseVehicleCount(int amount) { this.vehicleCount += amount; }

        //void RentCar(Car car, string carID)
        //{
        //    if (currentUser.vehicleCount < currentUser.rentLimit)
        //    {
        //        car.rentedBy = currentUser.accessCode;
        //        rentedVehicles.rentedCars.Add(rentedVehicles.rentedCars.Count + 1.ToString(), car);
        //        currentUser.IncreaseVehicleCount(1);
        //        // Will need to remove the vehicles from their vehicle list. IE cars.
        //        Console.WriteLine($"Vehicle {carID} rented successfully.\n\n");
        //    }
        //    else
        //    {
        //        Console.WriteLine($"You have reached the rent limit of {currentUser.rentLimit}. Cannot rent more vehicles.\n\n");
        //    }
        //}

        public void RentCar(Car car, string carID, RentedVehicles rentedVehicles)
        {
            if (vehicleCount < rentLimit)
            {

                car.rentedBy = accessCode;
                rentedVehicles.rentedCars.Add(rentedVehicles.rentedCars.Count + 1.ToString(), car);
                IncreaseVehicleCount(1);

                Console.WriteLine($"Vehicle {carID} rented successfully.\n\n");
            }
            else { Console.WriteLine($"You have reached the rent limit of {rentLimit}. Cannot rent more vehicles.\n\n"); }
        }

        public void RentTruck(Truck truck, string truckID, RentedVehicles rentedVehicles)
        {
            // Check if the user has reached the rent limit
            if (vehicleCount < rentLimit)
            {
                truck.rentedBy = accessCode;
                rentedVehicles.rentedTrucks.Add(rentedVehicles.rentedTrucks.Count + 1.ToString(), truck);
                IncreaseVehicleCount(1);
                //trucks.Remove(truckID); // Remove the vehicle from the dictionary
                Console.WriteLine($"Vehicle {truckID} rented successfully.\n\n");
            }
            else { Console.WriteLine($"You have reached the rent limit of {rentLimit}. Cannot rent more vehicles.\n\n"); }
        }

        public void RentMotorbike(Motorbike motorbike, string motorbikeID, RentedVehicles rentedVehicles)
        {
            // Check if the user has reached the rent limit
            if (vehicleCount < rentLimit)
            {
                motorbike.rentedBy = accessCode;
                rentedVehicles.rentedMotorbikes.Add(rentedVehicles.rentedMotorbikes.Count + 1.ToString(), motorbike);
                IncreaseVehicleCount(1);
                //motorbikes.Remove(motorbikeID); // Remove the vehicle from the dictionary
                Console.WriteLine($"\nVehicle {motorbikeID} rented successfully.\n\n");
            }
            else { Console.WriteLine($"You have reached the rent limit of {rentLimit}. Cannot rent more vehicles.\n\n"); }
        }
    }
}
