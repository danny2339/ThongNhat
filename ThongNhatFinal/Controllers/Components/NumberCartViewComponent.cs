using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using ThongNhatFinal.Extension;
using ThongNhatFinal.ModelViews;

namespace ThongNhatFinal.Controllers.Components
{
    public class NumberCartViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            var cart = HttpContext.Session.Get<List<CartItem>>("ShoppingCart");
            return View(cart);
        }
    }
}
