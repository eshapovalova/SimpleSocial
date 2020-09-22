﻿using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using SimpleSocial.Data.Models;
using SimpleSocial.Services.Mapping;
using SimpleSocial.Services.Models.Comments;
using SimpleSocial.Services.Models.Followers;
using SimpleSocial.Services.Models.Users;

namespace SimpleSocial.Services.Models.Posts
{
    public class PostViewModel : IMapFrom<Post>, IHaveCustomMappings
    {
        public int Id { get; set; }

        public UserInfoViewModel User { get; set; }

        public int UserId { get; set; }

        public int WallId { get; set; }

        public string Title { get; set; }

        public string CharactersCount { get; set; }

        public string Content { get; set; }

        public DateTime CreatedOn { get; set; }

        public ICollection<CommentViewModel> Comments { get; set; }

        public ICollection<SimpleUserViewModel> Likes { get; set; }

        public int LikesCount => this.Likes.Count;

        public bool IsLiked { get; set; }

        public void CreateMappings(IProfileExpression configuration)
        {
            configuration.CreateMap<Post, PostViewModel>().ForMember(x => x.CharactersCount,
                x => x.MapFrom(p => p.Content.Length));

            configuration.CreateMap<Post, PostViewModel>().ForMember(x => x.Comments, x => x.MapFrom(p => p.Comments));

            configuration.CreateMap<Post, PostViewModel>().ForMember(x => x.User, x => x.MapFrom(p => p.User));

            configuration.CreateMap<Post, PostViewModel>().ForMember(x => x.Likes, x => x.MapFrom(p => p.Likes.Select(l => l.User)));


        }
    }
}
