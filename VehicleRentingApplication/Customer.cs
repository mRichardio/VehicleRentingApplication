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

        public void RentCar(Car car, string carID, RentedVehicles rentedVehicles)
        {
            if (vehicleCount < rentLimit)
            {

                car.rentedBy = accessCode;
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
                truck.rentedBy = accessCode;
                rentedVehicles.rentedTrucks.Add(truck);
                vehicleCount++;
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
                rentedVehicles.rentedMotorbikes.Add(motorbike);
                vehicleCount++;
                Console.WriteLine($"\nVehicle {motorbikeID} rented successfully.\n\n");
            }
            else { Console.WriteLine($"You have reached the rent limit of {rentLimit}. Cannot rent more vehicles.\n\n"); }
        }

        public void ReturnCar(string regPlate, RentedVehicles rentedVehicles)
        {
            //if (vehicleCount > 0)
            //{
                Car carToRemove = rentedVehicles.rentedCars.Find(car => car.reg.reg == regPlate);

                // Check if the car is found
                if (carToRemove != null)
                {
                    rentedVehicles.rentedCars.Remove(carToRemove);
                    vehicleCount--;

                    Console.WriteLine($"Car with registration plate {regPlate} has been returned.");
                }
                else { Console.WriteLine($"Car with registration plate {regPlate} not found in the rented vehicles list."); }
            //}
        }
    }
}
