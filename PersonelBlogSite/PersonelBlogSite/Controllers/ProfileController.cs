using Microsoft.AspNetCore.Mvc;

namespace PersonelBlogSite.Controllers
{
    public class ProfileController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
