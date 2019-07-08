using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SampleWebApp.Models
{
    public class PostViewModel
    {
        public int PostId { get; set; }
        public string BloggerName { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public string LastUpdated { get; set; }
        public string TagNames { get; set; }
        public bool CanMakeLike { get; set; }
        public int LikeCount { get; set; }
        public int CommentCount { get; set; }        
    }
}
