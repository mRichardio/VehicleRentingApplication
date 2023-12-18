﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace VehicleRentingApplication
{
    internal class RentedVehicleHandler
    {
        private List<Vehicle> vehicles;

        public RentedVehicleHandler()
        {
            vehicles = new List<Vehicle>(3);
        }

        public void WriteToJSON(string fileName)
        {
            string jsonContent = JsonSerializer.Serialize(vehicles, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(fileName, jsonContent);
        }

        public void ReadFromJSON(string fileName)
        {
            if (File.Exists(fileName))
            {
                string jsonContent = File.ReadAllText(fileName);
                vehicles = JsonSerializer.Deserialize<List<Vehicle>>(jsonContent);
            }
            else
            {
                Console.WriteLine("File not found: " + fileName);
            }
        }

        public void AddVehicle(Vehicle vehicle)
        {
            if (vehicles.Count < 3)
            {
                vehicles.Add(vehicle);
                Console.WriteLine("Vehicle added successfully.");
            }
            else
            {
                Console.WriteLine("Cannot add more vehicles. Maximum capacity reached.");
            }
        }

        public void RemoveVehicle(string registrationNumber)
        {
            Vehicle vehicleToRemove = vehicles.FirstOrDefault(v => v.reg.reg == registrationNumber);

            if (vehicleToRemove != null)
            {
                vehicles.Remove(vehicleToRemove);
                Console.WriteLine("Vehicle removed successfully.");
            }
            else
            {
                Console.WriteLine($"Vehicle with registration number {registrationNumber} not found.");
            }
        }
    }
}