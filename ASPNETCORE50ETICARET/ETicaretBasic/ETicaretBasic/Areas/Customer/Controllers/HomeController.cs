using ETicaretBasic.Data;
using ETicaretBasic.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ETicaretBasic.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _db;
        public HomeController(ILogger<HomeController> logger, ApplicationDbContext db)
        {
            _db = db;   
            _logger = logger;
        }
        public IActionResult Search(string q)
        {
            if (!String.IsNullOrEmpty(q))
            {
                var ara=_db.Products.Where(i=>i.Title.Contains(q)||i.Description.Contains(q));
                return View(ara);
            }
            return View();  
        }
        public IActionResult CategoryDetails(int? id)
        {
            var product = _db.Products.Where(i => i.CategoryID == id).ToList();
            ViewBag.kategoryId = id;
            return View(product);
        }
        public IActionResult Index()
        {
            var product = _db.Products.Where(i=>i.HomeStatus).ToList();
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            if(claim!=null)
            {
                var count = _db.ShoppingCarts.Where(i => i.ApplicationUserID == claim.Value).ToList().Count();
                HttpContext.Session.SetInt32(Other.SessionShoppingCart, count);
            }
            return View(product);
        }
        public IActionResult Details(int id)
        {
            var product = _db.Products.FirstOrDefault(i => i.ProductID == id);
            ShoppingCart cart = new ShoppingCart()
            {
                Product = product,
                ProductID = product.ProductID
            };
            return View(cart);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public IActionResult Details(ShoppingCart scart)
        {
            scart.ShoppingCartID = 0;
            if (ModelState.IsValid)
            {
                var claimsIdentity = (ClaimsIdentity)User.Identity;
                var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
                scart.ApplicationUserID = claim.Value;
                ShoppingCart cart = _db.ShoppingCarts.FirstOrDefault(
                    u => u.ApplicationUserID == scart.ApplicationUserID && u.ProductID == scart.ProductID);
                if (cart == null)
                {
                    _db.ShoppingCarts.Add(scart);
                }
                else
                {
                    cart.Count += scart.Count;
                }
                _db.SaveChanges();
                var count = _db.ShoppingCarts.Where(i => i.ApplicationUserID == scart.ApplicationUserID).ToList().Count();
                HttpContext.Session.SetInt32(Other.SessionShoppingCart, count);
                return RedirectToAction(nameof(Index));
            }
            else
            {
                var product = _db.Products.FirstOrDefault(i => i.ProductID == scart.ProductID);
                ShoppingCart cart = new ShoppingCart()
                {
                    Product = product,
                    ProductID = product.ProductID
                };
            }

            return View(scart);
        }
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
