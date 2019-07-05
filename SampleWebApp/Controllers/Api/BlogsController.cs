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

        public IHttpActionResult GetTags([FromUri] int? draw, [FromUri] int? start, [FromUri] int? length)
        {
            IQueryable<Blog> query = db.Blogs.Include("Posts");

            int recordsTotal = 0;
            //Partitioning from [start] take [length] objects
            if (start != null && length != null)
            {
                recordsTotal = query.Count(); //total objects
                query = query.OrderBy(x => x.Name).Skip((int)start).Take((int)length);
            }

            var tagsDtos = query
                .ToList()
                .Select(x => new {
                    BlogId = x.BlogId,
                    Name = x.Name,
                    EmailAddress = x.EmailAddress,
                    NumPosts = x.Posts.Count
                });

            var list = new TablePartitioningResponseViewModels
            {
                draw = (int)draw,
                recordsTotal = recordsTotal,
                recordsFiltered = recordsTotal,
                aaData = tagsDtos
            };

            return Ok(list);
        }
    }
}
