using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VehicleRentingApplication
{
    internal class Staff : Account
    {
        private bool PastCustomer; // Here to check if a staff member used to be a customer or not.

        public Staff() { }

        public Staff(string fname, string lname, string accessCode)
        {
            this.firstName = fname;
            this.lastName = lname;
            PastCustomer = false;
        }

        // This copy constructor is being used to turn customers into staff members.
        // I decided to do this because if an existing customer was hired, then they could be easily converted.
        public Staff(Customer customer)
        {
            this.firstName = customer.firstName; 
            this.lastName = customer.lastName;
            this.accessCode = GenerateAccessCode();
            PastCustomer = true; // Set this in here because this is the copy constructor used to turn a customer into staff.
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

        public bool PastCustomerCheck()
        {
            return PastCustomer;
        }
    }
}
