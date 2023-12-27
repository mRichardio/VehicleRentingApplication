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
        public string accessCode { get; set; }
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

        public void RentCar(Car car, string carID, RentedVehicles rentedVehicles)
        {
            if (vehicleCount < rentLimit)
            {
                car.rentedBy = this.accessCode;
                rentedVehicles.rentedCars.Add(car);
                vehicleCount++;

                Console.WriteLine($"Vehicle {carID} rented successfully.\n\n");
            }
            else { Console.WriteLine($"You have reached the rent limit of {rentLimit}. Cannot rent more vehicles.\n\n"); }
        }

        public void RentTruck(Truck truck, string truckID, RentedVehicles rentedVehicles)
        {
            // Check if the user has reached the rent limit
            if (vehicleCount < rentLimit)
            {
                truck.rentedBy = this.accessCode;
                rentedVehicles.rentedTrucks.Add(truck);
                vehicleCount++;
                Console.WriteLine($"Vehicle {truckID} rented successfully.\n\n");
            }
            else { Console.WriteLine($"You have reached the rent limit of {rentLimit}. Cannot rent more vehicles.\n\n"); }
        }

        public void RentMotorbike(Motorbike motorbike, string motorbikeID, RentedVehicles rentedVehicles)
        {
            // Check if the user has reached the rent limit
            if (vehicleCount < rentLimit)
            {
                motorbike.rentedBy = this.accessCode;
                rentedVehicles.rentedMotorbikes.Add(motorbike);
                vehicleCount++;
                Console.WriteLine($"\nVehicle {motorbikeID} rented successfully.\n\n");
            }
            else { Console.WriteLine($"You have reached the rent limit of {rentLimit}. Cannot rent more vehicles.\n\n"); }
        }

        public void ReturnCar(string regPlate, RentedVehicles rentedVehicles)
        {
            Car carToRemove = rentedVehicles.rentedCars.Find(car => car.reg.reg == regPlate);

            // Check if the car is found
            if (carToRemove != null)
            {
                rentedVehicles.rentedCars.Remove(carToRemove);
                vehicleCount--;

                Console.WriteLine($"Car with registration plate {regPlate} has been returned.");
            }
            else { Console.WriteLine($"Car with registration plate {regPlate} not found in the rented vehicles list."); }
        }

        public void ReturnTruck(string regPlate, RentedVehicles rentedVehicles)
        {
            Truck truckToRemove = rentedVehicles.rentedTrucks.Find(truck => truck.reg.reg == regPlate);

            // Check if the car is found
            if (truckToRemove != null)
            {
                rentedVehicles.rentedTrucks.Remove(truckToRemove);
                vehicleCount--;

                Console.WriteLine($"Car with registration plate {regPlate} has been returned.");
            }
            else { Console.WriteLine($"Car with registration plate {regPlate} not found in the rented vehicles list."); }
        }

        public void ReturnMotorbike(string regPlate, RentedVehicles rentedVehicles)
        {
            Motorbike motorbikeToRemove = rentedVehicles.rentedMotorbikes.Find(motorbike => motorbike.reg.reg == regPlate);

            // Check if the car is found
            if (motorbikeToRemove != null)
            {
                rentedVehicles.rentedMotorbikes.Remove(motorbikeToRemove);
                vehicleCount--;

                Console.WriteLine($"Car with registration plate {regPlate} has been returned.");
            }
            else { Console.WriteLine($"Car with registration plate {regPlate} not found in the rented vehicles list."); }
        }
    }
}
