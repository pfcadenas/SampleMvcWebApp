#region licence
// The MIT License (MIT)
// 
// Filename: PostsController.cs
// Date Created: 2014/06/18
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
using System.Threading;
using System.Web.Mvc;
using DataLayer.DataClasses;
using DataLayer.DataClasses.Concrete;
using DataLayer.Startup;
using GenericServices;
using SampleWebApp.Infrastructure;
using ServiceLayer.PostServices;
using SampleWebApp.Models;
using System.Collections.Generic;

namespace SampleWebApp.Controllers
{
    /// <summary>
    /// This is an example of a Controller using GenericServices database commands with a DTO.
    /// In this case we are using normal, non-async commands
    /// </summary>    
    public class PostsController : Controller
    {
        /// <summary>
        /// Note that is Index is different in that it has an optional id to filter the list on.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="service"></param>
        /// <returns></returns>
        [AllowAnonymous]
        public ActionResult Index(int? id, string content, int? blog, IListService service)
        {
            //Return filter content to allow tag cloud link to Post page and filtering by content
            ViewData["content"] = content;

            return View();
        }

        [AllowAnonymous]
        public ActionResult Details(int id, IDetailService service)
        {
            return View(service.GetDetail<DetailPostDto>(id).Result);
        }

        [Authorize]
        public ActionResult Edit(int id, IUpdateSetupService service)
        {
            return View(service.GetOriginal<DetailPostDto>(id).Result);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(DetailPostDto dto, IUpdateService service)
        {
            if (!ModelState.IsValid)
                //model errors so return immediately
                return View(service.ResetDto(dto));

            var response = service.Update(dto);
            if (response.IsValid)
            {
                TempData["message"] = response.SuccessMessage;
                return RedirectToAction("Index");
            }

            //else errors, so copy the errors over to the ModelState and return to view
            response.CopyErrorsToModelState(ModelState, dto);
            return View(dto);
        }

        [Authorize]
        public ActionResult Create(ICreateSetupService setupService)
        {
            var dto = setupService.GetDto<DetailPostDto>();
            return View(dto);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(DetailPostDto dto, ICreateService service)
        {
            if (!ModelState.IsValid)
                //model errors so return immediately
                return View(service.ResetDto(dto));

            var response = service.Create(dto);
            if (response.IsValid)
            {
                TempData["message"] = response.SuccessMessage;
                return RedirectToAction("Index");
            }

            //else errors, so copy the errors over to the ModelState and return to view
            response.CopyErrorsToModelState(ModelState, dto);
            return View(dto);
        }

        [Authorize]
        public ActionResult Delete(int id, IDeleteService service)
        {

            var response = service.Delete<Post>(id);
            if (response.IsValid)
                TempData["message"] = response.SuccessMessage;
            else
                //else errors, so send back an error message
                TempData["errorMessage"] = new MvcHtmlString(response.ErrorsAsHtml());
            
            return RedirectToAction("Index");
        }

        //-----------------------------------------------------
        //Code used in https://www.simple-talk.com/dotnet/.net-framework/the-.net-4.5-asyncawait-commands-in-promise-and-practice/

        [AllowAnonymous]
        public ActionResult NumPosts(SampleWebAppDb db)
        {
            //The cast to object is to stop the View using the string as a view name
            return View((object)GetNumPosts(db));
        }

        [AllowAnonymous]
        public static string GetNumPosts(SampleWebAppDb db)
        {
            var numPosts = db.Posts.Count();
            return string.Format("The total number of Posts is {0}", numPosts);
        }

        //--------------------------------------------
        [AllowAnonymous]
        public ActionResult CodeView()
        {
            return View();
        }

        [AllowAnonymous]
        public ActionResult Delay()
        {
            Thread.Sleep(500);
            return View(500);
        }

        [AllowAnonymous]
        public ActionResult Reset(SampleWebAppDb db)
        {
            DataLayerInitialise.ResetBlogs(db, TestDataSelection.Medium);
            TempData["message"] = "Successfully reset the blogs data";
            return RedirectToAction("Index");
        }        

        [AllowAnonymous]
        public ActionResult TopWords(SampleWebAppDb db)
        {
            string query = "";
            IEnumerable<Post> posts = db.Posts.ToList();
            
            //Build the query to obtain the 5 words most used on all post 
            query = @"SELECT 

                        u.word,
                        sum(u.weigth ) AS weigth

                        FROM 
                        (";

            string queryFrom = "";
            foreach (var post in posts)
            {
                queryFrom += " UNION SELECT * FROM dbo.FindWordByPost(" + post.PostId +")";
            }

            query += queryFrom.Substring(6) + @" )

                        as u

                        GROUP BY u.word
                        ORDER BY sum(u.weigth) DESC";


            var result = db.Database.SqlQuery<TopWordsViewModels>(query).Take(5).ToList();

            return View(result);
        }

        [AllowAnonymous]
        public ActionResult TagCloud(SampleWebAppDb db)
        {
            string query = "";
            IEnumerable<Post> posts = db.Posts.ToList();

            //Build the query to obtain the 20 words most used on all post to create a tag cloud 
            query = @"SELECT 

                        u.word,
                        sum(u.weigth ) AS weigth

                        FROM 
                        (";

            string queryFrom = "";
            foreach (var post in posts)
            {
                queryFrom += " UNION SELECT * FROM dbo.FindWordByPost(" + post.PostId + ")";
            }

            query += queryFrom.Substring(6) + @" )

                        as u

                        GROUP BY u.word
                        ORDER BY sum(u.weigth) DESC";


            var result = db.Database.SqlQuery<TopWordsViewModels>(query).Take(20).ToList();

            return View(result);
        }
    }
}