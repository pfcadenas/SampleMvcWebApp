#region licence
// The MIT License (MIT)
// 
// Filename: HomeController.cs
// Date Created: 2014/05/20
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
using System.Web.Mvc;
using SampleWebApp.Models;
using DataLayer.DataClasses;
using DataLayer.DataClasses.Concrete;
using System.Linq;
using GenericServices;
using ServiceLayer.PostServices;
using ServiceLayer.CommentServices;
using SampleWebApp.Infrastructure;

namespace SampleWebApp.Controllers
{
    [AllowAnonymous]
    public class HomeController : Controller
    {
        private SampleWebAppDb db;
        public HomeController()
        {
            db = new SampleWebAppDb();
        }

        public ActionResult Index(int? id, [System.Web.Http.FromUri] string content)
        {
            IQueryable<Post> query = db.Posts.Include("Like").Include("Blogger").Include("Tags").Include("Comment");

            //filter by Content 
            if (content != null && content != "")
                query = query.Where(x => x.Content.Contains(content));

            int recordsTotal = 0;
            //Partitioning from [start] take [length] objects
            int start = id == null || id < 1 ? 0 : ((int)id - 1) * 10;
            int length = 10;
            recordsTotal = query.Count(); //total object 
            query = query.OrderByDescending(x => x.LastUpdated).Skip(start).Take(length);

            ApplicationUser user = db.Users.SingleOrDefault(c => c.UserName == User.Identity.Name);

            var listPost = query
                .ToList()
                .Select(x => new PostViewModel
                {
                    PostId = x.PostId,
                    BloggerName = x.Blogger.Name,
                    Title = x.Title,
                    Content = x.Content,
                    LastUpdated = x.LastUpdated.ToShortDateString(),
                    TagNames = string.Join(", ", x.Tags.Select(t => t.Name)),
                    CanMakeLike = !x.Like.Contains(user),
                    LikeCount = x.Like.Count,
                    CommentCount = x.Comment.Count
                });

            //Sort and list to show last most liked Post
            query = query.OrderByDescending(x => x.LastUpdated.Year).ThenByDescending(x => x.Like.Count).Skip(0).Take(4);
            var listPostMostLike = query
               .ToList()
               .Select(x => new PostViewModel
               {
                   PostId = x.PostId,
                   BloggerName = x.Blogger.Name,
                   Title = x.Title,
                   Content = x.Content,
                   LastUpdated = x.LastUpdated.ToShortDateString(),
                   TagNames = string.Join(", ", x.Tags.Select(t => t.Name)),
                   CanMakeLike = !x.Like.Contains(user),
                   LikeCount = x.Like.Count
               });

            var listToReturn = new TablePartitioningResponseViewModels
            {
                draw = id == null || id < 1 ? 1 : (int)id,
                recordsTotal = recordsTotal,
                aaData = listPost,
                aaData1 = listPostMostLike,
                content = content
            };

            return View(listToReturn);
        }
       

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            return View();
        }

        public ActionResult Internals()
        {
            return View(new InternalsInfo());
        }

        public ActionResult CodeView()
        {
            return View();
        }

        public ActionResult PostDetails(int id, IDetailService service)
        {
            return View(new DetailPostViewModels
            {
                Comment = new Comment(),
                DetailPostDto = service.GetDetail<DetailPostDto>(id).Result
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult PostDetails(Comment comment, ICreateService service, IDetailService serviceDetail)
        {
            if (!ModelState.IsValid)
                //model errors so return immediately
                return View(new DetailPostViewModels
                {
                    Comment = comment,
                    DetailPostDto = serviceDetail.GetDetail<DetailPostDto>(comment.PostId).Result
                });

            var response = service.Create(comment);
            if (response.IsValid)
            {
                TempData["message"] = response.SuccessMessage;
                return RedirectToAction("PostDetails");
            }

            //else errors, so copy the errors over to the ModelState and return to view
            response.CopyErrorsToModelState(ModelState, comment);
            return View(new DetailPostViewModels
            {
                Comment = comment,
                DetailPostDto = serviceDetail.GetDetail<DetailPostDto>(comment.PostId).Result
            });
        }
    }
}