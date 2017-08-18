using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Social.Web.Models
{
    /// <summary>
    /// Refer https://developers.facebook.com/docs/graph-api/reference/v2.10/user/feed
    /// </summary>
    public class PostFeedModel
    {
        /// <summary>
        /// The main body of the post, otherwise called the status message. Either link, place, or message must be supplied.
        /// </summary>
        public string Message { get; set; }
        /// <summary>
        /// The URL of a link to attach to the post. Either link, place, or message must be supplied. Additional fields associated with link are shown below.
        /// </summary>
        public string Link { get; set; }
        /// <summary>
        /// Page ID of a location associated with this post. Either link, place, or message must be supplied.
        /// </summary>
        public string Place { get; set; }
        /// <summary>
        /// Comma-separated list of user IDs of people tagged in this post. You cannot specify this field without also specifying a place.
        /// </summary>
        public string Tags { get; set; }

        /// <summary>
        /// Facebook ID for an existing picture in the person's photo albums to use as the thumbnail image. They must be the owner of the photo, and the photo cannot be part of a message attachment.
        /// </summary>
        public string ObjectAttachment { get; set; }

        public HttpPostedFileBase File { get; set; }
    }
}