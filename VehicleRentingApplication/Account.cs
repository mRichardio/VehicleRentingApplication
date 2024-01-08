using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace VehicleRentingApplication
{
    internal abstract class Account : IUser, IAccessCode
    {
        // This is protected so no other classes can access it other than customer and staff, the deriving classes
        private string accessCode;
        // While there is validation provide within the program, this also protects against vehicles added manually within the json file.
        protected string AccessCode 
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
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public Account()
        {

        }

        public virtual string GetType() { return "User"; } // Change value in respective class

        public string GenerateAccessCode()
        {
            Random random = new Random();
            // This is a const as it wont change.
            const string characters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            StringBuilder codeBuilder = new StringBuilder();

            for (int i = 0; i < 3; i++)
            {
                int index = random.Next(characters.Length);
                codeBuilder.Append(characters[index]);
            }

            return codeBuilder.ToString();
        }

        public string GetAccessCode() {  return AccessCode; }

        public void DisplayDetails()
        {
            Console.WriteLine($"Account Type: {GetType()}\nFirstname: {FirstName}\nLastname: {LastName}\n\nYour Code: {AccessCode}");
        }
    }
}
