﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VehicleRentingApplication
{
    internal class Truck : Vehicle
    {
        private int doorCount;
        private int wheelCount;
        private int storageCapacity;

        public Truck(string type, int modelYear, int doorCount, bool isAutomatic, int wheelCount, int storageCapacity, Colour paint, Registration reg)
        {
            this.type = type;
            this.modelYear = modelYear;
            this.doorCount = doorCount;
            this.isAutomatic = isAutomatic;
            this.wheelCount = wheelCount;
            this.storageCapacity = storageCapacity;
            this.paint = paint;
            this.reg = reg;
        }
    }
}