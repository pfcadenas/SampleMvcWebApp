using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AutoMapper;
using DataLayer.DataClasses.Concrete;
using ServiceLayer.PostServices;
using ServiceLayer.BlogServices;
using ServiceLayer.TagServices;

namespace SampleWebApp
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Domain to Dto
            Mapper.CreateMap<Post, SimplePostDto>();
            Mapper.CreateMap<Post, DetailPostDto>();            
            Mapper.CreateMap<Blog, BlogListDto>();
            Mapper.CreateMap<Tag, TagListDto>();


            // Dto to Domain
            Mapper.CreateMap<SimplePostDto, Post>()
                .ForMember(c => c.PostId, opt => opt.Ignore());

            Mapper.CreateMap<DetailPostDto, Post>()
                .ForMember(c => c.PostId, opt => opt.Ignore());

            Mapper.CreateMap<BlogListDto, Blog>()
                .ForMember(c => c.BlogId, opt => opt.Ignore());

            Mapper.CreateMap<TagListDto, Tag>()
                .ForMember(c => c.TagId, opt => opt.Ignore());
        }

        protected override void Configure()
        {
            
        }
    }
}