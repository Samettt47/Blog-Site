using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc.Rendering;
using PersonelBlogSite.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using System.IO;

namespace PersonelBlogSite.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly BlogContext _context;

        public HomeController(ILogger<HomeController> logger , BlogContext blogContext)
        {
            _logger = logger;
            _context = blogContext;
        }

        public IActionResult Profile()
        {
            return View();
        }

       
        
        public IActionResult Index()
        {
            
                var list = _context.Blogs.Take(4).OrderByDescending(x => x.dateTime).ToList();
                foreach (var blog in list)
                {
                    blog.Author = _context.Author.Find(blog.AuthorId);
                }

          
                return View(list); 

        }

        public IActionResult Register()
        {
            return View();
        }

        public IActionResult publicIndex()
        {
            return View();
        }
        public IActionResult Login()
        {
            return View();
            
            
        }

        public IActionResult isLogin(string email , string password)
        {

            var author = _context.Author.FirstOrDefault(x => x.Email == email && x.Password == password);
            if (author == null )
            {
                ViewData["ShowProfileButton"] = false;

                return RedirectToAction(nameof(Login));
            }
            else
            {
                HttpContext.Session.SetInt32("id", author.Id);
                ViewData["ShowProfileButton"] = true;
                return RedirectToAction(nameof(Index));
            }

          

        }

        public async Task<IActionResult> addAuthor(Author author)
        {

                await _context.AddAsync(author);
                await _context.SaveChangesAsync();

                 TempData["SuccesMessage"] = "Kayıt başarılı bir şekilde oluşturuldu , Sitemize geçiş ve giriş yapmak için lütfen tamamı tıklayınız";
                return RedirectToAction(nameof(publicIndex));


           
        }

        public IActionResult addBlog()
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
            if (blog != null)
            {
                var file = Request.Form.Files.First(); // formdaki dosyayı alma
                // C:\Users\samet\source\repos\PersonelBlogSite\PersonelBlogSite\wwwroot
                string savePath = Path.Combine("C:", "Users", "samet", "source", "repos", "PersonelBlogSite", "PersonelBlogSite", "wwwroot", "img");
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


        public IActionResult Post(int id)
        {
            var blog = _context.Blogs.Find(id);
            blog.Author = _context.Author.Find(blog.AuthorId);
            blog.ImagePath = "/img/" + blog.ImagePath;
            return View(blog);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
