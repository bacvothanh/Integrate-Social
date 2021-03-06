﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Facebook;
using Social.Web.Models;

namespace Social.Web.Controllers
{
    /// <summary>
    /// Implement Facebook SDK
    /// </summary>
    public class HomeController : BaseController
    {
        private const string AppId = "1781593175413135";
        private const string AppSecret = "81da237deb99915fb6e2432148b98174";
        public ActionResult Index()
        {
            var url = GenerateLoginUrl(AppId, "user_posts,publish_actions");
            ViewBag.Url = url.AbsoluteUri;
            ViewBag.CurrentUser = CurrentUser;
            return View();
        }

        private  Uri GenerateLoginUrl(string appId, string extendedPermissions)
        {
            var parameters = new Dictionary<string, object>
            {
                ["client_id"] = appId,
                ["redirect_uri"] = $"{Domain}/Home/LoginSuccess",
                ["response_type"] = "code",
                ["display"] = "popup"
            };

            // add the 'scope' parameter only if we have extendedPermissions.
            if (!string.IsNullOrWhiteSpace(extendedPermissions))
                parameters["scope"] = extendedPermissions;

            // generate the login url
            var fb = new FacebookClient();
            return fb.GetLoginUrl(parameters);
        }


        [HttpGet]
        public ActionResult LoginSuccess()
        {
            var fb = new FacebookClient();
            FacebookOAuthResult result;
            if (fb.TryParseOAuthCallbackUrl(Request.Url, out result))
            {
                if (result.IsSuccess)
                {
                    // result clicked authorized
                    var code = result.Code;
                    dynamic token = fb.Get("oauth/access_token", new
                    {
                        client_id = AppId,
                        client_secret = AppSecret,
                        redirect_uri = $"{Domain}/Home/LoginSuccess",
                        code = code
                    });

                    var accessToken = token.access_token;
                    fb = new FacebookClient(accessToken);
                    dynamic me = fb.Get("me");
                    var account = new AccountInfoViewModel
                    {
                        Id = me.id,
                        Name = me.name,
                        AccessToken = accessToken
                    };

                    CurrentUser = account;
                    return RedirectToAction("Index");
                }
                else
                {
                    var error = result.ErrorDescription;
                }
            }

            return View();
        }

        public AccountInfoViewModel CurrentUser
        {
            get
            {
                return Session["account"] as AccountInfoViewModel;
            }
            set { Session["account"] = value; }
        }

        [HttpPost]
        public ActionResult Post(PostFeedModel model)
        {
            if (model.File != null)
            {
                var bytes = GetByteArrayFromFile(model.File);
                var client = new FacebookClient(CurrentUser.AccessToken);
                client.Post("me/photos", new
                {
                    message = model.Message,
                    file = new FacebookMediaObject
                    {
                        ContentType = model.File.ContentType,
                        FileName = model.File.FileName,
                    }
                        .SetValue(bytes)
                });
            }
            else
            {
                PostMessage(model.Message, model.Link);
            }

            return RedirectToAction("Index");
        }

        public void PostMessage(string message,string link)
        {
            var client = new FacebookClient(CurrentUser.AccessToken);
            client.Post("me/feed", new
            {
                message
            });
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
    }
}