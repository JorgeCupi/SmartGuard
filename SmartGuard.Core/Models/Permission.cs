using System;
using System.Collections.Generic;
using System.Linq;

namespace SmartGuard.Core.Models
{
    public class Permission
    {
        public string PartitionKey { get; set; }
        public string RowKey { get; set; }
        public bool IsAllowed { get; set; }
        public string FBIDFromViewer { get; set; }
        public string FBIDFromViewed { get; set; }
        public bool IsFavorite { get; set; }
    }
}