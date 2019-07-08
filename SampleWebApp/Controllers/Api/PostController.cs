#region licence
// The MIT License (MIT)
// 
// Filename: PostController.cs
// Date Created: 2019/06/18
// 
// Copyright (c) 2014 Jon Smith (www.selectiveanalytics.com & www.thereformedprogrammer.net)
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.
#endregion
using System.Linq;
using System.Net;
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

            var postsDtos = query
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
                aaData = postsDtos
            };

            return Ok(list);
        }
    }
}
