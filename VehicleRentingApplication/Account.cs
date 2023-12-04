﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace VehicleRentingApplication
{
    internal abstract class Account : IUser
    {
        public string accID { get; protected set; }
        public string accessCode { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }

        public virtual string GetType() { return "User"; } // Change value in respective class

        private readonly Random random = new Random();
        public string GenerateAccessCode()
        {
            // This is a const as it wont change.
            const string characters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            StringBuilder codeBuilder = new StringBuilder();

            for (int i = 0; i < 5; i++)
            {
                int index = random.Next(characters.Length);
                codeBuilder.Append(characters[index]);
            }

            return codeBuilder.ToString();
        }

        public void DisplayDetails()
        {
            Console.WriteLine($"Account Type: {GetType()}\nFirstname: {firstName}\nLastname: {lastName}\n\nYour Code: {accessCode}");
        }
    }
}