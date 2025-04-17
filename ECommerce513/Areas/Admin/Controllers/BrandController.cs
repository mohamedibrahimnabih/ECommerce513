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

        public IActionResult Edit(int id)
        {
            var brand = _context.Brands.Find(id);

            if (brand is not null)
            {
                return View(brand);
            }

            return RedirectToAction("NotFoundPage", "Home");
        }

        [HttpPost]
        public IActionResult Edit(Brand brand)
        {
            _context.Brands.Update(brand);
            _context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }

        public IActionResult Delete(int id)
        {
            var brand = _context.Brands.Find(id);

            if (brand is not null)
            {
                _context.Brands.Remove(brand);
                _context.SaveChanges();

                return RedirectToAction(nameof(Index));
            }

            return RedirectToAction("NotFoundPage", "Home");
        }
    }
}
