using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using DataLayer.DataClasses;
using DataLayer.DataClasses.Concrete;
using SampleWebApp.Models;
using ServiceLayer.BlogServices;
using AutoMapper;
using GenericServices;
using ServiceLayer.BlogServices;


namespace SampleWebApp.Controllers.Api
{
    public class BlogsController : ApiController
    {
        private SampleWebAppDb db;

        public BlogsController() {
            db = new SampleWebAppDb();
        }

        //GET /api/blogs
        public IHttpActionResult GetBlogs(string query = null)
        {
            var blogsDtos = new List<Blog>();

            if (!String.IsNullOrWhiteSpace(query))
                blogsDtos = db.Blogs
                   .Where(c => c.Name.Contains(query))
                   .ToList();
            else
                blogsDtos = db.Blogs
                    .ToList();

            return Ok(blogsDtos);
        }
    }
}
