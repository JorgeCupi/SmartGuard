using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
namespace SmartGuard.Core.Models
{
    public class Position
    {
        public string PartitionKey { get; set; }
        public string RowKey { get; set; }
        public string Timestamp { get; set; }
        public string Address { get; set; }
        public string Country { get; set; }
        public string City { get; set; }
        public string FacebookID { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public int Counter{ get; set; }
        public DateTime RegisteredAt { get; set; }
        public string LatLon { get; set; }
    }
}