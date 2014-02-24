using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
using System.Globalization;
namespace SmartGuard.Core.Azure.Authenticate
{
    public static class Authentication
    {
        public static string GetSignedString(string time, 
            string tableName, string account, 
            string key, string HTTPMethod, 
            string partitionKey, string rowKey)
        {
            String stringToSign = string.Empty;
            string type;
            if (HTTPMethod != "PUT" )
            {
                stringToSign = String.Format("{0}\n/{1}/{2}()",
                    time,
                    account,
                    tableName);
                type = "SharedKeyLite";
            }

            else
            {
                String contentMD5 = String.Empty;
                String contentType = "application/atom+xml";
                String canonicalizedResource = String.Format("/{0}/{1}(PartitionKey='{2}',RowKey='{3}')",
                    account,
                    tableName,
                    partitionKey
                    , rowKey);

                stringToSign = String.Format(
                        "{0}\n{1}\n{2}\n{3}\n{4}",
                        HTTPMethod,
                        contentMD5,
                        contentType,
                        time,
                        canonicalizedResource);
                type = "SharedKey";
            }
           

            string signedKey = SignThis(stringToSign, key, account,type);
            return signedKey;
        }

        private static String SignThis(String canonicalizedString, string Key, string Account,string type)
        {
            String signature = string.Empty;
            byte[] unicodeKey = Convert.FromBase64String(Key);
            HMACSHA256 hmacSha256 = new HMACSHA256(unicodeKey);
                Byte[] dataToHmac = System.Text.Encoding.UTF8.GetBytes(canonicalizedString);
                signature = Convert.ToBase64String(hmacSha256.ComputeHash(dataToHmac));

            String authorizationHeader = String.Format(
                  CultureInfo.InvariantCulture,
                  "{2} {0}:{1}",
                  Account,
                  signature,
                  type);

            return authorizationHeader;
        }
    }
}
