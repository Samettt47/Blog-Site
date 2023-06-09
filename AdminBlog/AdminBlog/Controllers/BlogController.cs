﻿using AdminBlog.Models;
using AdminBlog.Repos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace AdminBlog.Controllers
{
    public class BlogController : Controller
    {
        private readonly ILogger<BlogController> _logger;
        private readonly BlogContext _context;
        private readonly ICategoryRepo _categoryRepo;

        public BlogController(ILogger<BlogController> logger, BlogContext context, ICategoryRepo categoryRepo)
        {
            _logger = logger;
            _context = context;
            _categoryRepo = categoryRepo;
        }
        public IActionResult Index()
        {
            var list = _context.Blogs.ToList();
          
            return View(list);
        }
        public IActionResult Route()
        {
            return RedirectToAction(nameof(Index)); 
        }
        public IActionResult Publish(int id)
        {
            var blog = _context.Blogs.Find(id);
            blog.isPublish = true;
            _context.Update(blog);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
        public IActionResult Blog()
        {
            ViewBag.Categories = _context.Categories.Select(x =>
                new SelectListItem
                {
                    Text = x.Name,
                    Value = x.Id.ToString()
                }).ToList();
            return View();
        }
        public async Task<IActionResult> Save(Blog blog)
        {
            if(blog != null)
            {
                var file = Request.Form.Files.First(); // formdaki dosyayı alma
                // C:\Users\samet\source\repos\PersonelBlogSite\PersonelBlogSite\wwwroot
                string savePath = Path.Combine("C:", "Users", "samet", "source", "repos", "PersonelBlogSite", "PersonelBlogSite", "wwwroot","img");
                // yuklenen klasorun yukarıdaki yola kaydolmasını ıstedgımız  // combine fonksyıonunun yolunu buna gore cerecegız
                var fileName = $"{DateTime.Now:MMddHHmmss}.{file.FileName.Split(".").Last()}"; // eğer seçilen dosya daha oncesınden aynı ısımde varsa oncekı dosya sılınıp yenı dosya uzerıne yazılıyor
                                                                                               // bunu engellemek ıcın gelen zamanı stringe cevırerek aynı ısımden kurtarmaya calısıyoruz( cok takma )
                var fileUrl = Path.Combine(savePath, fileName);
                // BU DOSYAYI KOPYALAMAK ICIN ASSAGIDAKI KOD YAZILIR

                using (var fileStream = new FileStream(fileUrl, FileMode.Create))
                {
                    await file.CopyToAsync(fileStream);
                }

                blog.ImagePath = fileName;
                blog.AuthorId = (int)HttpContext.Session.GetInt32("id");
                await _context.AddAsync(blog);
                await _context.SaveChangesAsync();
                return Json(true);
            }
            return Json(false);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}

