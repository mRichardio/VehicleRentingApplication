using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VehicleRentingApplication
{
    internal class Registration
    {
        public string reg { get; set; }

        public Registration() { /*Default*/ }

        public Registration(string reg)
        {
            this.reg = reg;
        }

        public Registration CreateReg()
        {
            while (true)
            {
                Console.WriteLine("Enter vehicles registration: ");
                string input = Console.ReadLine().Trim();
                if (input.Length != 7) { Console.WriteLine("[Error]: Invalid reg length (reg have a length of 7!)"); }
                else { return new Registration(input); }
            }
        }
    }
}
