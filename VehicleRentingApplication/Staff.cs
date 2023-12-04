using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VehicleRentingApplication
{
    internal class Staff : Account
    {
        private double totalSales;

        public Staff(string fname, string lname, double totalSales)
        {
            this.firstName = fname;
            this.lastName = lname;
            this.totalSales = totalSales; // This should go up when a car has been rented.
        }

        public override string GetType() { return "Staff"; }
    }
}
