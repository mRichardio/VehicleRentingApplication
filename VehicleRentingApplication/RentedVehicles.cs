using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VehicleRentingApplication
{
    internal class RentedVehicles
    {
        public List<Car> RentedCars { get; set; }
        public List<Truck> RentedTrucks { get; set; }
        public List<Motorbike> RentedMotorbikes { get; set; }
        public List<Vehicle> CombinedVehicles { get; set; }


        public RentedVehicles()
        {
            RentedCars = new List<Car>();
            RentedTrucks = new List<Truck>();
            RentedMotorbikes = new List<Motorbike>();
            CombinedVehicles = new List<Vehicle>();
        }

        public int GetVehicleCount(Customer customer)
        {
            int count = 0;

            count += RentedCars.Count(c => c.RentedBy == customer.AccessCode);
            count += RentedTrucks.Count(c => c.RentedBy == customer.AccessCode);
            count += RentedMotorbikes.Count(c => c.RentedBy == customer.AccessCode);

            return count;
        }
    }
}
