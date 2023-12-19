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

        public RentedVehicles()
        {
            rentedCars = new List<Car>();
            rentedTrucks = new List<Truck>();
            rentedMotorbikes = new List<Motorbike>();
        }

        // [TODO] Probably should private the dictionaries and add methods for adding to the dictionaries (BECAUSE NEED TO FIGURE OUT ID issue.)

        //public void AddVehicle(string accessCode, Vehicle v)
        //{
        //    rentedVehiclesList.Add(accessCode, v);
        //}
    }
}
