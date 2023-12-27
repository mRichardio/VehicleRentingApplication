using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VehicleRentingApplication
{
    internal class RentedVehicles
    {
        public List<Car> rentedCars { get; set; }
        public List<Truck> rentedTrucks { get; set; }
        public List<Motorbike> rentedMotorbikes { get; set; }
        public List<Vehicle> combinedVehicles { get; set; }


        public RentedVehicles()
        {
            rentedCars = new List<Car>();
            rentedTrucks = new List<Truck>();
            rentedMotorbikes = new List<Motorbike>();
            combinedVehicles = new List<Vehicle>();
        }

        public int GetVehicleCount(Customer customer)
        {
            int count = 0;

            count += rentedCars.Count(c => c.rentedBy == customer.accessCode);
            count += rentedTrucks.Count(c => c.rentedBy == customer.accessCode);
            count += rentedMotorbikes.Count(c => c.rentedBy == customer.accessCode);

            return count;
        }
    }
}
