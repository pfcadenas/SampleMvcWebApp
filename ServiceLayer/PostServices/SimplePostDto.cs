﻿#region licence
// The MIT License (MIT)
// 
// Filename: SimplePostDto.cs
// Date Created: 2014/08/16
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
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.CompilerServices;
using DataLayer.DataClasses.Concrete;
using GenericServices.Core;

[assembly: InternalsVisibleTo("Tests")]

namespace ServiceLayer.PostServices
{
    public class SimplePostDto : EfGenericDto<Post, SimplePostDto>
    {

        [UIHint("HiddenInput")]
        [Key]
        public int PostId { get; set; }

        [UIHint("HiddenInput")]
        public int BlogId { get;  set; }

        public string BloggerName { get;  set; }

        [MinLength(2), MaxLength(128)]
        public string Title { get; set; }

        [Required]
        public string Content { get; set; }

        [ScaffoldColumn(false)]
        public ICollection<Tag> Tags { get;  set; }

        [ScaffoldColumn(false)]
        public ICollection<ApplicationUser> Like { get; set; }

        [ScaffoldColumn(false)]
        public DateTime LastUpdated { get;  set; }

        /// <summary>
        /// When it was last updated in DateTime format
        /// </summary>
        public DateTime LastUpdatedUtc { get { return DateTime.SpecifyKind(LastUpdated, DateTimeKind.Utc); } }

        public string TagNames { get { return string.Join(", ", Tags.Select(x => x.Name)); } }

        [Display(Name = "Likes")]
        public string LikeCount { get { return Like.Count.ToString(); } }


        public ICollection<string> LikeName {
            get
            {
                ICollection<string> answer = new List<string>();

                foreach (ApplicationUser user in Like)
                {
                    answer.Add(user.UserName);
                }
                return answer;
            }
        }

        //----------------------------------------------
        //overridden properties or methods

        protected override CrudFunctions SupportedFunctions
        {
            get { return CrudFunctions.List; }
        }
    }
}
