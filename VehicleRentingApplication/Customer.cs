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
        private string accessCode;
        // Validation was implemented when generating the access code, however I've added it in the setter here incase any other code
        // conflicts with that validation to ensure that there can definately be no issues.
        // While there is validation provide within the program, this also protects against vehicles added manually within the json file.
        public string AccessCode 
        {
            get
            {
                return accessCode;
            }
            set
            {
                if (value.Length > 3)
                {
                    this.accessCode = value.Substring(0, 3);
                }
                else
                {
                    this.accessCode = value;
                }
            }
        }

        private int RentLimit;

        // Validation isn't needed here as this int is calculated based off the amount of cars the customer has rented.
        // There is also validation set within the program to ensure that the vehicle count count can't be higher than the rent limit.

        // I don't want this stored in Json as it is calculated during runtime, based off of the amount of rented vehicle objects
        // that contain the rentedBy {customer accessCode}, it needs JsonIgnore as it is a public property but because it is calculated
        // during runtime, the value would be stored as 0 anyway so there is no need to store this value.
        [JsonIgnore]
        public int VehicleCount { get; private set; }

        //[JsonConstructor]
        public Customer() : base()
        {
            this.RentLimit = 3;
            this.VehicleCount = 0;
        }

        public Customer(string fname, string lname)
        {
            this.AccessCode = GenerateAccessCode();
            this.FirstName = fname;
            this.LastName = lname;
            this.RentLimit = 3;
            this.VehicleCount = 0;
        }

        public override string GetType() { return "Customer"; }

        public void RentCar(Car car, string carID, RentedVehicles rentedVehicles)
        {
            if (VehicleCount < RentLimit)
            {
                car.RentedBy = this.AccessCode;
                rentedVehicles.RentedCars.Add(car);
                VehicleCount++;

                Console.WriteLine($"Vehicle {carID} rented successfully.\n\n");
            }
            else { Console.WriteLine($"You have reached the rent limit of {RentLimit}. Cannot rent more vehicles.\n\n"); }
        }

        public void RentTruck(Truck truck, string truckID, RentedVehicles rentedVehicles)
        {
            // Check if the user has reached the rent limit
            if (VehicleCount < RentLimit)
            {
                truck.RentedBy = this.AccessCode;
                rentedVehicles.RentedTrucks.Add(truck);
                VehicleCount++;
                Console.WriteLine($"Vehicle {truckID} rented successfully.\n\n");
            }
            else { Console.WriteLine($"You have reached the rent limit of {RentLimit}. Cannot rent more vehicles.\n\n"); }
        }

        public void RentMotorbike(Motorbike motorbike, string motorbikeID, RentedVehicles rentedVehicles)
        {
            // Check if the user has reached the rent limit
            if (VehicleCount < RentLimit)
            {
                motorbike.RentedBy = this.AccessCode;
                rentedVehicles.RentedMotorbikes.Add(motorbike);
                VehicleCount++;
                Console.WriteLine($"\nVehicle {motorbikeID} rented successfully.\n\n");
            }
            else { Console.WriteLine($"You have reached the rent limit of {RentLimit}. Cannot rent more vehicles.\n\n"); }
        }

        public void ReturnCar(string regPlate, RentedVehicles rentedVehicles)
        {
            Car carToRemove = rentedVehicles.RentedCars.Find(car => car.Reg.Reg == regPlate);

            // Check if the car is found
            if (carToRemove != null)
            {
                rentedVehicles.RentedCars.Remove(carToRemove);
                VehicleCount--;

                Console.WriteLine($"Car with registration plate {regPlate} has been returned.");
            }
            else { Console.WriteLine($"Car with registration plate {regPlate} not found in the rented vehicles list."); }
        }

        public void ReturnTruck(string regPlate, RentedVehicles rentedVehicles)
        {
            Truck truckToRemove = rentedVehicles.RentedTrucks.Find(truck => truck.Reg.Reg == regPlate);

            // Check if the car is found
            if (truckToRemove != null)
            {
                rentedVehicles.RentedTrucks.Remove(truckToRemove);
                VehicleCount--;

                Console.WriteLine($"Car with registration plate {regPlate} has been returned.");
            }
            else { Console.WriteLine($"Car with registration plate {regPlate} not found in the rented vehicles list."); }
        }

        public void ReturnMotorbike(string regPlate, RentedVehicles rentedVehicles)
        {
            Motorbike motorbikeToRemove = rentedVehicles.RentedMotorbikes.Find(motorbike => motorbike.Reg.Reg == regPlate);

            // Check if the car is found
            if (motorbikeToRemove != null)
            {
                rentedVehicles.RentedMotorbikes.Remove(motorbikeToRemove);
                VehicleCount--;

                Console.WriteLine($"Car with registration plate {regPlate} has been returned.");
            }
            else { Console.WriteLine($"Car with registration plate {regPlate} not found in the rented vehicles list."); }
        }

        public int GetRentLimit() { return RentLimit; }
    }
}
