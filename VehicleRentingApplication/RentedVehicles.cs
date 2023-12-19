using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VehicleRentingApplication
{
    internal class RentedVehicles
    {
        public Dictionary<string, Car> rentedCars { get; set; }
        public Dictionary<string, Truck> rentedTrucks { get; set; }
        public Dictionary<string, Motorbike> rentedMotorbikes { get; set; }

        public RentedVehicles()
        {
            rentedCars = new Dictionary<string, Car>();
            rentedTrucks = new Dictionary<string, Truck>();
            rentedMotorbikes = new Dictionary<string, Motorbike>();
        }

        // [TODO] Probably should private the dictionaries and add methods for adding to the dictionaries (BECAUSE NEED TO FIGURE OUT ID issue.)

        //public void AddVehicle(string accessCode, Vehicle v)
        //{
        //    rentedVehiclesList.Add(accessCode, v);
        //}
    }
}
