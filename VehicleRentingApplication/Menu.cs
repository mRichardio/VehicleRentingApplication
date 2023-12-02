﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VehicleRentingApplication
{
    internal class Menu
    {
        private string[] options;

        public Menu(string[] options)
        {
            this.options = options;
        }


        public void DisplayMenu()
        {
            Console.WriteLine("Menu:");
            for (int i = 0; i < options.Length; i++)
            {
                Console.WriteLine($"{i + 1}. {options[i]}");
            }
        }
    }
}