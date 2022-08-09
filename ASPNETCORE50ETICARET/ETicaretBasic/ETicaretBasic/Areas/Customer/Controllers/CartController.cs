using ETicaretBasic.Data;
using ETicaretBasic.Models;
using Iyzipay;
using Iyzipay.Model;
using Iyzipay.Request;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace ETicaretBasic.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class CartController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly IEmailSender _emailSender;
        private readonly UserManager<IdentityUser> _userManager;
        [BindProperty]
        public ShoppingCartWm ShoppingCartWm { get; set; }  

        public CartController(ApplicationDbContext db, IEmailSender emailSender, UserManager<IdentityUser> userManager)
        {
            _db = db;
           _emailSender = emailSender;
            _userManager = userManager;
        }
        public IActionResult Summary()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            ShoppingCartWm = new ShoppingCartWm()
            {
                OrderHeader = new Models.OrderHeader(),
                ListCart = _db.ShoppingCarts.Where(i => i.ApplicationUserID == claim.Value).Include(i => i.Product)
            };
            foreach(var item in ShoppingCartWm.ListCart)
            {
                item.Price = item.Product.Price;

                ShoppingCartWm.OrderHeader.OrderTotal = (item.Count * item.Product.Price);

            }
            return View(ShoppingCartWm);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Summary(ShoppingCartWm model)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            ShoppingCartWm.ListCart=_db.ShoppingCarts.Where(i=>i.ApplicationUserID==claim.Value).Include(i => i.Product);
            ShoppingCartWm.OrderHeader.OrderStatus = Other.Durum_Beklemede;
            ShoppingCartWm.OrderHeader.ApplicationUserID = claim.Value;
            ShoppingCartWm.OrderHeader.OrderDate = DateTime.Now;
            _db.OrderHeaders.Add(ShoppingCartWm.OrderHeader);
            _db.SaveChanges();
            foreach(var item in ShoppingCartWm.ListCart)
            {
                item.Price = item.Product.Price;
                OrderDetails orderDetails = new OrderDetails()
                {
                    ProductID = item.ProductID,
                    OrderID = ShoppingCartWm.OrderHeader.ShoppingCartID,
                    Price = item.Price,
                    Count = item.Count
                };
                ShoppingCartWm.OrderHeader.OrderTotal += item.Count * item.Product.Price;
                model.OrderHeader.OrderTotal += item.Count * item.Product.Price;
                _db.OrderDetailss.Add(orderDetails);
            }
            var payment = PaymentProcess(model);
            _db.ShoppingCarts.RemoveRange(ShoppingCartWm.ListCart);
            _db.SaveChanges();
            HttpContext.Session.SetInt32(Other.SessionShoppingCart, 0);
            return RedirectToAction("SiparisTamam");

        }

        private Payment PaymentProcess(ShoppingCartWm model)
        {
            Options options = new Options();
            options.ApiKey = "sandbox-RHAVX5WN4IzboTUQwxQtotqLdAWQjoma";
            options.SecretKey = "sandbox-JnkYQR5GxI0hyQNdMqoyX0mGjGEL7UD6";
            options.BaseUrl = "https://sandbox-api.iyzipay.com";

            CreatePaymentRequest request = new CreatePaymentRequest();
            request.Locale = Locale.TR.ToString();
            request.ConversationId = new Random().Next(1111,9999).ToString();
            request.Price = model.OrderHeader.OrderTotal.ToString();
            request.PaidPrice = model.OrderHeader.OrderTotal.ToString();
            request.Currency = Currency.TRY.ToString();
            request.Installment = 1;
            request.BasketId = "B67832";
            request.PaymentChannel = PaymentChannel.WEB.ToString();
            request.PaymentGroup = PaymentGroup.PRODUCT.ToString();

            //PaymentCard paymentCard = new PaymentCard();
            //paymentCard.CardHolderName = "John Doe";
            //paymentCard.CardNumber = "5528790000000008";
            //paymentCard.ExpireMonth = "12";
            //paymentCard.ExpireYear = "2030";
            //paymentCard.Cvc = "123";
            //paymentCard.RegisterCard = 0;
            //request.PaymentCard = paymentCard;

            PaymentCard paymentCard = new PaymentCard();
            paymentCard.CardHolderName =model.OrderHeader.CartName;
            paymentCard.CardNumber = model.OrderHeader.CartNumber;
            paymentCard.ExpireMonth = model.OrderHeader.ExpMonth;
            paymentCard.ExpireYear = model.OrderHeader.ExpYear;
            paymentCard.Cvc = model.OrderHeader.Cvc;
            paymentCard.RegisterCard = 0;
            request.PaymentCard = paymentCard;

            Buyer buyer = new Buyer();
            buyer.Id = model.OrderHeader.ShoppingCartID.ToString();
            buyer.Name =model.OrderHeader.Name;
            buyer.Surname = model.OrderHeader.Surname;
            buyer.GsmNumber = model.OrderHeader.PhoneNumber;
            buyer.Email = "email@email.com";
            buyer.IdentityNumber = "74300864791";
            buyer.LastLoginDate = "2015-10-05 12:43:35";
            buyer.RegistrationDate = "2013-04-21 15:12:09";
            buyer.RegistrationAddress = model.OrderHeader.Address;
            buyer.Ip = "85.34.78.112";
            buyer.City = model.OrderHeader.İl;
            buyer.Country = "Turkey";
            buyer.ZipCode = model.OrderHeader.PostaKodu;
            request.Buyer = buyer;

            Address shippingAddress = new Address();
            shippingAddress.ContactName = "Jane Doe";
            shippingAddress.City = "Istanbul";
            shippingAddress.Country = "Turkey";
            shippingAddress.Description = "Nidakule Göztepe, Merdivenköy Mah. Bora Sok. No:1";
            shippingAddress.ZipCode = "34742";
            request.ShippingAddress = shippingAddress;

            Address billingAddress = new Address();
            billingAddress.ContactName = "Jane Doe";
            billingAddress.City = "Istanbul";
            billingAddress.Country = "Turkey";
            billingAddress.Description = "Nidakule Göztepe, Merdivenköy Mah. Bora Sok. No:1";
            billingAddress.ZipCode = "34742";
            request.BillingAddress = billingAddress;

            List<BasketItem> basketItems = new List<BasketItem>();
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            foreach (var item in _db.ShoppingCarts.Where(i => i.ApplicationUserID == claim.Value).Include(i => i.Product))
            {
                basketItems.Add(new BasketItem()
                {
                    Id = item.ProductID.ToString(),
                    Name = item.Product.Title,
                    Category1 = item.Product.CategoryID.ToString(),
                    ItemType = BasketItemType.PHYSICAL.ToString(),
                    Price = (item.Price * item.Count).ToString()

                });
            }
            request.BasketItems = basketItems;
            return  Payment.Create(request, options);
        }

        public IActionResult SiparisTamam()
        {
            return View();
        }
        public IActionResult Index()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            ShoppingCartWm = new ShoppingCartWm()
            {
                OrderHeader = new Models.OrderHeader(),
                ListCart = _db.ShoppingCarts.Where(i => i.ApplicationUserID == claim.Value).Include(i => i.Product)
            };
            ShoppingCartWm.OrderHeader.OrderTotal = 0;
            ShoppingCartWm.OrderHeader.ApplicationUser=_db.ApplicationUsers.FirstOrDefault(i=>i.Id==claim.Value);
            foreach(var item in ShoppingCartWm.ListCart)
            {
                ShoppingCartWm.OrderHeader.OrderTotal += (item.Count * item.Product.Price);
            }
            return View(ShoppingCartWm);
        }
        [HttpPost]
        [ActionName("Index")]
        public async Task<IActionResult> IndexPost()
        {

            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            var user = _db.ApplicationUsers.FirstOrDefault(i => i.Id == claim.Value);
            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Doğrulama emaili boş");
            }
            var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
            var callbackUrl = Url.Page(
                "/Account/ConfirmEmail",
                pageHandler: null,
                values: new { area = "Identity", userId = user.Id, code = code },
                protocol: Request.Scheme);

            await _emailSender.SendEmailAsync(user.Email, "E-MAİL ONAYLAMA",
                $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>lütfen buraya tıklayın</a>.");
            ModelState.AddModelError(string.Empty, "Email doğrulama kodu gönder");
            return RedirectToAction("Success");

        }
        public IActionResult Success()
        {
            return View();
        }
        public IActionResult Add(int cardId)
        {
            var cart = _db.ShoppingCarts.FirstOrDefault(i =>i.ProductID == cardId);
            cart.Count += 1;
            _db.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
        public IActionResult Decrease(int cardId)
        {
            var cart = _db.ShoppingCarts.FirstOrDefault(i => i.ProductID == cardId);
            if (cart.Count == 1)
            {
                var count = _db.ShoppingCarts.Where(u => u.ApplicationUserID == cart.ApplicationUserID).ToList().Count();
                _db.ShoppingCarts.Remove(cart);
                _db.SaveChanges();
                HttpContext.Session.SetInt32(Other.SessionShoppingCart, count - 1);
            }
            else
            {
                cart.Count -= 1;
                _db.SaveChanges();
            }

            return RedirectToAction(nameof(Index));
        }
        public IActionResult Remove(int cardId)
        {
            var cart = _db.ShoppingCarts.FirstOrDefault(i => i.ProductID == cardId);

            var count = _db.ShoppingCarts.Where(u => u.ApplicationUserID == cart.ApplicationUserID).ToList().Count();
            _db.ShoppingCarts.Remove(cart);
            _db.SaveChanges();
            HttpContext.Session.SetInt32(Other.SessionShoppingCart, count - 1);


            return RedirectToAction(nameof(Index));
        }
    }
}
