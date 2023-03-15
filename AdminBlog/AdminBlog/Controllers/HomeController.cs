using AdminBlog.Models;
using AdminBlog.Repos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace AdminBlog.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly BlogContext _context;
        private readonly ICategoryRepo _categoryRepo;

        public HomeController(ILogger<HomeController> logger, BlogContext context, ICategoryRepo categoryRepo)
        {
            _logger = logger;
            _context = context;
            _categoryRepo = categoryRepo;
        }

        public IActionResult Login( string Email ,string Password)
        {
            var author = _context.Author.FirstOrDefault(x => x.Email == Email && x.Password == Password);
            if(author == null)
            {
                return RedirectToAction(nameof(Index));
            }
            else
            {
                HttpContext.Session.SetInt32("id", author.Id);
            }

            return RedirectToAction(nameof(Category));
        }

        public IActionResult Category()
        {
            List<Category> list = _context.Categories.ToList();
            return View(list);
        }
       
        public async Task<IActionResult> AddCategory(Category category)
        {
          
            await _context.AddAsync(category);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Category));
        }

        public async Task<IActionResult> DeleteCategory(int? id)
        {
            Category category = await _context.Categories.FindAsync(id);
            _context.Remove(category);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Category));
        }
        public async Task<IActionResult> UpdateCategory(Category category)
        {
            if (category.Id == 0)
            {
                await _context.AddAsync(category);
            }
            else
            {
                 _context.Update(category);
            }
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Category));
        }

        public async Task<IActionResult> CategoryDetails(int id)
        {
            var catagory = await _context.Categories.FindAsync(id);
            return Json(catagory);
        }
        

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}



//public async Task<IActionResult> UpdateCategory(Category category)
//{

//    Category context = await _categoryRepo.GetById(category.Id);

//    if(context == null)
//    {
//        return NotFound();
//    }

//    context.Id = category.Id;
//    context.Name = category.Name;

//    _context.Update(context);

//    await _context.SaveChangesAsync();
//    return RedirectToAction("Category");



//}

