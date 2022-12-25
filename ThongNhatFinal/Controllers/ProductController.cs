using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PagedList.Core;
using System.Collections.Generic;
using System.Linq;
using ThongNhatFinal.Models;

namespace ThongNhatFinal.Controllers
{
    public class ProductController : Controller
    {
        private readonly ShoppingClothesContext _context;

        public ProductController(ShoppingClothesContext context)
        {
            _context = context;
        }
        [Route("shop.html", Name = "ShopProduct")]

        public IActionResult Index(int? page)
        {
            try
            {
                var pageNumber = page == null || page <= 0 ? 1 : page.Value;
                var pageSize = 9;
                var lscategory = _context.Categories
                    .AsNoTracking()
                    .Where(x => x.Publish == true)
                    .OrderBy(x => x.CatId)
                    .ToList();
                ViewBag.lscat = lscategory;
                var IsProducts = _context.Products
                    .AsNoTracking()
                    .OrderBy(x => x.DateCreate);
                PagedList<Product> models = new PagedList<Product>(IsProducts, pageNumber, pageSize);
                ViewBag.CurrentPage = pageNumber;
                var total = IsProducts.Count();
                ViewBag.total = total;
                var saleoff = _context.Products
                    .AsNoTracking()
                    .Where(x => x.Active == true && x.Discount > 0)
                    .OrderBy(x => x.DateCreate)
                    .Take(5)
                    .ToList();
                ViewBag.Saleoff = saleoff;
                var latestproduct = _context.Products
                    .AsNoTracking()
                    .Where(x => x.Active == true && x.Cat.CatId == 2)
                    .OrderBy(x => x.DateCreate)
                    .Take(3)
                    .ToList();
                ViewBag.Latest = latestproduct;
                var latestproduct1 = _context.Products
                   .AsNoTracking()
                   .Where(x => x.Active == true && x.Cat.CatId == 2)
                   .OrderBy(x => x.DateCreate)
                   .Take(3)
                   .ToList();
                ViewBag.Latest1 = latestproduct1;

                return View(models);
            }
            catch
            {
                return RedirectToAction("Index", "Home");
            }

        }
        [Route("/{Alias}", Name = "ListProduct")]

        public IActionResult List(string Alias, int page = 1)
        {
            try
            {
                var lscategory = _context.Categories
                    .AsNoTracking()
                    .Where(x => x.Publish == true)
                    .OrderBy(x => x.CatId)
                    .ToList();
                ViewBag.lscat = lscategory;
                var pageSize = 6;
                var category = _context.Categories.AsNoTracking().SingleOrDefault(x => x.Alias == Alias);
                var IsProducts = _context.Products
                    .AsNoTracking()
                    .Where(x => x.CatId == category.CatId)
                    .OrderBy(x => x.DateCreate);
                var total = IsProducts.Count();
                ViewBag.total = total;
                PagedList<Product> models = new PagedList<Product>(IsProducts, page, pageSize);
                ViewBag.CurrentPage = page;
                ViewBag.CurrentCat = category;
                return View(models);
            }
            catch
            {
                return RedirectToAction("Index", "Home");
            }

        }
        [Route("/{Alias}-{id}.html", Name = "ProductDetails")]
        public IActionResult Details(int id)
        {
            try
            {
                var lscategory = _context.Categories
                    .AsNoTracking()
                    .Where(x => x.Publish == true)
                    .OrderBy(x => x.CatId)
                    .ToList();
                ViewBag.lscat = lscategory;
                var product = _context.Products.Include(x => x.Cat).FirstOrDefault(x => x.ProductId == id);

                if (product == null)
                {
                    return RedirectToAction("Index");
                }
                var relatedproducts = _context.Products
                .AsNoTracking()
                .Where(x => x.Active == true && x.ProductId != id && x.CatId == product.CatId)
                .OrderBy(x => x.ProductId)
                .Take(3)
                .ToList();
                ViewBag.relatedproducts = relatedproducts;

                return View(product);
            }
            catch
            {
                return RedirectToAction("Index", "Home");
            }

        }
        [HttpPost]
        public IActionResult FindProduct(string keyword)
        {
            List<Product> ls = new List<Product>();
            if (string.IsNullOrEmpty(keyword) || keyword.Length < 1)
            {
                return PartialView("ListProductsSearchPartial", null);
            }
            ls = _context.Products
                .AsNoTracking()
                .Include(a => a.Cat)
                .Where(x => x.ProductName.Contains(keyword))
                .OrderBy(x => x.ProductName)
                .Take(10)
                .ToList();
            if (ls == null)
            {
                return PartialView("ListProductsSearchPartial", null);
            }
            else
            {
                return PartialView("ListProductsSearchPartial", ls);
            }

        }
    }
}
