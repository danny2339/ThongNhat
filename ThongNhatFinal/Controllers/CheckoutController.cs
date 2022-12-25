using AspNetCoreHero.ToastNotification.Abstractions;
using BraintreeHttp;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using PayPal.Core;
using PayPal.v1.Payments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ThongNhatFinal.Extension;
using ThongNhatFinal.Helper;
using ThongNhatFinal.Models;
using ThongNhatFinal.ModelViews;

namespace ThongNhatFinal.Controllers
{
    public class CheckoutController : Controller
    {
        private readonly ShoppingClothesContext _context;
        private readonly string _clientId;
        private readonly string _secretId;
        public INotyfService _notyfService { get; }
        public CheckoutController(ShoppingClothesContext context, INotyfService notyfService, IConfiguration config)
        {
            _context = context;
            _notyfService = notyfService;
            _clientId = config["PaypalSettings:ClientId"];
            _secretId = config["PaypalSettings:SecretId"];
        }
        public List<CartItem> ShoppingCart
        {
            get
            {
                var cr = HttpContext.Session.Get<List<CartItem>>("ShoppingCart");
                if (cr == default(List<CartItem>))
                {
                    cr = new List<CartItem>();
                }
                return cr;
            }
        }

        //GET: Checkout/Index
        [Route("checkout.html", Name = "Checkout")]
        public IActionResult Index(string returnUrl = null)
        {
            var lscategory = _context.Categories
                    .AsNoTracking()
                    .Where(x => x.Publish == true)
                    .OrderBy(x => x.CatId)
                    .ToList();
            ViewBag.lscat = lscategory;
            var cart = HttpContext.Session.Get<List<CartItem>>("ShoppingCart");
            var accountId = HttpContext.Session.GetString("CustomerId");
            BuyVM model = new BuyVM();
            if (accountId != null)
            {
                var kh = _context.Accounts.AsNoTracking().SingleOrDefault(x => x.AccountId == Convert.ToInt32(accountId));
                model.CustomerId = kh.AccountId;
                model.FullName = kh.FullName;
                model.Email = kh.Email;
                model.Phone = kh.Phone;

            }
            ViewBag.ShoppingCart = cart;
            return View(model);
        }

        [HttpPost]
        [Route("checkout.html", Name = "Checkout")]
        public async Task<IActionResult> Index(BuyVM buy)
        {
            var lscategory = _context.Categories
                    .AsNoTracking()
                    .Where(x => x.Publish == true)
                    .OrderBy(x => x.CatId)
                    .ToList();
            ViewBag.lscat = lscategory;
            var cart = HttpContext.Session.Get<List<CartItem>>("ShoppingCart");
            var accountId = HttpContext.Session.GetString("CustomerId");
            var kh = _context.Accounts.AsNoTracking().SingleOrDefault(x => x.AccountId == Convert.ToInt32(accountId));
            BuyVM model = new BuyVM();
            if (accountId != null)
            {
                var khh = _context.Accounts.AsNoTracking().SingleOrDefault(x => x.AccountId == Convert.ToInt32(accountId));
                model.CustomerId = khh.AccountId;
                model.FullName = khh.FullName;
                model.Email = khh.Email;
                model.Phone = khh.Phone;
                _context.Update(khh);
                _context.SaveChanges();
            }
            try
            {
                if (!ModelState.IsValid)
                {
                    Order order = new Order
                    {
                        CustomerId = model.CustomerId,
                        OrderDate = DateTime.Now,
                        TransactStatusId = 1,
                        Deleted = false,
                        Paid = false,
                        Note = Utilities.StripHTML(model.Note),
                        TotalMoney = Convert.ToInt32(cart.Sum(x => x.TotalMoney))
                    };
                    _context.Add(order);
                    _context.SaveChanges();

                    foreach (var item in cart)
                    {
                        OrderDetail ord = new OrderDetail
                        {
                            OrderId = order.OrderId,
                            ProductId = item.product.ProductId,
                            Quantity = item.amount,
                            Total = order.TotalMoney
                        };
                        _context.Add(ord);
                    }
                    _context.SaveChanges();
                    await ModelViews.MailSend.SendGmail("non-reply@homecoffee.com", kh.Email, "Order Products", "You has rencently ordered products on Home Coffee. Please wait for our response!", "tanndgcs190073@fpt.edu.vn", "Tanduy718899");
                    HttpContext.Session.Remove("ShoppingCart");
                    _notyfService.Success("Order Successfully");
                    return RedirectToAction("Success");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                ViewBag.ShoppingCart = cart;
                HttpContext.Session.Remove("ShoppingCart");
                return RedirectToAction("Success");
            }
            ViewBag.ShoppingCart = cart;
            HttpContext.Session.Remove("ShoppingCart");
            return RedirectToAction("Success");
        }

        [Route("order-success.html", Name = "Success")]
        public IActionResult Success()
        {
            var lscategory = _context.Categories
                    .AsNoTracking()
                    .Where(x => x.Publish == true)
                    .OrderBy(x => x.CatId)
                    .ToList();
            ViewBag.lscat = lscategory;
            return View();

        }
        [Authorize]
        public async Task<IActionResult> PaypalCheckout()
        {
            var enr = new SandboxEnvironment(_clientId, _secretId);
            var client = new PayPalHttpClient(enr);
            var cart = HttpContext.Session.Get<List<CartItem>>("ShoppingCart");

            var itemList = new ItemList()
            {
                Items = new List<Item>()
            };
            var total = Math.Round(cart.Sum(x => x.TotalMoney));
            foreach (var item in cart)
            {
                itemList.Items.Add(new Item()
                {
                    Name = item.product.ProductName,
                    Currency = "USD",
                    Price = item.product.Price.ToString(),
                    Quantity = item.amount.ToString(),
                    Sku = "sku",
                    Tax = "0"
                });
            }
            var paypalOrderId = DateTime.Now.Ticks;
            var hostname = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}";
            var payment = new Payment()
            {
                Intent = "sale",
                Transactions = new List<Transaction>()
                {
                    new Transaction()
                    {
                        Amount =new Amount()
                        {
                            Total = total.ToString(),
                            Currency = "USD",
                            Details = new AmountDetails
                            {
                                Tax="0",
                                Shipping="0",
                                Subtotal=total.ToString()
                            }
                        },
                        ItemList =itemList,
                        Description ="Invoice #001",
                        InvoiceNumber = paypalOrderId.ToString()
                    }
                },
                RedirectUrls = new RedirectUrls()
                {
                    CancelUrl = $"{hostname}/Checkout/CheckoutFail",
                    ReturnUrl = $"{hostname}/Checkout/CheckoutSuccess"
                },
                Payer = new Payer()
                {
                    PaymentMethod = "paypal"
                }
            };
            PaymentCreateRequest request = new PaymentCreateRequest();
            request.RequestBody(payment);
            try
            {
                var response = await client.Execute(request);
                var statusCode = response.StatusCode;
                Payment result = response.Result<Payment>();

                var links = result.Links.GetEnumerator();
                string paypalRedirectUrl = null;
                while (links.MoveNext())
                {
                    LinkDescriptionObject lnk = links.Current;
                    if (lnk.Rel.ToLower().Trim().Equals("approval_url"))
                    {
                        paypalRedirectUrl = lnk.Href;
                    }
                }
                return Redirect(paypalRedirectUrl);
            }
            catch (HttpException httpExceotion)
            {
                var statusCode = httpExceotion.StatusCode;
                var debugId = httpExceotion.Headers.GetValues("PayPal-Debug-Id").FirstOrDefault();
                return Redirect("/Checkout/CheckoutFail");
            }
        }

        public IActionResult CheckoutFail()
        {
            return View();
        }

        public async Task<IActionResult> CheckoutSuccess()
        {
            var cart = HttpContext.Session.Get<List<CartItem>>("ShoppingCart");
            var accountId = HttpContext.Session.GetString("CustomerId");
            var kh = _context.Customers.AsNoTracking().SingleOrDefault(x => x.CustomerId == Convert.ToInt32(accountId));
            Order order = new Order
            {
                CustomerId = kh.CustomerId,
                OrderDate = DateTime.Now,
                TransactStatusId = 7,
                Deleted = false,
                Paid = false,
                TotalMoney = Convert.ToInt32(cart.Sum(x => x.TotalMoney))
            };
            _context.Add(order);
            _context.SaveChanges();

            foreach (var item in cart)
            {
                OrderDetail ord = new OrderDetail
                {
                    OrderId = order.OrderId,
                    ProductId = item.product.ProductId,
                    Quantity = item.amount,
                    Total = order.TotalMoney
                };
                _context.Add(ord);
            }
            _context.SaveChanges();
            await ModelViews.MailSend.SendGmail("non-reply@homecoffee.com", kh.Email, "Payment Products", "You has rencently paid products on Home Coffee", "tanndgcs190073@fpt.edu.vn", "Tanduy718899");
            HttpContext.Session.Remove("ShoppingCart");
            _notyfService.Success("Order Successfully");
            return View();
        }
    }
}
