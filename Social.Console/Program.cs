using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Facebook;

namespace Social.Console
{
    class Program
    {
        private const string AppId = "1781593175413135";
        private const string AppSecret = "81da237deb99915fb6e2432148b98174";
        static void Main(string[] args)
        {
            //GetAppAccessToken();
            var result = GenerateLoginUrl(AppId, "");
            System.Console.ReadLine();
        }

        private static void GetAppAccessToken()
        {
            var client = new FacebookClient();
            dynamic result = client.Get("oauth/access_token", new
            {
                client_id= AppId,
                client_secret = AppSecret,
                grant_type = "client_credentials"
            });
            //result.access_token
            System.Console.WriteLine(result.access_token);
        }

        private static Uri GenerateLoginUrl(string appId, string extendedPermissions)
        {
            var parameters = new Dictionary<string, object>
            {
                ["client_id"] = appId,
                ["redirect_uri"] = "https://www.facebook.com/connect/login_success.html",
                ["response_type"] = "token",
                ["display"] = "popup"
            };
            // The requested response: an access token (token), an authorization code (code), or both (code token).



            // list of additional display modes can be found at http://developers.facebook.com/docs/reference/dialogs/#display

            // add the 'scope' parameter only if we have extendedPermissions.
            if (!string.IsNullOrWhiteSpace(extendedPermissions))
                parameters["scope"] = extendedPermissions;

            // generate the login url
            var fb = new FacebookClient();
            return fb.GetLoginUrl(parameters);
        }

        private static void Auth()
        {
            var client = new FacebookClient();
        }
    }
}
