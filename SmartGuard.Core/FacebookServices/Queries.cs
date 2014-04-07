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
            string result = await client.GetStringAsync(String.Format(
                    "https://graph.facebook.com/{0}/friends?fields=name,about,picture&access_token={1}",
                    Utilities.fbUserID,
                    Utilities.fbToken));

            FacebookData data = new FacebookData();
            data = JsonConvert.DeserializeObject<FacebookData>(result);
            return data.friends;
        }

        public static async Task<FacebookUser> DownloadUserInfo(string fbID)
        {
            HttpClient client = new HttpClient();
            try
            {
                string result = await client.GetStringAsync(String.Format(
                    "https://graph.facebook.com/{0}?fields=name,about,picture&access_token={1}",
                    fbID , 
                    Utilities.fbToken));
                return JsonConvert.DeserializeObject<FacebookUser>(result);
            }
            catch (Exception)
            {
                // The lines of code that called this method already have a way
                // to work things out with this null value.
                return null;
            }
        }
    }
}