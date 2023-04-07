using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using PersonelBlogSite.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Mail;
using System.Net;
using System.Web.Helpers;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Authorization;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace PersonelBlogSite.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly BlogContext _context;
        private readonly IConfiguration _configuration;
        private readonly string _apiKey;

        public HomeController(ILogger<HomeController> logger, BlogContext blogContext, IConfiguration configuration)
        {
            _logger = logger;
            _context = blogContext;
            _configuration = configuration;
            _apiKey = configuration["SendGriApiKey"];
        }
         


        public IActionResult Index(int pageNumber = 1)
        {

            ViewData["ShowProfileButton"] = true;

            var id1 = HttpContext.Session.GetInt32("id1");

            Author aut = _context.Author.Find(id1);


            ViewBag.name = aut.Name;
            ViewBag.surname = aut.Surname;
            ViewBag.id = aut.Id;

            int pageSize = 7;

            var curruentPage = pageNumber;

            var list = _context.Blogs.OrderByDescending(x => x.dateTime )
                .Skip((curruentPage - 1 )*pageSize)
                .Take(pageSize)
                .ToList();

            var pageCount = (double)list.Count / pageSize;



            foreach (var blog in list)
            {
                blog.Author = _context.Author.Find(blog.AuthorId);
            }

            ViewBag.PageCount = (int)Math.Ceiling(pageCount);
            ViewBag.CurrenPage = curruentPage;


            var tpllist = Tuple.Create(list, aut);
            return View(tpllist);

        }
        public IActionResult MyBlogs(int id)
        {
            ViewData["ShowProfileButton"] = true;
            var id1 = HttpContext.Session.GetInt32("id1");

            Author aut = _context.Author.Find(id1);

            ViewBag.name = aut.Name;
            ViewBag.surname = aut.Surname;
            ViewBag.id = aut.Id;

            var MyauthorList = _context.Blogs.Join(_context.Author,
                blogss => blogss.AuthorId,
                aut => aut.Id,
                (blogss, aut) => new { blogss, aut }).Where(x => x.aut.Id == id)
                .Select(x => x.blogss).ToList();

            var myBloglist = Tuple.Create(MyauthorList, aut); 

            return View(myBloglist);
        }

        public IActionResult Register()
        {
            ViewData["ShowProfileButton"] = false;
            return View();
        }

        [AllowAnonymous]
        public IActionResult publicIndex()
        {
            ViewData["ShowProfileButton"] = false;
            return View();
        }
        public IActionResult Login()
        {
            ViewData["ShowProfileButton"] = false;
            return View();


        }

        [AllowAnonymous]
        public IActionResult AboutMe()
        {
            ViewData["ShowProfileButton"] = false;
            return View();
        }

        [AllowAnonymous]
        public IActionResult ContactMe()
        {
            ViewData["ShowProfileButton"] = false;
            return View();
        }
        public IActionResult HiddenAboutMe()
        {

            ViewData["ShowProfileButton"] = true;

            var id1 = HttpContext.Session.GetInt32("id1");

            Author aut = _context.Author.Find(id1);


            ViewBag.name = aut.Name;
            ViewBag.surname = aut.Surname;
            ViewBag.id = aut.Id;
            return View();
        }
        public IActionResult HiddenContactMe()
        {
            ViewData["ShowProfileButton"] = true;

            var id1 = HttpContext.Session.GetInt32("id1");

            Author aut = _context.Author.Find(id1);


            ViewBag.name = aut.Name;
            ViewBag.surname = aut.Surname;
            ViewBag.id = aut.Id;

            return View();
        }
        public IActionResult isLogin(string email, string password)
        {

            var author = _context.Author.FirstOrDefault(x => x.Email == email && x.Password == password);
            if (author == null)
            {
                return RedirectToAction(nameof(Login));
            }
            else
            {

                HttpContext.Session.SetInt32("id1", author.Id);
                ViewBag.name = author.Name;
                ViewBag.surname = author.Surname;
                //TempData["id"] = author.Id;
                //TempData["id1"] = author.Id;
                HttpContext.Session.SetInt32("id1", author.Id);

                return RedirectToAction(nameof(Index));
            }

        }
      
        [HttpPost]
        public IActionResult PublicSendMail(Mail mail)
        {
            try
            {
                MailMessage msg = new MailMessage();
                msg.From = new MailAddress("bloginfo14@gmail.com");
                msg.To.Add("blogdeneme845@gmail.com");
                msg.IsBodyHtml = true;
                string content = "Blog sitenizde ki bir kullanıcıdan mesajınız var !!!!" +
                    "<br/> Name : " + mail.FromName + " Email Adres : " + mail.Email;
                content += "<br/> Message : " + mail.Mesaj;

                msg.Subject = mail.Konu;
                msg.Body = content;
                SmtpClient smtpClient = new SmtpClient("smtp.gmail.com", 587);
                smtpClient.UseDefaultCredentials = false;
                smtpClient.Credentials = new NetworkCredential("bloginfo14@gmail.com", "nitevxngcwogjxep");
                smtpClient.EnableSsl = true;
                smtpClient.Send(msg);

                ViewBag.message = "Mail başarılı bir şekilde" + mail.Email +  "gönderildi ! ";
                ModelState.Clear();
            }
            catch (Exception ex)
            {

                ViewBag.message = ex.Message.ToString();
            }

            return RedirectToAction(nameof(ContactMe));
        }

        [HttpPost]
        public IActionResult PrivateSendMail(Mail mail)
        {
            #region This code Mail Sending with SendGrid but its cannot work 
            //var id1 = HttpContext.Session.GetInt32("id1");
            //Author aut = _context.Author.Find(id1);
            //var client = new SendGridClient(_apiKey);
            //var from = new EmailAddress(aut.Email, mail.FromName);
            //var to = new EmailAddress("sametbyrktr47@gmail.com");
            //var body = mail.Mesaj;
            //var htmlContent = "<p>" + mail.Mesaj + "</p>";
            //var msg = MailHelper.CreateSingleEmail(from, to, mail.Konu, body, htmlContent);
            //var response = await client.SendEmailAsync(msg);

            //if(response.StatusCode == System.Net.HttpStatusCode.OK || response.StatusCode == System.Net.HttpStatusCode.Accepted)
            //{
            //    TempData["MailMessage"] = "Mail Başarılı bir şekilde gönderilmiştir";
            //    return RedirectToAction(nameof(HiddenContactMe));
            //}
            //else
            //{
            //    TempData["MailMessage"] = "Mail Başarılı bir şekilde gönderilmiştir : " + response.StatusCode.ToString(); ;
            //    return RedirectToAction(nameof(HiddenContactMe));
            //}

            #endregion
            if (!ModelState.IsValid)
            {
                try
                {

                    var id1 = HttpContext.Session.GetInt32("id1");
                    Author aut = _context.Author.Find(id1);
                    MailMessage msg = new MailMessage();
                    msg.From = new MailAddress("bloginfo14@gmail.com");
                    msg.To.Add("blogdeneme845@gmail.com");
                    msg.IsBodyHtml = true;
                    string content = "Blog sitenizde ki bir kullanıcıdan mesajınız var !!!!" +
                        "<br/> Name : " + mail.FromName + " Email Adres : " + aut.Email;
                    content += "<br/> Message : " + mail.Mesaj;

                    msg.Subject = mail.Konu;
                    msg.Body = content;
                    SmtpClient smtpClient = new SmtpClient("smtp.gmail.com", 587);
                    smtpClient.UseDefaultCredentials = false;
                    smtpClient.Credentials = new NetworkCredential("bloginfo14@gmail.com", "nitevxngcwogjxep");
                    smtpClient.EnableSsl = true;
                    smtpClient.Send(msg);

                    TempData["MailMessage"] = "Mail Başarılı bir şekilde site sahibine gönderilmiştir size geri dönüş yapılacaktır";
                    

                }
                catch (Exception ex)
                {

                    ViewBag.message = ex.Message.ToString();
                }
            }
            else
            {
                TempData["MailMessage"] = "Mail gönderilememiştir , lütfen uygun değerler giriniz";
                
            }

            return RedirectToAction(nameof(HiddenContactMe));
        }

        public IActionResult AccountInfo(int id)
        {
            ViewData["ShowProfileButton"] = true;

            var author = _context.Author.Find(id);
            HttpContext.Session.SetInt32("id", id);
            ViewBag.name = author.Name;
            ViewBag.surname = author.Surname;
            ViewBag.email = author.Email;
            ViewBag.id = author.Id;


            return View();

        }
        
        public async Task<IActionResult> addAuthor(Author author)
        {

            await _context.AddAsync(author);
            await _context.SaveChangesAsync();

            TempData["SuccesMessage"] = "Kayıt başarılı bir şekilde oluşturuldu , Sitemize geçiş ve giriş yapmak için lütfen tamamı tıklayınız";
            return RedirectToAction(nameof(publicIndex));



        }

        public IActionResult addBlog(int id)
        {

            HttpContext.Session.SetInt32("id", id);
            ViewData["ShowProfileButton"] = true;

            var id1 = HttpContext.Session.GetInt32("id1");

            Author aut = _context.Author.Find(id1);


            ViewBag.name = aut.Name;
            ViewBag.surname = aut.Surname;
            ViewBag.id = aut.Id;



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
            HttpContext.Session.SetInt32("id", id);

            var id1 = HttpContext.Session.GetInt32("id1");

            Author aut = _context.Author.Find(id1);


            ViewBag.name = aut.Name;
            ViewBag.surname = aut.Surname;
            ViewBag.id = aut.Id;


            ViewData["ShowProfileButton"] = true;
            List<Blog> viewBlogs = new List<Blog>();
            var blog = _context.Blogs.Find(id);
            blog.Author = _context.Author.Find(blog.AuthorId);
            blog.ImagePath = "/img/" + blog.ImagePath;
            viewBlogs.Add(blog);
            var tpllist = Tuple.Create(viewBlogs.ToList(), aut);

            return View(tpllist);

        }

        public IActionResult EditBlog(int id)
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
