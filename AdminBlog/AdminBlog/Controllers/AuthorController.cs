using AdminBlog.Models;
using AdminBlog.Repos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AdminBlog.Controllers
{
    public class AuthorController : Controller
    {
        private readonly ILogger<AuthorController> _logger;
        private readonly BlogContext _context;
        private readonly ICategoryRepo _categoryRepo;

        public AuthorController(ILogger<AuthorController> logger, BlogContext context, ICategoryRepo categoryRepo)
        {
            _logger = logger;
            _context = context;
            _categoryRepo = categoryRepo;
        }
        public IActionResult Author()
        {
            List<Author> list = _context.Author.ToList();
            return View(list);
        }


        public async Task<IActionResult> AddAuthor(Author author)

        {

            await _context.AddAsync(author);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Author));

        }
        public async Task<IActionResult> DeleteAuthor(int? id)
        {
            Author author = await _context.Author.FindAsync(id);
            _context.Remove(author);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Author));
        }

        public async Task<IActionResult> UpdateAuthor(Author author)
        {
            if (author.Id == 0)
            {
                await _context.AddAsync(author);
            }
            else
            {
                _context.Update(author);
            }
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Author));
        }
        public async Task<IActionResult> AuthorDetails(int id)
        {
            var author = await _context.Author.FindAsync(id);
            return Json(author);
        }

        public IActionResult Privacy()
        {
            return View();
        }


        public IActionResult Index()
        {
            return View();
        }
    }
}
