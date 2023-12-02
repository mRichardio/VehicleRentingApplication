using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VehicleRentingApplication
{
    abstract internal class Users
    {
        public string accessCode { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }

        public virtual string GetType() { return "User"; } // Change value in respective class
    }
}
