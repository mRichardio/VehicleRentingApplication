﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace VehicleRentingApplication
{
    internal class DeserialisatonVehicle : Vehicle
    {
        [JsonConstructor]
        public DeserialisatonVehicle()
        {

        }
    }
}
