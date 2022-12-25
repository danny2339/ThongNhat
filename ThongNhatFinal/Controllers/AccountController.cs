using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using ThongNhatFinal.Extension;
using ThongNhatFinal.Helper;
using ThongNhatFinal.Models;
using ThongNhatFinal.ModelViews;

namespace ThongNhatFinal.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private readonly ShoppingClothesContext _context;
        public INotyfService _notyfService { get; }
        public AccountController(ShoppingClothesContext context, INotyfService notyfService)
        {
            _context = context;
            _notyfService = notyfService;
        }
        [HttpGet]
        [AllowAnonymous]
        public IActionResult ValidatePhone(string Phone)
        {
            try
            {
                var cs = _context.Accounts.AsNoTracking().SingleOrDefault(x => x.Phone.ToLower() == Phone.ToLower());
                if (cs != null)

                    return Json(data: "Phone number" + Phone + "is existed");
                return Json(data: true);
            }
            catch
            {
                return Json(data: true);
            }

        }

        //Validate Email is existed or not
        [HttpGet]
        [AllowAnonymous]
        public IActionResult ValidateEmail(string Email)
        {
            try
            {
                var cs = _context.Accounts.AsNoTracking().SingleOrDefault(x => x.Email.ToLower() == Email.ToLower());
                if (cs != null)

                    return Json(data: "Email" + Email + "is existed<br />");
                return Json(data: true);
            }
            catch
            {
                return Json(data: true);
            }

        }
        [Route("myaccount.html", Name = "Dashboard")]
        public IActionResult Dashboard()
        {
            var lscategory = _context.Categories
                    .AsNoTracking()
                    .Where(x => x.Publish == true)
                    .OrderBy(x => x.CatId)
                    .ToList();
            ViewBag.lscat = lscategory;
            var accountId = HttpContext.Session.GetString("CustomerId");
            if (accountId != null)
            {
                var kh = _context.Customers.AsNoTracking().SingleOrDefault(x => x.CustomerId == Convert.ToInt32(accountId));
                if (kh != null)
                {
                    var lsOrders = _context.Orders
                        .Include(x => x.TransactStatus)
                        .AsNoTracking()
                        .Where(x => x.CustomerId == kh.CustomerId)
                        .OrderBy(x => x.OrderDate)
                        .ToList();
                    ViewBag.Order = lsOrders;
                    return View(kh);
                }
            }
            return RedirectToAction("LoginAccount");
        }

        [HttpGet]
        [AllowAnonymous]
        [Route("register.html", Name = "register")]
        public IActionResult RegisterAccount()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("register.html", Name = "register")]
        public async Task<IActionResult> RegisterAccount(RegisterVM account)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    string salt = Utilities.GetRandomKey();
                    Customer cs = new Customer
                    {
                        FullName = account.FullName,
                        Phone = account.Phone.Trim().ToLower(),
                        Email = account.Email.Trim().ToLower(),
                        Password = (account.Password + salt.Trim()).ToMD5(),
                        Active = true,
                        Salt = salt,
                        CreateDate = DateTime.Now
                    };
                    Account ac = new Account
                    {
                        FullName = account.FullName,
                        Phone = account.Phone,
                        Email = account.Email,
                        Password = (account.Password + salt.Trim()).ToMD5(),
                        Active = true,
                        Salt = salt,
                        CreateDate = DateTime.Now
                    };
                    try
                    {
                        _context.Add(cs);
                        _context.Add(ac);
                        await _context.SaveChangesAsync();
                        HttpContext.Session.SetString("CustomerId", ac.AccountId.ToString());
                        var AccountId = HttpContext.Session.GetString("CustomerId");
                        var claims = new List<Claim>
                        {
                            new Claim(ClaimTypes.Name, ac.FullName),
                            new Claim("CustomerId", ac.AccountId.ToString()),
                        };
                        ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims, "login");
                        ClaimsPrincipal claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
                        await HttpContext.SignInAsync(claimsPrincipal);
                        return RedirectToAction("Dashboard", "Accounts");
                    }
                    catch
                    {
                        return RedirectToAction("RegisterAccount", "Accounts");
                    }
                }
                else
                {
                    return View(account);
                }
            }
            catch
            {
                return View(account);
            }
        }


        [AllowAnonymous]
        [Route("Login.html", Name = "Login")]
        public IActionResult LoginAccount(string returnUrl = null)
        {
            var accountId = HttpContext.Session.GetString("CustomerId");
            if (accountId != null)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("Login.html", Name = "Login")]
        public async Task<IActionResult> LoginAccount(LoginViewModel cs, string returnUrl = null)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    bool isEmail = Utilities.IsValidEmail(cs.UserName);
                    if (!isEmail) return View(cs);

                    var kh = _context.Accounts
                        .Include(x => x.Role)
                        .AsNoTracking()
                        .SingleOrDefault(x => x.Email.Trim() == cs.UserName);

                    if (kh == null) return RedirectToAction("RegisterAccount");
                    string pass = (cs.Password + kh.Salt.Trim()).ToMD5();
                    if (kh.Password != pass)
                    {
                        _notyfService.Warning("Wrong Login Information");
                        return View(cs);
                    }
                    //Check whether account is disable or not
                    if (kh.Active == false) return RedirectToAction("Notify", "Accounts");

                    //Save session Customer ID
                    HttpContext.Session.SetString("CustomerId", kh.AccountId.ToString());
                    var accountID = HttpContext.Session.GetString("CustomerId");

                    //Identity
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, kh.FullName),

                        new Claim("CustomerId", kh.AccountId.ToString()),
                        new Claim(ClaimTypes.Role, kh.Role.RoleName)
                    };
                    ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims, "login");
                    ClaimsPrincipal claimsPrinciple = new ClaimsPrincipal(claimsIdentity);
                    await HttpContext.SignInAsync(claimsPrinciple);
                    _notyfService.Success("Login Successfully");
                    return RedirectToAction("Index", "Home");
                }
            }
            catch
            {
                return RedirectToAction("RegisterAccount", "Accounts");
            }
            return View(cs);
        }
        [HttpGet]
        [Route("logout.html", Name = "Logout")]
        public IActionResult Logout()
        {
            HttpContext.SignOutAsync();
            HttpContext.Session.Remove("CustomerId");
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public IActionResult ChangePassword(ChangePasswordViewModel model)
        {
            try
            {
                var accountId = HttpContext.Session.GetString("CustomerId");
                if (accountId == null)
                {
                    return RedirectToAction("LoginAccount", "Accounts");
                }
                if (ModelState.IsValid)
                {
                    var tk = _context.Accounts.Find(Convert.ToInt32(accountId));
                    if (tk == null) return RedirectToAction("Index", "Home");
                    var pass = (model.PasswordNow.Trim() + tk.Salt.Trim()).ToMD5();
                    if (pass == tk.Password)
                    {
                        string passnew = (model.Password.Trim() + tk.Salt.Trim()).ToMD5();
                        tk.Password = passnew;
                        _context.Update(tk);
                        _context.SaveChanges();
                        _notyfService.Success("Change Password Successfully");
                        return RedirectToAction("Dashboard", "Accounts");
                    }
                }
            }
            catch
            {
                _notyfService.Error("Change Password Failed");
                return RedirectToAction("Dashboard", "Accounts");
            }
            _notyfService.Error("Change Password Failed");
            return RedirectToAction("Dashboard", "Accounts");
        }
    }
}
