using SmartGuard.Core.Azure.Authenticate;
using System;
using System.Threading.Tasks;

namespace SmartGuard.Core.Azure.Tables
{
    public static partial class Queries
    {
        public static async Task<bool> TryInsertEntityAsync(string tableName, dynamic entity)
        {
            string uri = @"https://" + Utilities.azureAccount + ".table.core.windows.net/" + tableName;
            return await Utilities.TryInsertEntityAsync(uri, tableName, entity);
        }

        public static async Task<bool> TryUpdateEntityAsync(string tableName, dynamic entity, string partitionKey, string rowKey)
        {
            string uri = @"https://{0}.table.core.windows.net/{1}(PartitionKey='{2}',RowKey='{3}')";
            uri = String.Format(uri, Utilities.azureAccount, tableName, partitionKey, rowKey);
            return await Utilities.TryUpdateEntityAsync(uri, tableName, entity);
        }
    }
}