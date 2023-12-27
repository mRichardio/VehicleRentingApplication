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
        public string firstName { get; protected set; }
        public string lastName { get; protected set; }

        public virtual string GetType() { return "User"; } // Change value in respective class

        public void DisplayDetails() { }
    }
}
