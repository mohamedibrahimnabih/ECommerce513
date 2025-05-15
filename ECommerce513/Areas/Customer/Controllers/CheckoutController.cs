using ECommerce513.Data;
using ECommerce513.Models;
using ECommerce513.Repository.IRepository;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Stripe.Checkout;
using System.Threading.Tasks;

namespace ECommerce513.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class CheckoutController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ICartRepository _cartRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly IOrderItemRepository _orderItemRepository;
        private readonly IEmailSender _emailSender;

        public CheckoutController(UserManager<ApplicationUser> userManager, ICartRepository cartRepository, IOrderRepository orderRepository, IOrderItemRepository orderItemRepository, IEmailSender emailSender)
        {
            _userManager = userManager;
            _cartRepository = cartRepository;
            _orderRepository = orderRepository;
            _orderItemRepository = orderItemRepository;
            _emailSender = emailSender;
        }

        public async Task<IActionResult> Success(int orderId)
        {
            if (TempData["ValidatePage"] is not null)
            {

                var applicationUser = await _userManager.GetUserAsync(User);

                var carts = await _cartRepository.GetAsync(e => e.ApplicationUserId == applicationUser.Id, includes: [e => e.Product]);

                if (applicationUser is not null && carts is not null)
                {

                    // 1. Transform Cart => Order items
                    List<OrderItems> orderItems = [];

                    foreach (var item in carts)
                    {
                        orderItems.Add(new()
                        {
                            OrderId = orderId,
                            ProductId = item.ProductId,
                            Count = item.Count,
                            Price = item.Product.Price
                        });
                    }

                    await _orderItemRepository.CreateRangeAsync(orderItems);

                    // 2. Decrement Quantity
                    foreach (var item in carts)
                    {
                        item.Product.Quantity -= item.Count;
                        await _cartRepository.CommitAsync();
                    }

                    // 3. Delete Old Cart
                    await _cartRepository.DeleteRangeAsync(carts);

                    // 4. Update Order Prop.
                    var order = _orderRepository.GetOne(e => e.Id == orderId);

                    var service = new SessionService();
                    var session = service.Get(order.SessionId);

                    order.SessionId = session.Id;

                    order.OrderStatus = OrderStatus.InProcessing;
                    order.TransactionStatus = true;
                    order.TransctionId = session.PaymentIntentId;

                    await _orderRepository.CommitAsync();

                    // 5. Send Email to user
                    await _emailSender.SendEmailAsync(applicationUser.Email, "Thanks", "Order Completed");


                    return View(orderId);
                }

                return BadRequest();
            }

            return BadRequest();
        }

        public IActionResult Cancel()
        {
            if (TempData["ValidatePage"] is not null)
            {
                return View();
            }

            return BadRequest();
        }
    }
}
