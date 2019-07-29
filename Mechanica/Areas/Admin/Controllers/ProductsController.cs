using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Mechanica.Data;
using Mechanica.Models;
using Mechanica.Models.ViewModel;
using Mechanica.Utility;
using Microsoft.AspNetCore.Hosting.Internal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Mechanica.Controllers
{
    [Area("Admin")]
    public class ProductsController : Controller
    {

        private readonly ApplicationDbContext _db;
        private readonly HostingEnvironment _hostingEnvironment;

        [BindProperty]
        public ProductsViewModel ProductsVM { get; set; }

        public ProductsController(ApplicationDbContext db, HostingEnvironment hostingEnvironment)
        {
            _db = db;
            _hostingEnvironment = hostingEnvironment;

            ProductsVM = new ProductsViewModel()
            {
                ProductTypes = _db.ProductTypes.ToList(),
                Products = new Models.Products()
            };

        }

        public async Task<IActionResult> Index()
        {
            var products = _db.Products.Include(m => m.ProductTypes);
            return View(await products.ToListAsync());
        }


        //GET : PRODUCTS CREATE
        public IActionResult Create()
        {
            return View(ProductsVM);
        }

        //POST : PRODUCTS CREATE
        [HttpPost, ActionName("Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreatePost() //productsVM was binded so i dont have to pass ProductViewModel obj
        {
            if (!ModelState.IsValid)
            {
                return View(ProductsVM);
            }

            _db.Products.Add(ProductsVM.Products);
            await _db.SaveChangesAsync();

            //Saving Image to DB

            string webRootPath = _hostingEnvironment.WebRootPath;
            var files = HttpContext.Request.Form.Files;

            var productsFromDb = _db.Products.Find(ProductsVM.Products.Id);


            if(files.Count != 0)
            {
                //Files has been uploaded
                var uploads = Path.Combine(webRootPath, StaticDetails.ImageFolder);
                var extensionOfTheFile = Path.GetExtension(files[0].FileName);

                using (var filestream = new FileStream(Path.Combine(uploads, ProductsVM.Products.Id + extensionOfTheFile), FileMode.Create))
                {
                    files[0].CopyTo(filestream);
                }
                productsFromDb.Image = @"\" + StaticDetails.ImageFolder + @"\" + ProductsVM.Products.Id + extensionOfTheFile;
            }
            else
            {
                //No uploaded image
                var uploads = Path.Combine(webRootPath, StaticDetails.ImageFolder + @"\" + StaticDetails.DefaultProductImage);
                System.IO.File.Copy(uploads, webRootPath + @"\" + StaticDetails.ImageFolder + @"\" + ProductsVM.Products.Id + @".jpg");
                productsFromDb.Image = @"\" + StaticDetails.ImageFolder + @"\" + ProductsVM.Products.Id + @".jpg";
            }

            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        //GET : PRODUCT EDIT
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            ProductsVM.Products = await _db.Products.Include(m => m.ProductTypes).FirstOrDefaultAsync(m => m.Id == id);

            if(ProductsVM.Products == null)
            {
                return NotFound();
            }

            return View(ProductsVM);

        }

        //POST : PRODUCT EDIT 

        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PostEdit(int id)
        {
            if (ModelState.IsValid)
            {
                string webRootPath = _hostingEnvironment.WebRootPath;
                var files = HttpContext.Request.Form.Files;

                var productFromDb = _db.Products.Where(m => m.Id == ProductsVM.Products.Id).FirstOrDefault();
                if(files.Count > 0 && files[0] != null)
                {
                    var uploads = Path.Combine(webRootPath, StaticDetails.ImageFolder);
                    var extensionNew = Path.GetExtension(files[0].FileName);
                    var extensionOld = Path.GetExtension(productFromDb.Image);

                    if (System.IO.File.Exists(Path.Combine(uploads, ProductsVM.Products.Id + extensionOld)))
                    {
                        System.IO.File.Delete(Path.Combine(uploads, ProductsVM.Products.Id + extensionOld));
                    }

                    using (var filestream = new FileStream(Path.Combine(uploads, ProductsVM.Products.Id + extensionNew), FileMode.Create))
                    {
                        files[0].CopyTo(filestream);
                    }
                    
                    ProductsVM.Products.Image = @"\" + StaticDetails.ImageFolder + @"\" + ProductsVM.Products.Id + extensionNew;
                }

                if(ProductsVM.Products.Image != null)
                {
                    productFromDb.Image = ProductsVM.Products.Image;
                }

                productFromDb.Name = ProductsVM.Products.Name;
                productFromDb.Price = ProductsVM.Products.Price;
                productFromDb.Quantity = ProductsVM.Products.Quantity;
                productFromDb.Weight = ProductsVM.Products.Weight;
                productFromDb.ProductTypeId = ProductsVM.Products.ProductTypeId;
                productFromDb.Available = ProductsVM.Products.Available;
                productFromDb.Capacity = ProductsVM.Products.Capacity;

                await _db.SaveChangesAsync();

                return RedirectToAction(nameof(Index));

            }

            return View(ProductsVM);
        }


        //GET : PRODUCT INFO

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            ProductsVM.Products = await _db.Products.Include(m => m.ProductTypes).FirstOrDefaultAsync(m => m.Id == id);

            if(ProductsVM.Products == null)
            {
                return NotFound();
            }

            return View(ProductsVM);

        }

        //GET : PRODUCT DELETE

        public async Task<IActionResult> Delete(int? id)
        {
            if(id == null)
            {
                return NotFound();
            }

            ProductsVM.Products = await _db.Products.Include(m => m.ProductTypes).FirstOrDefaultAsync(m => m.Id == id);

            if(ProductsVM.Products == null)
            {
                return NotFound();
            }

            return View(ProductsVM);

        }

        //POST : PRODUCT DELETE 

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            string webRootPath = _hostingEnvironment.WebRootPath;
            Products products = await _db.Products.FindAsync(id);

            if(products == null)
            {
                return NotFound();
            }

            var uploads = Path.Combine(webRootPath, StaticDetails.ImageFolder);
            var extension = Path.GetExtension(products.Image);

            if (System.IO.File.Exists(Path.Combine(uploads, products.Id + extension)))
            {
                System.IO.File.Delete(Path.Combine(uploads, products.Id + extension));
            }

            _db.Products.Remove(products);
            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}