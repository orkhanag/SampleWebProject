using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sample_Web_Project.Data;
using Sample_Web_Project.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Sample_Web_Project.Controllers
{
    public class UserController : Controller
    {

        private readonly ApplicationDbContext _db;

        public UserController(ApplicationDbContext db)
        {
            _db = db;
        }

        public IActionResult Index()
        {
            IEnumerable<User> userList = _db.Users;
            return View(userList);
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(User user)
        {
            if (ModelState.IsValid)
            {
                _db.Add(user);
                _db.SaveChanges();
                return await Login(user.Email, user.Password, "Home", true);
            }
            else
            {
                return View();
            }
        }

        [HttpGet]
        public IActionResult Login(string returnUrl)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string email, string password, string returnUrl, bool isRegister = false)
        {
            var users = _db.Users;
            if (users.Count() > 0)
            {
                if (users.Where(u => u.Email == email).FirstOrDefault().Email == email && users.Where(u => u.Password == password).FirstOrDefault().Password == password)
                {
                    ViewData["ReturnUrl"] = returnUrl;
                    var claims = new List<Claim>();
                    claims.Add(new Claim("email", email));
                    claims.Add(new Claim(ClaimTypes.NameIdentifier, email));
                    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
                    await HttpContext.SignInAsync(claimsPrincipal);
                    if (isRegister)
                    {
                        return RedirectToAction("Index", "Home");
                    }
                    else
                    {
                        return Redirect(returnUrl);
                    }
                }
            }

            TempData["Error"] = "Username or Password incorrect";
            return View("login");
        }

        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            return Redirect("/");
        }
    }
}
