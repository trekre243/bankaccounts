using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using Microsoft.AspNetCore.Identity;
using BankAccounts.Models;

namespace BankAccounts.Controllers
{
    public class UserController : Controller
    {   
        private MyContext dbContext;

        public UserController(MyContext context)
        {
            dbContext = context;
        }

        [HttpGet("")]
        public IActionResult Register()
        {
            int? userId = HttpContext.Session.GetInt32("UserId");
            if(userId != null)
            {
                return RedirectToAction("Transactions", "Transaction", new { uId = userId});
            }

            return View();
        }
        
        [HttpPost("user")]
        public IActionResult Create(User newUser)
        {
            if(ModelState.IsValid)
            {
                if(dbContext.Users.Any(user => user.Email == newUser.Email))
                {
                    ModelState.AddModelError("Email", "Email already in use!");
                }

                PasswordHasher<User> Hasher = new PasswordHasher<User>();
                newUser.Password = Hasher.HashPassword(newUser, newUser.Password);
                dbContext.Add(newUser);
                dbContext.SaveChanges();

                HttpContext.Session.SetInt32("UserId", newUser.UserId);
                HttpContext.Session.SetString("FirstName", newUser.FirstName);
                HttpContext.Session.SetString("LastName", newUser.LastName);
                HttpContext.Session.SetString("Email", newUser.Email);
                return RedirectToAction("Transactions", "Transaction", new { uId = newUser.UserId});
            }
            else
            {
                return View("Register");
            }
        }

        [HttpGet("login")]
        public IActionResult LoginForm()
        {
            int? userId = HttpContext.Session.GetInt32("UserId");
            if(userId != null)
            {
                return RedirectToAction("Transactions", "Transaction",  new { uId = userId});
            }
            return View("Login");
        }

        [HttpPost("login")]
        public IActionResult Login(LoginUser user)
        {
            if(ModelState.IsValid)
            {
                var userInDb = dbContext.Users.FirstOrDefault(u => u.Email == user.Email);

                if(userInDb == null)
                {
                    ModelState.AddModelError("Email", "Invalid Email/Password");
                    return View("Login");
                }

                var hasher = new PasswordHasher<LoginUser>();
                var result = hasher.VerifyHashedPassword(user, userInDb.Password, user.Password);
                if(result == 0)
                {
                    ModelState.AddModelError("Password", "Invalid Email/Password");
                    return View("Login");
                }

                HttpContext.Session.SetInt32("UserId", userInDb.UserId);
                HttpContext.Session.SetString("FirstName", userInDb.FirstName);
                HttpContext.Session.SetString("LastName", userInDb.LastName);
                HttpContext.Session.SetString("Email", userInDb.Email);
                return RedirectToAction("Transactions", "Transaction", new { uId = userInDb.UserId});
            }
            else
            {
                return View("Login");
            }
        }

        [HttpGet("logout")]
        public RedirectToActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Register");
        }

    }
}
