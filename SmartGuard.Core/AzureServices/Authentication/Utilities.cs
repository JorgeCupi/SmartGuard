using SmartGuard.Core.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace SmartGuard.Core.Azure.Authenticate
{
    public static class Utilities
    {
        public static string azureKey { get; set; }
        public static string azureAccount { get; set; }
        private static readonly XNamespace d = "http://schemas.microsoft.com/ado/2007/08/dataservices";
        private static readonly XNamespace m = "http://schemas.microsoft.com/ado/2007/08/dataservices/metadata";


        public static async Task<string> GetEntitiesAsync(string tableName, string keyName, string keyValue, int maxValue)
        {
            string uri = string.Empty;
            if(maxValue>=0)
            {
                string queryString = String.Format("{0}()?$filter={1}%20eq%20'{2}'", tableName, keyName, keyValue);
                if(maxValue>0)
                    queryString = String.Format("{0}&$top={1}",queryString,maxValue);
                uri = String.Format(@"https://{0}.table.core.windows.net/{1}",Utilities.azureAccount, queryString);
            }
            else uri = String.Format(@"https://{0}.table.core.windows.net/{1}",Utilities.azureAccount, tableName);
            return await Utilities.DownloadString(uri, tableName);
        }

        public static async Task<string> GetEntitiesAsync(string tableName, string keyName, string keyValue, string KeyNameTwo, string KeyValueTwo, int maxValue)
        {
            string uri = string.Empty;
            if (maxValue >= 0)
            { 
                string queryString = String.Format("{0}()?$filter={1}%20eq%20'{2}'%20and%20{3}%20eq%20{4}",tableName,keyName,keyValue,KeyNameTwo,KeyValueTwo);
                if (maxValue > 0)
                    queryString = String.Format("{0}&$top={1}", queryString, maxValue);
                uri = String.Format(@"https://{0}.table.core.windows.net/{1}", Utilities.azureAccount, queryString);
            }
            else uri = String.Format(@"https://{0}.table.core.windows.net/{1}",Utilities.azureAccount,tableName);
            return await Utilities.DownloadString(uri, tableName);
        }
        
        public static async Task<bool> TryInsertEntityAsync(string uri, string tableName, dynamic entity)
        {
            string body = buildBodyForInsertOperation(entity);
            HttpClient request = GetHttpClient(tableName, "POST", "", "");
            StringContent stringContent = GetStringContent(body);

            HttpResponseMessage messageResult = await request.PostAsync(uri+"()", stringContent);
            string result = messageResult.StatusCode.ToString();

            if (result == "Created")
                return true;
            else return false;
        }
        
        public static async Task<bool> TryUpdateEntityAsync(string uri, string tableName, dynamic entity)
        {
            string body = buildBodyForUpdateOperation(entity, tableName);
            string PT = entity.GetType().GetProperty("PartitionKey").GetValue(entity, null).ToString();
            string RK = entity.GetType().GetProperty("RowKey").GetValue(entity, null).ToString();
            HttpClient request = GetHttpClient(tableName,"PUT",PT,RK);

            request.DefaultRequestHeaders.Add("If-Match","*");
            StringContent stringContent = GetStringContent(body);

            HttpResponseMessage messageResult = await request.PutAsync(uri, stringContent);
            string result = messageResult.StatusCode.ToString();    
            if (result == "NoContent")
                return true;
            else return false;
        }

        public static List<Position> TransformToPositionList(string message)
        {
            XDocument document = XDocument.Parse(message);
            IEnumerable<Position> query = from N in document.Descendants(m + "properties")
                        select new Position
                        {
                            Address = (string)N.Element(d + "Address"),
                            FacebookID = (string)N.Element(d + "FacebookID"),
                            Latitude = (double)(N.Element(d + "Latitude")),
                            Longitude = (double)(N.Element(d + "Longitude")),
                            City = (string)N.Element(d + "City"),
                            Country = (string)N.Element(d + "Country"),
                            RegisteredAt = DateTime.Parse((string)(N.Element(d + "RegisteredAt"))),
                        };
            return query.ToList();
        }
        public static List<Permission> TransformToPermissionList(string message)
        {
            XDocument document = XDocument.Parse(message);
            IEnumerable<Permission> query = from N in document.Descendants(m + "properties")
                        select new Permission
                        {
                            FBIDFromViewed= (string)N.Element(d + "FBIDFromViewed"),
                            FBIDFromViewer = (string)N.Element(d + "FBIDFromViewer"),
                            IsAllowed = (bool)(N.Element(d + "IsAllowed"))
                        };
            return query.ToList();
        }
        public static List<RedZone> TransformToRedZoneList(string message)
        {
            XDocument document = XDocument.Parse(message);
            IEnumerable<RedZone> query = from N in document.Descendants(m + "properties")
                                            select new RedZone
                                            {
                                                City = (string)N.Element(d + "City"),
                                                Country = (string)N.Element(d + "Country"),
                                                Description = (string)N.Element(d + "Description"),
                                                Address = (string)N.Element(d + "Address"),
                                                FacebookID = (string)N.Element(d + "FacebookID"),
                                                Latitude = (double)(N.Element(d + "Latitude")),
                                                Longitude = (double)(N.Element(d + "Longitude")),
                                                RegisteredAt = DateTime.Parse((string)(N.Element(d + "RegisteredAt"))),
                                                Radius = (int)(N.Element(d + "Radius")),
                                            };
            return query.ToList();
        }


        private static async Task<string> DownloadString(string uri, string tableName)
        {
            HttpClient request = new HttpClient();

            string formatedTime = DateTime.UtcNow.ToString("R", CultureInfo.InvariantCulture);
            request.DefaultRequestHeaders.Add("x-ms-date", formatedTime);

            string authorization = Authentication.GetSignedString(formatedTime,  tableName, Utilities.azureAccount, Utilities.azureKey,"GET","","");
            request.DefaultRequestHeaders.Add("Authorization", authorization);

            try
            {
                return await request.GetStringAsync(uri);
            }
            catch (Exception)
            {
                return null;
            }
        }
        #region CRUD Operations

        private static StringContent GetStringContent(string body)
        {
            StringContent stringContent = new StringContent(body);
            stringContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/atom+xml");
            stringContent.Headers.ContentLength = Encoding.UTF8.GetByteCount(body);
            return stringContent;
        }
        private static HttpClient GetHttpClient(string tableName, string HTTPMethod,string partitionKey, string rowKey)
        {
            HttpClient request = new HttpClient();
            string formatedTime = DateTime.UtcNow.ToString("R", CultureInfo.InvariantCulture);
            string authorization = Authentication.GetSignedString(formatedTime, tableName, Utilities.azureAccount, Utilities.azureKey,HTTPMethod,partitionKey,rowKey);

            request.DefaultRequestHeaders.Add("Authorization", authorization);
            request.DefaultRequestHeaders.Add("x-ms-date", formatedTime);
            request.DefaultRequestHeaders.Add("x-ms-version", "2011-08-18");
            request.DefaultRequestHeaders.Add("DataServiceVersion", "1.0;NetFx");
            request.DefaultRequestHeaders.Add("MaxDataServiceVersion", "2.0;NetFx");
            return request;
        }


        public static string SetProperty = "<d:{0}>{1}</d:{0}>";
        private static string SetTime()
        {
            DateTime time = DateTime.UtcNow;
            string t = time.Year + "-" + time.Month.ToString("00") + "-" +
                time.Day.ToString("00") + "T" +
                time.Hour.ToString("00") + ":" +
                time.Minute.ToString("00") + ":" +
                time.Second.ToString("00") + "Z";
            return t;
        }
        private static string SetProperties(dynamic entity)
        {
            string properties = "";
            foreach (PropertyInfo prop in entity.GetType().GetProperties())
            {
                try
                {
                    if (prop.Name != "PartitionKey" && prop.Name != "RowKey")
                    {
                        string value = prop.GetValue(entity, null).ToString();
                        properties += String.Format(SetProperty, prop.Name, value);
                    }
                }
                catch (Exception)
                { }
            }
            return properties;
        }

        #region Build body for insert operation
        private static string buildBodyForInsertOperation(dynamic entity)
        {
            string properties = SetProperties(entity);
            string time = SetTime();

            string body = String.Format(
                InsertEntity,
                time,
                properties,
                entity.GetType().GetProperty("PartitionKey").GetValue(entity, null).ToString(),
                entity.GetType().GetProperty("RowKey").GetValue(entity, null).ToString());

            return body;
        }
        public static string InsertEntity =
            @"<?xml version=""1.0"" encoding=""utf-8""?>" +
            @"<entry xmlns:d=""http://schemas.microsoft.com/ado/2007/08/dataservices"" " +
            @"xmlns:m=""http://schemas.microsoft.com/ado/2007/08/dataservices/metadata"" " +
            @"xmlns=""http://www.w3.org/2005/Atom"">" +
            @"<title />" +
            @"<updated>{0}</updated>
              <author>
                <name />
              </author>
              <id />" +
            @"<content type=""application/xml"">" +
                @"<m:properties>" +
                "{1}" +
                 "<d:PartitionKey>{2}</d:PartitionKey>" +
                 "<d:RowKey>{3}</d:RowKey>" +
                "</m:properties>" +
              "</content>" +
            "</entry>";
        #endregion

        #region Build body for Update operation
        private static string buildBodyForUpdateOperation(dynamic entity, string tableName)
        {
            string properties = SetProperties(entity);
            string time = SetTime();

            string body = String.Format(
                UpdateEntity,
                time,
                properties,
                entity.GetType().GetProperty("PartitionKey").GetValue(entity, null).ToString(),
                entity.GetType().GetProperty("RowKey").GetValue(entity, null).ToString(),
                tableName);

            return body;
        }
        public static string UpdateEntity =
            @"<?xml version=""1.0"" encoding=""utf-8""?>" +
            @"<entry xmlns:d=""http://schemas.microsoft.com/ado/2007/08/dataservices"" " +
            @"xmlns:m=""http://schemas.microsoft.com/ado/2007/08/dataservices/metadata"" " +
            @"xmlns=""http://www.w3.org/2005/Atom"">" +
            @"<title />" +
            @"<updated>{0}</updated>
              <author>
                <name />
              </author>
                <id>
                    https://smartguard.table.core.windows.net/{4}(PartitionKey='{2}', RowKey='{3}')
                </id>" +
            @"<content type=""application/xml"">" +
                @"<m:properties>" +
                "{1}" +
                 "<d:PartitionKey>{2}</d:PartitionKey>" +
                 "<d:RowKey>{3}</d:RowKey>" +
                "</m:properties>" +
              "</content>" +
            "</entry>";
        #endregion
        #endregion
    }
}
