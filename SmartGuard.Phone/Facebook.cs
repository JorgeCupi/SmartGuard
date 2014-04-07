using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Facebook;
using Facebook.Client;
using System.Net;
using Newtonsoft.Json;
using QuickMethods;
using SmartGuard.Phone;
using SmartGuard.Core.Facebook.Classes;
using Microsoft.Phone.Controls;
using SmartGuard.Core.Facebook.Queries;
namespace Facebook.Methods
{
    public static class FacebookMethods
    {
        public static void LogOut()
        {
            FacebookSessionClient client = new FacebookSessionClient(App.fbAppID);
            client.Logout();
            new WebBrowser().ClearCookiesAsync();
        }

        public static async Task<string> LogIn()
        {
            string message = String.Empty;
            try
            {
                FacebookSession session = new FacebookSession();
                FacebookSessionClient client = new FacebookSessionClient(App.fbAppID);
                session = await client.LoginAsync(
                    "user_about_me," +
                    "friends_about_me," +
                    "publish_stream");
                App.isAuthenticated = true;
                App.fbToken = session.AccessToken;
                App.fbUserID = session.FacebookId;

                 FacebookUser user = await Users.DownloadUserInfo(App.fbUserID);

                App.fbProfilePicture = user.Picture.data.url;
                App.fbProfileName = user.Name;
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
            WebClient client = new WebClient();
            string result = String.Empty;
            await new WebBrowser().ClearCookiesAsync();
            try
            {
                result = await Internet.DownloadStringAsync(client,
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
