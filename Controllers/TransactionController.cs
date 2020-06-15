using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using Microsoft.AspNetCore.Identity;
using BankAccounts.Models;

namespace BankAccounts.Controllers
{
    public class TransactionController : Controller
    {   
        private MyContext dbContext;

        public TransactionController(MyContext context)
        {
            dbContext = context;
        }

        [HttpGet("account/{uId}")]        
        public IActionResult Transactions(int uId)
        {
            Console.WriteLine("Entered Transactions");
            int? userId = HttpContext.Session.GetInt32("UserId");
            if(userId == null || userId != uId)
            {
                return RedirectToAction("Register", "User");
            }

            var user = dbContext.Users
                .SingleOrDefault(u => u.UserId == uId);

            var userTransactions = dbContext.Transactions
                .Include(t => t.User)
                .Where(t => t.UserId == user.UserId);
            
            ViewBag.Username = $" {user.FirstName} {user.LastName}";
            ViewBag.Balance = userTransactions.Sum(t => t.Amount);
            ViewBag.Transactions = userTransactions.ToList();
            return View();
        }

        [HttpPost("account/{uId}")]
        public IActionResult Create(int uId, WithdrawDeposit wd)
        {
            var user = dbContext.Users
            .SingleOrDefault(u => u.UserId == uId);

            var userTransactions = dbContext.Transactions
                .Include(t => t.User)
                .Where(t => t.UserId == user.UserId);
            
            ViewBag.Username = $" {user.FirstName} {user.LastName}";
            ViewBag.Balance = userTransactions.Sum(t => t.Amount);
            ViewBag.Transactions = userTransactions.ToList();
            if(ModelState.IsValid)
            {
                if(wd.WithdrawSelect == "withdraw")
                {
                    double balance = dbContext.Transactions
                        .Include(t => t.User)
                        .Where(t => t.User.UserId == uId)
                        .Sum(t => t.Amount); 
                    if(wd.Amount > balance)
                    {
                        ModelState.AddModelError("Amount", "Insufficient Funds");

                        return View("Transactions");
                    }
                }
                Transaction newTransaction = new Transaction();
                newTransaction.UserId = uId;
                if(wd.WithdrawSelect == "withdraw")
                {
                    newTransaction.Amount = -wd.Amount;
                }
                else
                {
                    newTransaction.Amount = wd.Amount;
                }
                dbContext.Add(newTransaction);
                dbContext.SaveChanges();
                return RedirectToAction("Transactions", new { uId = uId });
            }
            else
            {
                return View("Transactions");
            }
        }

    }
}
