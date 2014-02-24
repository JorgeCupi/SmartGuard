using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Facebook;
using Facebook.Client;
using System.Net;
using Newtonsoft.Json;
using SmartGuard.Core.Facebook.Classes;
using SmartGuard.Store;
using System.Net.Http;
using SmartGuard.Core.Facebook.Queries;
namespace Facebook.Methods
{
    public static class FacebookMethods
    {
        public static void LogOut()
        {
            FacebookSessionClient client = new FacebookSessionClient(App.fbAppID);
            client.Logout();
        }

        public static async Task<string> LogIn()
        {
            string message = String.Empty;
            try
            {
                FacebookSession session = new FacebookSession();
                FacebookSessionClient client = new FacebookSessionClient(App.fbAppID);
                session = await client.LoginAsync(
                    "user_about_me,"+
                    "friends_about_me," +
                    "publish_stream");
                App.isAuthenticated = true;
                App.fbToken = session.AccessToken;
                App.fbUserID = session.FacebookId;

                FacebookUser user = await Users.DownloadUserInfo(App.fbUserID);

                App.fbProfilePicture = user.Picture.data.url;
                App.fbProfileName = user.Name;

                RenewToken();
                return "OK";

            }
            catch (InvalidOperationException e)
            {
                message = "Login failed! Exception details: " + e.Message;
                return message;
            }
        }

        public static async Task<List<FacebookUser>> DownloadFriendsList()
        {
            string result = String.Empty;
            try
            {
                result = await DownloadStringAsync(
                new Uri(
                    "https://graph.facebook.com/" + 
                    "me/friends?fields=" +
                            "name," +
                            "picture," +
                            "installed" +
                    "&access_token=" + App.fbToken, UriKind.Absolute));
            }
            catch (Exception)
            { result = null; }

            if (result != null)
            {
                FacebookData data = new FacebookData();
                data = JsonConvert.DeserializeObject<FacebookData>(result);
                return data.friends;
            }
            else return null;
            
        }

        public static async void RenewToken()
        { 
        string extendedToken = "";
            try
            {
                string request = "https://graph.facebook.com/oauth/access_token?" +
                    "grant_type=fb_exchange_token" +
                    "&client_id=" + App.fbAppID +
                    "&client_secret=" + App.fbSecret +
                    "&fb_exchange_token=" + App.fbToken;
                HttpClient client = new HttpClient();
                string result = await client.GetStringAsync(request);
                extendedToken = result.Substring(13);
            }
            catch
            {
                extendedToken = App.fbToken;
            }
            App.fbToken = extendedToken;
        }

        private static async Task<string> DownloadStringAsync(Uri uri)
        {
            HttpClient client = new HttpClient();
            try
            {
                string result = await client.GetStringAsync(uri.OriginalString);
                return result;
            }
            catch (Exception) { return null; }
        }

        public static List<FacebookUser> GetFriendsUsingTheApp(List<FacebookUser> data)
        {

            IEnumerable<FacebookUser> formatedList = from FacebookUser F in data
                                                     where F.UsesApp == true
                                                     orderby F.Name ascending
                                                     select F;
            return formatedList.ToList();
        }
    }
}
