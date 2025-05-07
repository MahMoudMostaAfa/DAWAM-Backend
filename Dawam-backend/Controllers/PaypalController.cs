using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PayPalCheckoutSdk.Orders;
using Dawam_backend.Models;
using Dawam_backend.DTOs;
using Dawam_backend.Extensions;
using Dawam_backend.Data;

namespace Dawam_backend.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class PayPalController(
    ApplicationDbContext context,
    UserManager<ApplicationUser> userManager) : ControllerBase
{
    [HttpPost("create-payment")]
    public async Task<IActionResult> CreatePayment()
    {
        var user = await userManager.GetUserAsync(User);
        if (user == null) return Unauthorized();

        var plan = await context.SubscriptionPlans.FirstOrDefaultAsync();
        if (plan == null) return BadRequest("No subscription plan available");

        var orderRequest = new OrdersCreateRequest();
        orderRequest.Prefer("return=representation");
        orderRequest.RequestBody(new OrderRequest
        {
            CheckoutPaymentIntent = "CAPTURE",

            PurchaseUnits =
            [
                new PurchaseUnitRequest
                {
                    AmountWithBreakdown = new AmountWithBreakdown
                    {
                        CurrencyCode = "USD",
                        Value = plan.Price.ToString("F2")
                    }
                }
            ],
            ApplicationContext = new ApplicationContext
            {
                ReturnUrl = "http://localhost:3000/payment/success",
                CancelUrl = "http://localhost:3000/payment/cancel"
            }
        });

        try
        {
            var response = await PayPalClient.GetClient().Execute(orderRequest);
            var order = response.Result<Order>();
            var approveLink = order.Links.First(link => link.Rel == "approve");



            return Ok(new { ApprovalUrl = approveLink.Href });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { Error = ex.Message });
        }
    }

    [HttpPost("execute-payment")]
    public async Task<IActionResult> ExecutePayment([FromBody] ExecutePaymentDto dto)
    {
        var orderCaptureRequest = new OrdersCaptureRequest(dto.OrderId);
        Console.WriteLine($"Order ID: {orderCaptureRequest.Body}");
        orderCaptureRequest.RequestBody(new OrderActionRequest());

        try
        {
            var response = await PayPalClient.GetClient().Execute(orderCaptureRequest);
            var capturedOrder = response.Result<Order>();
            Console.WriteLine($"Captured Order: {capturedOrder.Status}");
            if (capturedOrder.Status != "COMPLETED")
                return BadRequest(new { Error = "Payment failed" });

            var user = await userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();

            var plan = await context.SubscriptionPlans.FirstAsync();

            var payment = new Payment
            {
                UserId = user.Id,
                SubscriptionPlanId = plan.Id,
                StripePaymentIntentId = capturedOrder.Id,
                Amount = plan.Price,
                PaymentDate = DateTime.UtcNow,
                Status = "Completed"
            };

            user.SubscriptionPlanId = plan.Id;

            await context.Payments.AddAsync(payment);
            await context.SaveChangesAsync();

            return Ok(new { Status = "success" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { Error = ex.Message });
        }

    }
}