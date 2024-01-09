using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VehicleRentingApplication
{
    internal class RentedVehicles
    {
        // The reason why I decided to use a list for storing rented vehicles is because I know there will be very minimal amount of lookups
        // being used on these lists as there can only be 3 rented cars per user and one user being logged in at once. So instead of searching for
        // a vehicles reg, the searchs are looking for the users access code, which can't be a duplicated key in the dictionary as this wouldn't work.
        // So, because of this, I figured that a list would be the best solution.
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

        // Not the most elegant way of doing this but I needed to update the vehicle count of each user during runtime, to ensure that the vehicle
        // count was accurate. I tried updating it as vehicles were stored, but this resulted in issues.
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
