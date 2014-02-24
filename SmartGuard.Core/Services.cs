using SmartGuard.Core.Azure.Tables;
using SmartGuard.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartGuard.Core
{
    public class Services
    {
        public static async Task<bool> TryUploadPosition(Position position)
        {
            return await Queries.TryInsertEntityAsync("Position", position);
        }
    }
}
