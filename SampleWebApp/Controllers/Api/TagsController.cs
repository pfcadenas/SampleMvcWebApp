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
    public class TagsController : ApiController
    {
        private SampleWebAppDb db;

        public TagsController() {
            db = new SampleWebAppDb();
        }
     
        //GET /api/tags
        public IHttpActionResult GetTags(string query = null)
        {
            var tagsDtos = new List<Tag>();

            if (!String.IsNullOrWhiteSpace(query))
                tagsDtos = db.Tags
                   .Where(c => c.Name.Contains(query))                   
                   .ToList();
            else
                tagsDtos = db.Tags
                    .ToList();

            return Ok(tagsDtos);
        }
    }
}
