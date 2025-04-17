using ECommerce513.Data;
using ECommerce513.Models;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce513.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class BrandController : Controller
    {
        private readonly ApplicationDbContext _context = new();

        public IActionResult Index()
        {
            var brands = _context.Brands;

            return View(brands.ToList());
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Brand brand)
        {
            _context.Brands.Add(brand);
            _context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }
    }
}
