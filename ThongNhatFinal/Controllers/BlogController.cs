using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PagedList.Core;
using System.Linq;
using ThongNhatFinal.Models;

namespace ThongNhatFinal.Controllers
{
    public class BlogController : Controller
    {

        private readonly ShoppingClothesContext _context;

        public BlogController(ShoppingClothesContext context)
        {
            _context = context;
        }
        [Route("blogs.html", Name = "Blog")]
        public IActionResult Index(int? page)
        {
            var pageNumber = page == null || page <= 0 ? 1 : page.Value;
            var pageSize = 5;
            var IsNews = _context.News
                .AsNoTracking()
                .OrderBy(x => x.PostId);
            var lscategory = _context.Categories
                    .AsNoTracking()
                    .Where(x => x.Publish == true)
                    .OrderBy(x => x.CatId)
                    .ToList();
            ViewBag.lscat = lscategory;
            PagedList<News> models = new PagedList<News>(IsNews, pageNumber, pageSize);
            ViewBag.CurrentPage = pageNumber;
            return View(models);
        }
        [Route("/blog/{Alias}-{id}.html", Name = "BlogDetails")]
        public IActionResult Details(int id)
        {
            var lscategory = _context.Categories
                    .AsNoTracking()
                    .Where(x => x.Publish == true)
                    .OrderBy(x => x.CatId)
                    .ToList();
            ViewBag.lscat = lscategory;
            var news = _context.News.AsNoTracking().SingleOrDefault(x => x.PostId == id);
            if (news == null)
            {
                return RedirectToAction("Index", "Home");
            }
            var relatednews = _context.News
                .AsNoTracking()
                .Where(x => x.Publish == true && x.PostId != id)
                .Take(3)
                .ToList();
            ViewBag.relatednews = relatednews;
            return View(news);
        }
    }
}
