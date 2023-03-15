using AdminBlog.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AdminBlog.Repos
{
   public interface ICategoryRepo
    {
        Task<Category> GetById(int id);
        Task Update(Category category);

        }
        public class CategoryRepo : ICategoryRepo
        {

            private readonly BlogContext _context;

            public CategoryRepo(BlogContext context)
            {
                _context = context;
            }


            public async Task<Category> GetById(int id)
            {
                return await _context.Categories.FindAsync(id);
            }

            public async Task Update(Category category)
            {
                _context.Entry(category).State = EntityState.Modified;
                await _context.SaveChangesAsync();
            }
        }
   
}
