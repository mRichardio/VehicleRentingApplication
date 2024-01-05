using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace VehicleRentingApplication
{
    internal interface IUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public virtual string GetType() { return "User"; } // Change value in respective class

        public void DisplayDetails() { }
    }
}
