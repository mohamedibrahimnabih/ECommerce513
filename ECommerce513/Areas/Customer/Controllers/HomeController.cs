using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using ECommerce513.Models;
using ECommerce513.Data;
using Microsoft.EntityFrameworkCore;
using ECommerce513.Models.ViewModels;

namespace ECommerce513.Areas.Customer.Controllers;

[Area("Customer")]
public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly ApplicationDbContext _context = new();

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index(FilterItemsVM filterItemsVM, int page = 1)
    {
        #region Queries
        IQueryable<Product> products = _context.Products.Include(e => e.Category);
        var categories = _context.Categories; 
        #endregion

        #region Filter Bar
        if (filterItemsVM.ProductName is not null)
        {
            products = products.Where(e => e.Name.Contains(filterItemsVM.ProductName));
        }

        if (filterItemsVM.CategoryId > 0 && filterItemsVM.CategoryId <= categories.Count())
        {
            products = products.Where(e => e.CategoryId == filterItemsVM.CategoryId);
        }

        if (filterItemsVM.MinPrice > 0)
        {
            products = products.Where(e => e.Price - e.Price * e.Discount / 100 > filterItemsVM.MinPrice);
        }

        if (filterItemsVM.MaxPrice > 0)
        {
            products = products.Where(e => e.Price - e.Price * e.Discount / 100 < filterItemsVM.MaxPrice);
        }

        //ViewData["ProductName"] = filterItemsVM.ProductName;
        //ViewBag.ProductName = filterItemsVM.ProductName;
        //ViewData["MinPrice"] = filterItemsVM.MinPrice;
        //ViewData["MaxPrice"] = filterItemsVM.MaxPrice;
        //ViewData["CategoryId"] = filterItemsVM.CategoryId;

        //ViewData["FilterItemsVM"] = filterItemsVM;
        #endregion

        #region Pagination
        var totalPageNumber = Math.Ceiling(products.Count() / 8.0); 
        products = products.Skip((page - 1) * 8).Take(8);
        #endregion

        ProductWithFilterVM productWithFilterVM = new()
        {
            Products = products.ToList(),
            Categories = categories.ToList(),
            FilterItemsVM = filterItemsVM,
            TotalPageNumber = totalPageNumber
        };

        return View(productWithFilterVM);
    }

    public IActionResult Details(int id)
    {
        var product = _context.Products.Find(id);

        if (product is not null)
        {
            var relatedProducts = _context.Products.Include(e => e.Category).Where(e => e.Name.Contains(product.Name.Substring(0, 5)) && e.Id != product.Id).Skip(0).Take(4);

            var topProducts = _context.Products.Include(e => e.Category).Where(e => e.Id != product.Id).OrderByDescending(e => e.Traffic).Skip(0).Take(4);

            var sameCategoryProducts = _context.Products.Include(e => e.Category).Where(e => e.CategoryId == product.CategoryId && e.Id != product.Id).Skip(0).Take(4);

            ProductWithRelatedVM productWithRelated = new()
            {
                Product = product,
                RelatedProducts = relatedProducts.ToList(),
                TopProducts = topProducts.ToList(),
                SameCategoryProducts = sameCategoryProducts.ToList()
            };

            product.Traffic++;
            _context.SaveChanges();

            return View(productWithRelated);
        }

        return RedirectToAction(nameof(NotFoundPage));
    }

    public IActionResult NotFoundPage()
    {
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
