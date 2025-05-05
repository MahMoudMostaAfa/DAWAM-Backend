using Dawam_backend.Data;
using Dawam_backend.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Stripe;
using Stripe.Checkout;

namespace Dawam_backend.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class PaymentsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _config;

        public PaymentsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IConfiguration config)
        {
            _context = context;
            _userManager = userManager;
            _config = config;
        }
        [Authorize(Roles = "JobApplier")]
        [HttpPost("create-checkout-session")]
        public async Task<IActionResult> CreateCheckoutSession()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return Unauthorized();

            // Get the fixed subscription plan
            var plan = await _context.SubscriptionPlans.FirstOrDefaultAsync();
            if (plan == null)
                return NotFound("Subscription plan not found");

            var domain = _config["FrontendDomain"]  ; // e.g., http://localhost:3000

            var options = new SessionCreateOptions
            {
                PaymentMethodTypes = new List<string> { "card" },
                LineItems = new List<SessionLineItemOptions>
            {
                new()
                {
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        UnitAmount = (long)(plan.Price * 100), // cents
                        Currency = "usd",
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = plan.Name
                        }
                    },
                    Quantity = 1
                }
            },
                Mode = "payment",
                SuccessUrl = $"{domain}?session_id={{CHECKOUT_SESSION_ID}}",
                CancelUrl = $"{domain}/",
                Metadata = new Dictionary<string, string>
            {
                { "userId", user.Id },
                { "subscriptionPlanId", plan.Id.ToString() }
            }
            };

            try
            {
                var service = new SessionService();
                var session = await service.CreateAsync(options);
                return Ok(new { url = session.Url });
            }
            catch (StripeException e)
            {
                return StatusCode(500, new { error = e.Message });
            }
            //var service = new SessionService();
            //var session = await service.CreateAsync(options);

            //return Ok(new { url = session.Url });
        }

        [AllowAnonymous]
        [HttpPost("webhook")]
        public async Task<IActionResult> StripeWebhook()
        {
            Console.WriteLine("Received webhook call");
            var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();
            Console.WriteLine($"Raw JSON: {json}");
            try
            {
                var stripeEvent = EventUtility.ConstructEvent(
                    json,
                    Request.Headers["Stripe-Signature"],
                    _config["Stripe:WebhookSecret"]
                );
                Console.WriteLine($"Stripe event type: {stripeEvent.Type}");
                if (stripeEvent.Type == "checkout.session.completed")
                {
                    var session = stripeEvent.Data.Object as Session;
                    var userId = session.Metadata["userId"];
                    var planId = int.Parse(session.Metadata["subscriptionPlanId"]);

                    var payment = new Payment
                    {
                        UserId = userId,
                        SubscriptionPlanId = planId,
                        StripePaymentIntentId = session.PaymentIntentId,
                        Amount = (decimal)(session.AmountTotal / 100.0),
                        Status = "Succeeded"
                    };

                    _context.Payments.Add(payment);
                    await _context.SaveChangesAsync();
                }

                return Ok();
            }
            catch
            {
                return BadRequest();
            }
        }

    }



}
