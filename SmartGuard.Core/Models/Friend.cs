using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SmartGuard.Core.Models
{
    public class Friend
    {
        public Position LastPosition { get; set; }
        public string FacebookID { get; set; }
        public string Name { get; set; }
        public string Picture { get; set; }
        public override string ToString()
        {
            return String.Format("{0} {1}", Name, FacebookID);
        }

        public bool IsAllowed { get; set; }
        public string FBIDFromViewer { get; set; }
        public string FBIDFromViewed { get; set; }
    }
}
