using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using ThongNhatFinal.Models;

namespace ThongNhatFinal.Controllers
{
    public class PageController : Controller
    {
        private readonly ShoppingClothesContext _context;

        public PageController(ShoppingClothesContext context)
        {
            _context = context;
        }
        [Route("/page/{Alias}", Name = "PageDetails")]
        public IActionResult Details(string Alias)
        {
            var lscategory = _context.Categories
                    .AsNoTracking()
                    .Where(x => x.Publish == true)
                    .OrderBy(x => x.CatId)
                    .ToList();
            ViewBag.lscat = lscategory;
            if (string.IsNullOrEmpty(Alias)) return RedirectToAction("Index", "Home");
            var page = _context.Pages.AsNoTracking().SingleOrDefault(x => x.Alias == Alias);
            if (page == null)
            {
                return RedirectToAction("Index", "Home");
            }
            return View(page);
        }
    }
}
