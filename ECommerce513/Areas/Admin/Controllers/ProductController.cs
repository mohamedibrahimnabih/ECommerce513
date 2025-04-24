using ECommerce513.Data;
using ECommerce513.Models;
using ECommerce513.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Mono.TextTemplating;

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

            return View();
        }

        [HttpPost]
        public IActionResult Create(Product product)
        {
            _context.Products.Add(product);
            _context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }

        public IActionResult Edit(int id)
        {
            var product = _context.Products.Find(id);

            var categories = _context.Categories;
            var brands = _context.Brands;

            ViewBag.categories = categories.ToList();
            ViewData["brands"] = brands.ToList();

            return View(product);
        }

        [HttpPost]
        public IActionResult Edit(Product product)
        {
            _context.Products.Update(product);
            _context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }
    }
}
