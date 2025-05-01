using ECommerce513.Data;
using ECommerce513.Models;
using ECommerce513.Repository;
using ECommerce513.Repository.IRepository;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace ECommerce513.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CategoryController : Controller
    {
        private readonly ICategoryRepository _categoryRepository;

        //private readonly ApplicationDbContext _context = new();

        public CategoryController(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        public async Task<IActionResult> Index()
        {
            var categories = await _categoryRepository.GetAsync();

            return View(categories.ToList());
        }

        public IActionResult Create()
        {
            return View(new Category());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Category category)
        {
            if (!ModelState.IsValid)
            {
                return View(category);
            }

            await _categoryRepository.CreateAsync(category);

            //CookieOptions cookieOptions = new();
            //cookieOptions.Expires = DateTime.Now.AddMinutes(30);
            //Response.Cookies.Append("Notification", "Add Product Successfully", cookieOptions);

            TempData["Notification"] = "Add Product Successfully";

            return RedirectToAction(nameof(Index));
        }

        public IActionResult Edit(int id)
        {
            var category = _categoryRepository.GetOne(e => e.Id == id);

            if (category is not null)
            {
                return View(category);
            }

            return RedirectToAction("NotFoundPage", "Home");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Category category)
        {
            if(!ModelState.IsValid)
            {
                return View(category);
            }

            await _categoryRepository.EditAsync(category);

            TempData["Notification"] = "Update Product Successfully";

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id)
        {
            var category = _categoryRepository.GetOne(e => e.Id == id);

            if (category is not null)
            {
                await _categoryRepository.DeleteAsync(category);

                TempData["Notification"] = "Remove Product Successfully";

                return RedirectToAction(nameof(Index));
            }

            return RedirectToAction("NotFoundPage", "Home");
        }
    }
}
