using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using DataLayer.DataClasses;
using DataLayer.DataClasses.Concrete;
using SampleWebApp.Models;
using AutoMapper;
using ServiceLayer.PostServices;

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


        public IHttpActionResult GetPosts([FromUri] int? draw, [FromUri] int? start, [FromUri] int? length, [FromUri] string content, [FromUri] int? blog)
        {
            IQueryable<Post> query = db.Posts.Include("Like").Include("Blogger").Include("Tags");            
            
            //filter by Content 
            if (content != null && content != "")
                query = query.Where(x => x.Content.Contains(content));           

            //filter by blog
            if (blog != null && blog != 0)
                query = query.Where(x => x.BlogId == blog);

            int recordsTotal = 0;
            //Partitioning from [start] take [length] objects
            if (start != null && length != null)
            {
                recordsTotal = query.Count(); //total object 
                query = query.OrderByDescending(x => x.LastUpdated).Skip((int)start).Take((int)length);
            }

            ApplicationUser user = db.Users.SingleOrDefault(c => c.UserName == User.Identity.Name);            

            var customerDtos = query
                .ToList()
                .Select(x => new {
                    PostId = x.PostId,
                    BloggerName = x.Blogger.Name,
                    Title = x.Title,
                    Content = x.Content,
                    LastUpdated = x.LastUpdated.ToShortDateString(),
                    TagNames = string.Join(", ", x.Tags.Select(t => t.Name)),
                    CanMakeLike = !x.Like.Contains(user),
                    LikeCount = x.Like.Count
                });

            var list = new TablePartitioningResponseViewModels
            {
                draw = (int)draw,
                recordsTotal = recordsTotal,
                recordsFiltered = recordsTotal,
                aaData = customerDtos
            };

            return Ok(list);
        }
    }
}
