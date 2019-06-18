using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using DataLayer.DataClasses;
using DataLayer.DataClasses.Concrete;
using SampleWebApp.Models;

namespace SampleWebApp.Controllers.Api
{
    public class PostController : ApiController
    {
        private SampleWebAppDb db;

        public PostController() {
            db = new SampleWebAppDb();
        }
        public void Like(PostLikeViewModel postLikeViewModel)
        {       
            Post post = db.Posts.SingleOrDefault(c => c.PostId == postLikeViewModel.PostId);
            if (post == null)
                throw new HttpResponseException(HttpStatusCode.NotFound);

            ApplicationUser user = db.Users.SingleOrDefault(c => c.UserName == User.Identity.Name);
            if (user == null)
                throw new HttpResponseException(HttpStatusCode.NotFound);

            post.Like.Add(user);
           
            db.SaveChanges();            
        }
    }
}
