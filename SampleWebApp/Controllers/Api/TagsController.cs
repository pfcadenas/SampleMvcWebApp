#region licence
// The MIT License (MIT)
// 
// Filename: TagsController.cs
// Date Created: 2019/06/22
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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using DataLayer.DataClasses;
using DataLayer.DataClasses.Concrete;
using SampleWebApp.Models;


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

        public IHttpActionResult GetTags([FromUri] int? draw, [FromUri] int? start, [FromUri] int? length)
        {
            IQueryable<Tag> query = db.Tags.Include("Posts");           

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
                    TagId = x.TagId,
                    Name = x.Name,
                    Slug = x.Slug,
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
