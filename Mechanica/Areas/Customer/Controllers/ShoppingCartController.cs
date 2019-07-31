using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Mechanica.Data;
using Mechanica.Extensions;
using Mechanica.Models;
using Mechanica.Models.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Mechanica.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class ShoppingCartController : Controller
    {
        private readonly ApplicationDbContext _db;

        [BindProperty]
        public ShoppingCartViewModel ShoppingCartVM { get; set; }
        public object Product { get; private set; }

        public ShoppingCartController(ApplicationDbContext db)
        {
            _db = db;
            ShoppingCartVM = new ShoppingCartViewModel()
            {
                Products = new List<Models.Products>()
            };
        }

        public IActionResult Index()
        {
            List<int> listShoppingCart = HttpContext.Session.Get<List<int>>("ssShoppingCart");
            if(listShoppingCart.Count > 0)
            {
                foreach(int cartItem in listShoppingCart)
                {
                    Products prod = _db.Products.Include(p => p.ProductTypes).Where(p => p.Id == cartItem).FirstOrDefault();
                    ShoppingCartVM.Products.Add(prod);
                }
            }
            return View(ShoppingCartVM);
        }
    }
}