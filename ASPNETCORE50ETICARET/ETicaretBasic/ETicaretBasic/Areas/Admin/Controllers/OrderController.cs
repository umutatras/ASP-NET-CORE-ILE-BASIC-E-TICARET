using ETicaretBasic.Data;
using ETicaretBasic.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace ETicaretBasic.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class OrderController : Controller
    {
        private readonly ApplicationDbContext _db;
    

        public OrderController(ApplicationDbContext db)
        {
            _db = db;
        }
        [BindProperty]
        public OrderDetailsVm OrderVm { get; set; }
        [HttpPost]
        [Authorize(Roles =Other.Role_Admin)]
        public IActionResult Onaylandi()
        {
            OrderHeader orderHeader = _db.OrderHeaders.FirstOrDefault(i => i.ShoppingCartID == OrderVm.OrderHeader.ShoppingCartID);
            orderHeader.OrderStatus = Other.Durum_Onaylandi;
            _db.SaveChanges();
            return RedirectToAction("Index");

        }
        [HttpPost]
        [Authorize(Roles = Other.Role_Admin)]
        public IActionResult KargoyaVer()
        {
            OrderHeader orderHeader = _db.OrderHeaders.FirstOrDefault(i => i.ShoppingCartID == OrderVm.OrderHeader.ShoppingCartID);
            orderHeader.OrderStatus = Other.Durum_Kargoda;
            _db.SaveChanges();
            return RedirectToAction("Index");

        }
        public IActionResult Details(int id)
        {
            OrderVm = new OrderDetailsVm
            {
                OrderHeader = _db.OrderHeaders.FirstOrDefault(i => i.ShoppingCartID == id),
                OrderDetails = _db.OrderDetailss.Where(x => x.OrderID == id).Include(x => x.Product)
            };
            return View(OrderVm);
        }
        public IActionResult Index()
        {

            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            IEnumerable<OrderHeader> orderHeaderList;
            if (User.IsInRole(Other.Role_Admin))
            {
                orderHeaderList = _db.OrderHeaders.ToList();
            }
            else
            {
                orderHeaderList = _db.OrderHeaders.Where(i => i.ApplicationUserID == claim.Value).Include(i => i.ApplicationUser);
            }
            return View(orderHeaderList);
        }
        public IActionResult Beklenen()
        {

            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            IEnumerable<OrderHeader> orderHeaderList;
            if (User.IsInRole(Other.Role_Admin))
            {
                orderHeaderList = _db.OrderHeaders.Where(i => i.OrderStatus == Other.Durum_Beklemede);
            }
            else
            {
                orderHeaderList = _db.OrderHeaders.Where(i => i.ApplicationUserID == claim.Value&&i.OrderStatus==Other.Durum_Beklemede).Include(i => i.ApplicationUser);
            }
            return View(orderHeaderList);
        }
        public IActionResult Onaylanan()
        {

            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            IEnumerable<OrderHeader> orderHeaderList;
            if (User.IsInRole(Other.Role_Admin))
            {
                orderHeaderList = _db.OrderHeaders.Where(i => i.OrderStatus == Other.Durum_Onaylandi);
            }
            else
            {
                orderHeaderList = _db.OrderHeaders.Where(i => i.ApplicationUserID == claim.Value && i.OrderStatus == Other.Durum_Onaylandi).Include(i => i.ApplicationUser);
            }
            return View(orderHeaderList);
        }
        public IActionResult Kargolandı()
        {

            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            IEnumerable<OrderHeader> orderHeaderList;
            if (User.IsInRole(Other.Role_Admin))
            {
                orderHeaderList = _db.OrderHeaders.Where(i => i.OrderStatus == Other.Durum_Kargoda);
            }
            else
            {
                orderHeaderList = _db.OrderHeaders.Where(i => i.ApplicationUserID == claim.Value && i.OrderStatus == Other.Durum_Kargoda).Include(i => i.ApplicationUser);
            }
            return View(orderHeaderList);
        }
    }
}
