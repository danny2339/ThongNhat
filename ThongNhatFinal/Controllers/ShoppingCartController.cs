using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using ThongNhatFinal.Extension;
using ThongNhatFinal.Models;
using ThongNhatFinal.ModelViews;

namespace ThongNhatFinal.Controllers
{
    public class ShoppingCartController : Controller
    {
        private readonly ShoppingClothesContext _context;
        public INotyfService _notyfService { get; }
        public ShoppingCartController(ShoppingClothesContext context, INotyfService notyfService)
        {
            _context = context;
        }
        public List<CartItem> ShoppingCart
        {
            get
            {
                var sc = HttpContext.Session.Get<List<CartItem>>("ShoppingCart");
                if (sc == default(List<CartItem>))
                {
                    sc = new List<CartItem>();
                }
                return sc;
            }
        }
        [HttpPost]
        [Route("api/cart/add")]
        public ActionResult AddtoCart(int productId, int? amount)
        {
            List<CartItem> cart = ShoppingCart;
            try
            {
                CartItem item = cart.SingleOrDefault(x => x.product.ProductId == productId);
                if (item != null)
                {
                    item.amount = item.amount + amount.Value;
                    HttpContext.Session.Set<List<CartItem>>("ShoppingCart", cart);
                }
                else
                {
                    Product ab = _context.Products.SingleOrDefault(x => x.ProductId == productId);
                    item = new CartItem
                    {
                        amount = amount.HasValue ? amount.Value : 1,
                        product = ab
                    };
                    cart.Add(item);
                }
                HttpContext.Session.Set<List<CartItem>>("ShoppingCart", cart);
                return Json(new { success = true });
            }
            catch
            {
                return Json(new { success = false });
            }
        }
        [HttpPost]
        [Route("api/cart/remove")]
        public ActionResult Remove(int productId)
        {
            try
            {
                List<CartItem> cart = ShoppingCart;
                CartItem item = cart.SingleOrDefault(x => x.product.ProductId == productId);
                if (cart != null)
                {
                    cart.Remove(item);
                }
                //Save session
                HttpContext.Session.Set<List<CartItem>>("ShoppingCart", cart);
                return Json(new { success = true });
            }
            catch
            {
                return Json(new { success = false });
            }
        }

        [HttpPost]
        [Route("api/cart/update")]
        public IActionResult UpdateCart(int productID, int? amount)
        {
            var cart = HttpContext.Session.Get<List<CartItem>>("ShoppingCart");
            try
            {
                if (cart != null)
                {
                    CartItem item = cart.SingleOrDefault(p => p.product.ProductId == productID);
                    if (item != null && amount.HasValue)
                    {
                        item.amount = amount.Value;
                    }
                    HttpContext.Session.Set<List<CartItem>>("ShoppingCart", cart);
                }
                return Json(new { success = true });
            }
            catch
            {
                return Json(new { success = false });
            }
        }

        [Route("cart.html", Name = "ShoppingCart")]
        public IActionResult Index()
        {
            var lsCart = ShoppingCart;
            var lscategory = _context.Categories
                    .AsNoTracking()
                    .Where(x => x.Publish == true)
                    .OrderBy(x => x.CatId)
                    .ToList();
            ViewBag.lscat = lscategory;
            return View(ShoppingCart);
        }
    }
}
