using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VehicleRentingApplication
{
    internal class Staff : Account
    {
        // Here to check if a staff member used to be a customer or not.
        // Wont need validation as it is automatically set to true or false when a new staff member is created.
        private bool PastCustomer; 
        // Validation was implemented when generating the access code, however I've added it in the setter here incase any other code
        // conflicts with that validation to ensure that there can definately be no issues.
        private string accessCode;
        public string AccessCode 
        {
            get
            {
                return accessCode;
            }
            set
            {
                if (value.Length > 3)
                {
                    this.accessCode = value.Substring(0, 3);
                }
                else
                {
                    this.accessCode = value;
                }
            }
        }

        public Staff() 
        {

        }

        public Staff(string fname, string lname)
        {
            this.FirstName = fname;
            this.LastName = lname;
            PastCustomer = false;
            this.AccessCode = GenerateAccessCode();
        }

        // This copy constructor is being used to turn customers into staff members.
        // I decided to do this because if an existing customer was hired, then they could be easily converted.
        public Staff(Customer customer)
        {
            this.FirstName = customer.FirstName; 
            this.LastName = customer.LastName;
            this.AccessCode = GenerateAccessCode();
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
            Staff staff = new Staff(fName, lName);
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
