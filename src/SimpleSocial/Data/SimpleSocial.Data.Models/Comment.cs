﻿using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace SimpleSocial.Data.Models
{
    public class Comment
    {
        public int Id { get; set; }

        public int AuthorId { get; set; }

        public SimpleSocialUser Author { get; set; }

        public DateTime PostedOn { get; set; } = DateTime.UtcNow;

        public string CommentText { get; set; }

        [ForeignKey("Post")]
        public int PostId { get; set; }

        public Post Post { get; set; }
    }
}
