using Microsoft.EntityFrameworkCore;

namespace PersonelBlogSite.Models
{
    public class BlogContext : DbContext
    {
        public BlogContext(DbContextOptions<BlogContext> options) : base(options)
        {


        }
        public DbSet<Author> Author { get; set; } // tablosunun oluşmasını istediğimiz sınıflar
        public DbSet<Blog> Blogs { get; set; }
        public DbSet<Category> Categories { get; set; }
    }
}
