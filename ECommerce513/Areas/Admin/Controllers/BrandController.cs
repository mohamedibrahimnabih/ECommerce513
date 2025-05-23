﻿using ECommerce513.Data;
using ECommerce513.Models;
using ECommerce513.Repository;
using ECommerce513.Repository.IRepository;
using ECommerce513.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace ECommerce513.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = $"{SD.SuperAdmin},{SD.Admin}")]
    public class BrandController : Controller
    {
        //private readonly ApplicationDbContext _context = new();
        private readonly IBrandRepository _brandRepository;// = new BrandRepository();

        public BrandController(IBrandRepository brandRepository)
        {
            _brandRepository = brandRepository;
        }

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
