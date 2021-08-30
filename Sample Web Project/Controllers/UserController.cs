using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sample_Web_Project.Data;
using Sample_Web_Project.Helpers;
using Sample_Web_Project.Models;
using Sample_Web_Project.Services;
using Sample_Web_Project.Settings;
using Sample_Web_Project.ViewModel;
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
        private readonly IMailService mailService;
        private readonly CryptSettings _cryptSettings;

        public UserController(ApplicationDbContext db, IMailService mailService = null, CryptSettings cryptSettings = null)
        {
            _db = db;
            this.mailService = mailService;
            _cryptSettings = cryptSettings;
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
                string rawPass = user.Password;
                user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);
                _db.Add(user);
                _db.SaveChanges();
                return await Login(user.Email, rawPass);
            }
            else
                return View();
        }

        [HttpGet]
        public IActionResult Login(string returnUrl = "")
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string email, string password, string returnUrl = "")
        {
            var user = _db.Users.Where(u => u.Email == email).FirstOrDefault();
            if (user != null)
            {
                if (BCrypt.Net.BCrypt.Verify(password, user.Password))
                {
                    var claims = new List<Claim>();
                    claims.Add(new Claim("email", user.Email));
                    claims.Add(new Claim("fullname", user.FullName));
                    claims.Add(new Claim("birthDate", user.BirthDate.ToString()));
                    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
                    await HttpContext.SignInAsync(claimsPrincipal);
                    if (returnUrl == "")
                        return RedirectToAction("Index", "Home");
                    else
                    {
                        if (string.IsNullOrEmpty(returnUrl))
                            return RedirectToAction("Index", "Home");
                        return Redirect(returnUrl);
                    }
                }
            }
            TempData["Error"] = "Username or Password incorrect";
            return View("login");
        }

        [HttpGet]
        public IActionResult ForgotPassword(string isMailSent = "")
        {
            ViewData["isMailSent"] = isMailSent;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ForgotPassword([FromForm] MailRequest mailRequest)
        {

            var user = _db.Users.Where(u => u.Email == mailRequest.ToMail).FirstOrDefault();
            var resetLink = "https://localhost:44304/User/ResetPassword" + "?u=" + CryptHelper.EncryptString(user.Id.ToString(), _cryptSettings.StringEncriptionKey);
            mailRequest.Subject = "Password reset";
            mailRequest.Body = resetLink;

            try
            {
                await mailService.SendMailAsync(mailRequest);
                return ForgotPassword("200");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpGet]
        public IActionResult ResetPassword(string u)
        {
            ViewData["UserId"] = u;
            return View();
        }

        [HttpPost]
        public IActionResult ResetPassword(User user)
        {

            var userInsert = _db.Users.Where(u => u.Id == user.Id).FirstOrDefault();
            userInsert.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);
            _db.Update(userInsert);
            _db.SaveChanges();
            return RedirectToAction("Login");
        }

        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            return Redirect("/");
        }
    }
}
