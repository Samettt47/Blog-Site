﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AdminBlog.Models
{
    public class Blog
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Subtitle { get; set; }
        public string Content { get; set; }
        public string ImagePath { get; set; } // resmi direkt veritabanını vermektense yolu vereceğiz
        public bool isPublish { get; set; } // yazılan bloglar direkt paylaşılmak zorunda deil kaydedlip durabilir bu yuzden
        public DateTime dateTime { get; set; } = DateTime.Now;
        public Author Author { get; set; }
        public int AuthorId { get; set; }
        public Category Category { get; set; }
        public int  CategoryId { get; set; }
    }
}
