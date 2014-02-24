using Newtonsoft.Json;
using SmartGuard.Core.Facebook.Authenticate;
using SmartGuard.Core.Facebook.Classes;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace SmartGuard.Core.Facebook.Queries
{
    public class Users
    {
        public static async Task<List<FacebookUser>> DownloadFriendsList()
        {
            HttpClient client = new HttpClient();
            string result = await client.GetStringAsync(
                    "https://graph.facebook.com/" +
                    Utilities.fbUserID +
                    "/friends?fields=" +
                    "name," +
                    "about," +
                    "picture" +
                    "&access_token=" + Utilities.fbToken);

            FacebookData data = new FacebookData();
            data = JsonConvert.DeserializeObject<FacebookData>(result);
            return data.friends;
        }

        public static async Task<FacebookUser> DownloadUserInfo(string fbID)
        {
            HttpClient client = new HttpClient();
            string url = "https://graph.facebook.com/" +
                    fbID +
                    "?fields=" +
                    "name," +
                    "about," +
                    "picture" +
                    "&access_token=" + Utilities.fbToken;
            try
            {
                string result = await client.GetStringAsync(new Uri(url, UriKind.Absolute));
            FacebookUser data = new FacebookUser();
            data = JsonConvert.DeserializeObject<FacebookUser>(result);
            return data;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}