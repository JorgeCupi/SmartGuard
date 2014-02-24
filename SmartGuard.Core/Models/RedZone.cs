using SmartGuard.Core.Facebook.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartGuard.Core.Models
{
    public class RedZone
    {
        public string PartitionKey { get; set; }
        public string RowKey { get; set; }
        public string Timestamp { get; set; }
        public string FacebookID { get; set; }
        public string City { get; set; }
        public string Address{ get; set; }
        public string Country { get; set; }
        public string Description{ get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public DateTime RegisteredAt { get; set; }
        public int Radius { get; set; }

        public FacebookUser FbUser{ get; set; }
    }
}
