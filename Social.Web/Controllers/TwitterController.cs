using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Social.Web.Models;
using Tweetinvi;
using Tweetinvi.Models;
using Tweetinvi.Parameters;
using TweetSharp;
using Twitterizer;
using TwitterStatus = Twitterizer.TwitterStatus;

namespace Social.Web.Controllers
{
    /// <summary>
    /// Implement Twitter SDK
    /// </summary>
    public class TwitterController : BaseController
    {
        private const string CosumerKey = "YLACdDwwOQ9jWf6FzpFpQx8fi";
        private const string CosumerSecret = "UiQ5EAKCOB8xg9XyHcQOYae09DexUy3mQaiYejksvoPQ3JhGHq";

        public TwitterController()
        {
        }
        // GET: Twitter
        public ActionResult Index()
        {
            ViewBag.CurrentUser = CurrentUser;
            return View();
        }

        [HttpPost]
        public ActionResult TwitterAuth()
        {
            var appCreds = new ConsumerCredentials(CosumerKey, CosumerSecret);
            var redirectUrl = $"{Domain}/Twitter/ValidateTwitterAuth";
            var authenticationContext = AuthFlow.InitAuthentication(appCreds, redirectUrl);

            return new RedirectResult(authenticationContext.AuthorizationURL);
        }

        public ActionResult ValidateTwitterAuth()
        {
            // Get some information back from the URL
            var verifierCode = Request.Params.Get("oauth_verifier");
            var authorizationId = Request.Params.Get("authorization_id");

            if (verifierCode != null)
            {
                var userCreds = AuthFlow.CreateCredentialsFromVerifierCode(verifierCode, authorizationId);
                var user = Tweetinvi.User.GetAuthenticatedUser(userCreds);
                CurrentUser = user;
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult DoTwitter(string tweet, HttpPostedFileBase file)
        {
            var fileBytes = GetByteArrayFromFile(file);

            var publishedTweet = Auth.ExecuteOperationWithCredentials(CurrentUser.Credentials, () =>
            {
                var publishOptions = new PublishTweetOptionalParameters();
                if (fileBytes != null)
                {
                    publishOptions.MediaBinaries.Add(fileBytes);
                }

                return Tweet.PublishTweet(tweet, publishOptions);
            });

            var publishedTweet2 = Auth.ExecuteOperationWithCredentials(CurrentUser.Credentials, () =>
            {
                return Tweet.PublishTweet(tweet);
            });

            var timeline = CurrentUser.GetHomeTimeline();
            
            // Get my Home Timeline
            var tweets = Timeline.GetHomeTimeline();
            return RedirectToAction("Index");
        }

        private byte[] GetByteArrayFromFile(HttpPostedFileBase file)
        {
            if (file == null || file.ContentLength == 0)
            {
                return null;
            }

            var memoryStream = new MemoryStream();
            file.InputStream.CopyTo(memoryStream);
            return memoryStream.ToArray();
        }

        [HttpPost]
        public ActionResult DoTwitter2(string message)
        {
           
            var service = new TwitterService(CurrentUser.Credentials.ConsumerKey, CurrentUser.Credentials.ConsumerSecret);
            service.AuthenticateWith(CurrentUser.Credentials.AccessToken, CurrentUser.Credentials.AccessTokenSecret);
            var result = service.SendTweet(new SendTweetOptions
            {
                Status = "mensaje."
            });
            return RedirectToAction("Index");
        }

        public IAuthenticatedUser CurrentUser
        {
            get
            {
                return Session["account"] as IAuthenticatedUser;
            }
            set { Session["account"] = value; }
        }
    }
}