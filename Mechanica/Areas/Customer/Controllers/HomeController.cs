﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Mechanica.Models;
using Mechanica.Data;
using Microsoft.EntityFrameworkCore;
using Mechanica.Extensions;
using Microsoft.AspNetCore.Http;

namespace Mechanica.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {

        private readonly ApplicationDbContext _db;

        public HomeController(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<IActionResult> Index()
        {
            var productList = await _db.Products.Include(m => m.ProductTypes).ToListAsync();
            return View(productList);
        }

        [HttpPost, ActionName("Index")]
        [ValidateAntiForgeryToken]
        public IActionResult IndexAddItemToShoppingCart(int id)
        {
            List<int> listShoppingCart = HttpContext.Session.Get<List<int>>("ssShoppingCart");
            if(listShoppingCart == null)
            {
                listShoppingCart = new List<int>();
            }

            listShoppingCart.Add(id);
            HttpContext.Session.Set("ssShoppingCart", listShoppingCart);

            return RedirectToAction("Index", "Home", new { area = "Customer" });
        }

        public async Task<IActionResult> Details(int id)
        {
            var product = await _db.Products.Include(m => m.ProductTypes).Where(m => m.Id == id).FirstOrDefaultAsync();
            return View(product);
        }

        [HttpPost, ActionName("Details")]
        [ValidateAntiForgeryToken]
        public IActionResult DetailsPost(int id)
        {
            List<int> listShoppingCart = HttpContext.Session.Get<List<int>>("ssShoppingCart");
            if(listShoppingCart == null)
            {
                listShoppingCart = new List<int>();
            }

            listShoppingCart.Add(id);
            HttpContext.Session.Set("ssShoppingCart", listShoppingCart);

            return RedirectToAction("Index", "Home", new { area = "Customer" });
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
