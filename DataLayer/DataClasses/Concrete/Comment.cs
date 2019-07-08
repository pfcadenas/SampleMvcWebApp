#region licence
// The MIT License (MIT)
// 
// Filename: Comment.cs
// Date Created: 2019/07/07
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
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DataLayer.DataClasses.Concrete
{
    public class Comment
    {
        public int CommentId { get; set; }
        [MinLength(2)]
        [MaxLength(64)]
        [Required]
        public string Name { get; set; }

        [MaxLength(256)]
        [Required]
        [EmailAddress]
        public string EmailAddress { get; set; }

        [DataType(DataType.MultilineText)]
        [MinLength(2)]
        [MaxLength(1000)]
        [Required]
        public string Content { get; set; }

        public int PostId { get; set; }
        public Post Post { get; set; }

        public bool Accepted { get; set; }

        public override string ToString()
        {
            return string.Format("CommentId: {0}, Name: {1}, EmailAddress: {2}, Content: {3}, Post: {4}",
                CommentId, Name, EmailAddress, Content, Post == null ? "null" : Post.Title);
        }
    }
}
