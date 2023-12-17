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
        public string accessCode { get; set; }
        public string firstName { get; protected set; }
        public string lastName { get; protected set; }

        public virtual string GetType() { return "User"; } // Change value in respective class

        public string GenerateAccessCode() { return ""; } // This function will generate an access code for each account

        public void DisplayDetails() { }
    }
}
