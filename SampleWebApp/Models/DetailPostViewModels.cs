using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using DataLayer.DataClasses.Concrete;
using ServiceLayer.PostServices;
using ServiceLayer.CommentServices;

namespace SampleWebApp.Models
{
    public class DetailPostViewModels
    {
        public Comment Comment { get; set; }
        public DetailPostDto DetailPostDto { get; set; }
    }
}
