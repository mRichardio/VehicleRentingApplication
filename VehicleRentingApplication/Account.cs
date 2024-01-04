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
        protected string accessCode { get; set; } // This is protected so no other classes can access it other than customer and staff, the deriving classes
        public string firstName { get; set; }
        public string lastName { get; set; }

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

        public string GetAccessCode() {  return accessCode; }

        public void DisplayDetails()
        {
            Console.WriteLine($"Account Type: {GetType()}\nFirstname: {firstName}\nLastname: {lastName}\n\nYour Code: {accessCode}");
        }
    }
}
