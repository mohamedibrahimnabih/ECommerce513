using ECommerce513.Data;
using ECommerce513.Models;
using ECommerce513.Repository;
using ECommerce513.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Stripe.Checkout;
using System.Threading.Tasks;

namespace ECommerce513.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize]
    public class CartController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ICartRepository _cartRepository;
        private readonly ApplicationDbContext _context;
        private readonly IOrderRepository _orderRepository;

        public CartController(UserManager<ApplicationUser> userManager, ICartRepository cartRepository, ApplicationDbContext context, IOrderRepository orderRepository)
        {
            _userManager = userManager;
            _cartRepository = cartRepository;
            _context = context;
            _orderRepository = orderRepository;
        }

        public async Task<IActionResult> AddToCart(int productId, int count)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction("Details", "Home", new { area = "Customer", id = productId });
            }

            var productCount = _context.Products.FirstOrDefault(e => e.Id == productId).Quantity;

            if (productCount < count)
            {
                ModelState.AddModelError(string.Empty, $"Max Ava.: {productCount}");

                return RedirectToAction("Details", "Home", new { area = "Customer", id = productId });
            }

            var applicationUser = await _userManager.GetUserAsync(User);

            if (applicationUser is not null)
            {
                var cartInDb = _cartRepository.GetOne(e => e.ApplicationUserId == applicationUser.Id && e.ProductId == productId);

                if (cartInDb is not null)
                {
                    cartInDb.Count += count;
                    await _cartRepository.CommitAsync();

                    TempData["Notification"] = $"Update Product int the cart by {count}";
                }
                else
                {
                    await _cartRepository.CreateAsync(new()
                    {
                        ApplicationUserId = applicationUser.Id,
                        ProductId = productId,
                        Count = count
                    });

                    TempData["Notification"] = "Add Product to cart";
                }

                return RedirectToAction("Index", "Home", new { area = "Customer" });
            }

            return BadRequest();
        }

        public async Task<IActionResult> Index()
        {
            var applicationUser = await _userManager.GetUserAsync(User);

            if (applicationUser is not null)
            {
                var carts = await _cartRepository.GetAsync(e => e.ApplicationUserId == applicationUser.Id, includes: [e => e.Product]);

                var totalPrice = carts.Sum(e => e.Product.Price * e.Count);
                ViewBag.totalPrice = totalPrice;

                return View(carts);
            }

            return RedirectToAction("NotFoundPage");
        }

        public async Task<IActionResult> IncrementCount(int productId)
        {
            var applicationUser = await _userManager.GetUserAsync(User);

            if (applicationUser is not null)
            {
                var cart = _cartRepository.GetOne(e => e.ProductId == productId && e.ApplicationUserId == applicationUser.Id);

                cart.Count++;
                await _cartRepository.CommitAsync();

                return RedirectToAction(nameof(Index));
            }

            return RedirectToAction("NotFoundPage");
        }

        public async Task<IActionResult> DecrementCount(int productId)
        {
            var applicationUser = await _userManager.GetUserAsync(User);

            if (applicationUser is not null)
            {
                var cart = _cartRepository.GetOne(e => e.ProductId == productId && e.ApplicationUserId == applicationUser.Id);

                if (cart.Count > 1)
                {
                    cart.Count--;
                    await _cartRepository.CommitAsync();
                }
                else
                {
                    await _cartRepository.DeleteAsync(cart);
                }

                return RedirectToAction(nameof(Index));
            }

            return RedirectToAction("NotFoundPage");
        }

        public async Task<IActionResult> DeleteItem(int productId)
        {
            var applicationUser = await _userManager.GetUserAsync(User);

            if (applicationUser is not null)
            {
                var cart = _cartRepository.GetOne(e => e.ProductId == productId && e.ApplicationUserId == applicationUser.Id);

                await _cartRepository.DeleteAsync(cart);

                return RedirectToAction(nameof(Index));
            }

            return RedirectToAction("NotFoundPage");
        }

        public async Task<IActionResult> Pay()
        {
            var applicationUser = await _userManager.GetUserAsync(User);

            var carts = await _cartRepository.GetAsync(e => e.ApplicationUserId == applicationUser.Id, includes: [e => e.Product]);

            if (applicationUser is not null && carts is not null)
            {
                Order order = new()
                {
                    ApplicationUserId = applicationUser.Id,
                    OrderDate = DateTime.UtcNow,
                    OrderStatus = OrderStatus.Pending,
                    TotalPrice = carts.Sum(e => e.Product.Price * e.Count),
                    TransactionStatus = false
                };

                await _orderRepository.CreateAsync(order);

                var options = new SessionCreateOptions
                {
                    PaymentMethodTypes = new List<string> { "card" },
                    LineItems = new List<SessionLineItemOptions>(),
                    Mode = "payment",
                    SuccessUrl = $"{Request.Scheme}://{Request.Host}/Customer/Checkout/Success?orderId={order.Id}",
                    CancelUrl = $"{Request.Scheme}://{Request.Host}/Customer/Checkout/Cancel",
                };

                foreach (var item in carts)
                {
                    options.LineItems.Add(new SessionLineItemOptions
                    {
                        PriceData = new SessionLineItemPriceDataOptions
                        {
                            Currency = "egp",
                            ProductData = new SessionLineItemPriceDataProductDataOptions
                            {
                                Name = item.Product.Name,
                                Description = item.Product.Description,
                            },
                            UnitAmount = (long)item.Product.Price * 100,
                        },
                        Quantity = item.Count,
                    });
                }

                var service = new SessionService();
                var session = service.Create(options);

                order.SessionId = session.Id;
                await _orderRepository.CommitAsync();

                TempData["ValidatePage"] = "ComeFromPayAction";

                return Redirect(session.Url);
            }

            return RedirectToAction("NotFoundPage");

        }
    }
}
