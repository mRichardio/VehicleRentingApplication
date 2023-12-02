using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace VehicleRentingApplication
{
    internal abstract class Vehicle
    {
        //[JsonPropertyName("type")]
        public virtual string type { get; set; } // Make it virtual so it can be overridden in derived classes
        public int modelYear { get; set; }
        public string manufacturer { get; set; }
        public string model { get; set; }
        public int engineSize { get; set; }
        public bool isAutomatic { get; set; }
        public Colour paint { get; set; }
        public Registration reg { get; set; }
    }
}
