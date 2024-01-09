using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace VehicleRentingApplication
{
    internal class Registration
    {
        // Validation for this variable is handled in actual program however, I have included validation to make sure that
        // the variable can not be longer than 7
        private string reg;
        public string Reg
        {
            get
            {
                return reg;
            }
            set
            {
                if (value.Length > 7)
                {
                    this.reg = value.Substring(0, 7);
                }
                else
                {
                    this.reg = value;
                }
            }
        }

        public Registration() 
        {

        }

        public Registration(string reg)
        {
            this.Reg = reg;
        }

        public Registration CreateReg()
        {
            while (true)
            {
                Console.WriteLine("Enter vehicles registration: ");
                string input = Console.ReadLine().Trim().ToUpper();
                if (input.Length != 7) { Console.WriteLine("[Error]: Invalid reg length (reg must have a length of 7!)"); }
                else { return new Registration(input); }
            }
        }
    }
}
