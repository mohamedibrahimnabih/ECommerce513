using ECommerce513.Data;
using ECommerce513.Models;
using ECommerce513.Models.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Mono.TextTemplating;
using System.IO;

namespace ECommerce513.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductController : Controller
    {
        private readonly ApplicationDbContext _context = new();

        public IActionResult Index()
        {
            var products = _context.Products
                .Include(e => e.Category)
                .Include(e => e.Brand)
                .Select(e => new ProductWithBrandNameWithCategoryNameVM()
                {
                    Id = e.Id,
                    Name = e.Name,
                    Price = e.Price,
                    Quantity = e.Quantity,
                    Rate = e.Rate,
                    CategoryName = e.Category.Name,
                    BrandName = e.Brand.Name,
                    Status = e.Status
                });

            return View(products.ToList());
        }

        public IActionResult Create()
        {
            var categories = _context.Categories;
            var brands = _context.Brands;

            ViewBag.categories = categories.ToList();
            ViewData["brands"] = brands.ToList();

            return View(new Product());
        }

        [HttpPost]
        public IActionResult Create(Product product, IFormFile MainImg)
        {
            //ModelState.Remove("Category");
            //ModelState.Remove("Brand");

            if (!ModelState.IsValid)
            {
                ViewBag.categories = _context.Categories.ToList();
                ViewData["brands"] = _context.Brands.ToList();

                return View(product);
            }

            if (MainImg is not null && MainImg.Length > 0)
            {
                // Add file to wwwroot
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(MainImg.FileName);

                var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images", fileName);

                using (var stream = System.IO.File.Create(path))
                {
                    MainImg.CopyTo(stream);
                }

                // Add file Name to product in DB
                product.MainImg = fileName;

                _context.Products.Add(product);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }

            ViewBag.categories = _context.Categories.ToList();
            ViewData["brands"] = _context.Brands.ToList();

            return View(product);
        }

        public IActionResult Edit(int id)
        {
            var product = _context.Products.Find(id);

            if(product is not null)
            {
                var categories = _context.Categories;
                var brands = _context.Brands;

                ViewBag.categories = categories.ToList();
                ViewData["brands"] = brands.ToList();
                
                return View(product);
            }

            return RedirectToAction("NotFoundPage", "Home");
        }

        [HttpPost]
        public IActionResult Edit(Product product, IFormFile MainImg)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.categories = _context.Categories.ToList();
                ViewData["brands"] = _context.Brands.ToList();

                return View(product);
            }

            var productInDb = _context.Products.AsNoTracking().FirstOrDefault(e => e.Id == product.Id);

            if(productInDb is not null)
            {
                if (MainImg is not null && MainImg.Length > 0)
                {
                    // Add file to wwwroot
                    var fileName = Guid.NewGuid().ToString() + Path.GetExtension(MainImg.FileName);

                    var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images", fileName);

                    using (var stream = System.IO.File.Create(path))
                    {
                        MainImg.CopyTo(stream);
                    }

                    // Delete old img from wwwroot
                    var oldPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images", productInDb.MainImg);

                    if (System.IO.File.Exists(oldPath))
                    {
                        System.IO.File.Delete(oldPath);
                    }

                    // Add file Name to product in DB
                    product.MainImg = fileName;
                }
                else
                {
                    product.MainImg = productInDb.MainImg;
                }

                _context.Products.Update(product);
                _context.SaveChanges();

                return RedirectToAction(nameof(Index));
            }

            ViewBag.categories = _context.Categories.ToList();
            ViewData["brands"] = _context.Brands.ToList();

            return View(product);
        }

        public IActionResult Delete(int id)
        {
            var product = _context.Products.Find(id);

            if (product is not null)
            {
                // Delete old img from wwwroot
                var oldPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images", product.MainImg);

                if (System.IO.File.Exists(oldPath))
                {
                    System.IO.File.Delete(oldPath);
                }

                _context.Products.Remove(product);
                _context.SaveChanges();

                return RedirectToAction(nameof(Index));
            }

            return RedirectToAction("NotFoundPage", "Home");
        }

        public IActionResult DeleteImg(int id)
        {
            var product = _context.Products.Find(id);

            if (product is not null)
            {
                // Delete old img from wwwroot
                var oldPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images", product.MainImg);

                if (System.IO.File.Exists(oldPath))
                {
                    System.IO.File.Delete(oldPath);
                }

                product.MainImg = "default-img.png";
                _context.SaveChanges();

                return RedirectToAction(nameof(Edit), new { id = id });
            }

            return RedirectToAction("NotFoundPage", "Home");
        }
    }
}
