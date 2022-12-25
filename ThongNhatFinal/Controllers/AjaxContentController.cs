using Microsoft.AspNetCore.Mvc;

namespace ThongNhatFinal.Controllers
{
    public class AjaxContentController : Controller
    {
        public IActionResult NumberCart()
        {
            return ViewComponent("NumberCart");
        }
    }
}
