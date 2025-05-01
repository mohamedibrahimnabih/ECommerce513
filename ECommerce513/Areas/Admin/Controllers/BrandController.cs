using ECommerce513.Data;
using ECommerce513.Models;
using ECommerce513.Repository;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace ECommerce513.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class BrandController : Controller
    {
        //private readonly ApplicationDbContext _context = new();
        private readonly BrandRepository _brandRepository = new();

        public async Task<IActionResult> Index()
        {
            var brands = await _brandRepository.GetAsync();

            return View(brands.ToList());
        }

        public IActionResult Create()
        {
            return View(new Brand());
        }

        [HttpPost]
        public async Task<IActionResult> Create(Brand brand)
        {
            if (!ModelState.IsValid)
            {
                return View(brand);
            }

            await _brandRepository.CreateAsync(brand);

            return RedirectToAction(nameof(Index));
        }

        public IActionResult Edit(int id)
        {
            var brand = _brandRepository.GetOne(e => e.Id == id);

            if (brand is not null)
            {
                return View(brand);
            }

            return RedirectToAction("NotFoundPage", "Home");
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Brand brand)
        {
            if (!ModelState.IsValid)
            {
                return View(brand);
            }

            await _brandRepository.EditAsync(brand);

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id)
        {
            var brand = _brandRepository.GetOne(e => e.Id == id);

            if (brand is not null)
            {
                await _brandRepository.DeleteAsync(brand);

                return RedirectToAction(nameof(Index));
            }

            return RedirectToAction("NotFoundPage", "Home");
        }
    }
}
