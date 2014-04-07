using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartGuard.Core.Azure.Authenticate;
using System.Net;
using System.Net.Http;
using SmartGuard.Core.Models;
namespace SmartGuard.Core.Azure.Tables
{
    public static partial class Queries
    {
        public static async Task<List<Position>> GetAllEntitiesFromThisTableAsync(string tableName)
        {
            string result = await Utilities.GetEntitiesAsync(tableName, "","", -1);
            return Utilities.TransformToPositionList(result);
        }

        public static async Task<List<Position>> GetPositionsForThisCode(string partitionKey,int maxAmount)
        {
            string result = await Utilities.GetEntitiesAsync("Position","PartitionKey",partitionKey, maxAmount);
            return Utilities.TransformToPositionList(result);
        }

        public static async Task<List<Permission>> GetPermissionsForThisCode(string keyName, string keyValue)
        {
            string result = await Utilities.GetEntitiesAsync("AllowedOne", keyName,keyValue, 0);
            if (result != null)
                return Utilities.TransformToPermissionList(result);
            else return null;
        }

        public static async Task<List<Permission>> GetFavoritePermissionsForThisCode(string keyName, string keyValue)
        {
            string result = await Utilities.GetEntitiesAsync("AllowedOne", keyName, keyValue,"IsFavorite","true", 0);
            if (result != null)
                return Utilities.TransformToPermissionList(result);
            else return null;
        }
        public static async Task<List<RedZone>> GetRedZonesForThisCode(string partitionKey)
        {
            string result = await Utilities.GetEntitiesAsync("RedZone","PartitionKey",partitionKey, 0);
            return Utilities.TransformToRedZoneList(result);
        }
    }
}
