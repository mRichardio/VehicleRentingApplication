using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VehicleRentingApplication
{
    internal class Menu
    {
        // The reason I have decided to implement my menu system in this way is because it makes it alot easier to generate
        // new menus and results in a lot less duplicate code. Furthermore, it allows me to easilly use a switch case with this
        // solution making the code overall a lot easier to understand/read.

        // I basically wanted to make better use of OOP to reduce the amount of code needed and I think this is a much more elegant
        // way of doing things.

        // Used an array here as there was no need for a list, the capacity is pre-set and wont be searching through it.
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