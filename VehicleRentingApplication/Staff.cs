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

        public Staff() { }

        public Staff(string fname, string lname, string accessCode)
        {
            this.firstName = fname;
            this.lastName = lname;
            this.totalSales = 0;
        }

        // This copy constructor is being used to turn customers into staff members.
        // I decided to do this because if an existing customer was hired, then they could be easily converted.
        public Staff(Customer customer)
        {
            this.firstName = customer.firstName; 
            this.lastName = customer.lastName;
            this.accessCode = GenerateAccessCode();
            this.totalSales = 0;
        }

        public override string GetType() { return "Staff"; }

        public void RegisterStaff(HashSet<Staff> staffList)
        {
            Console.WriteLine("---| Create Staff |---\nFirst Name:");
            string fName = Console.ReadLine();
            Console.WriteLine("Last Name: ");
            string lName = Console.ReadLine();
            string accessCode = GenerateAccessCode();
            Staff staff = new Staff(fName, lName, accessCode);
            staffList.Add(staff);
            Console.WriteLine("Successfully created new staff member.");
            Console.WriteLine("Press ENTER to continue..."); Console.ReadLine();
        }
    }
}
