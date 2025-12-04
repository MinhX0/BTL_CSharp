using backend.Entities.Store;
using backend.Repositories.Store;
using backend.Repositories.UserInfoRepositories;
using backend.Services.Store;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Security.Claims;

namespace backend;

[ApiController]
[Route("api/[controller]")]
public class CheckoutController : ControllerBase
{
    private class OrderInfo
    {
        public long OrderId { get; set; }
        public long Amount { get; set; }
        public string Status { get; set; } = "0";
        public DateTime CreatedDate { get; set; }
    }
    private readonly VnPayLibrary _vnPayLibrary = new VnPayLibrary();
    private readonly IOrderService _orderService;
    private readonly ICartService _cartService;
    private readonly IProductService _productService;
    private readonly ICustomerRepository _customerRepository;

    private readonly IConfiguration _configuration;

    public CheckoutController(
        ICartService cartService,
        IProductService productService,
        ICustomerRepository customerRepository,
        IOrderService orderService,
        IConfiguration configuration)
    {
        _customerRepository=customerRepository;
        _cartService = cartService;
        _productService = productService;
        _orderService = orderService;
        _configuration = configuration;
    }

    [HttpPost("vnpay-checkout")]
    public async Task<IActionResult> VnpayCheckout()
    {
        // Read settings from appsettings.json under Vnpay
        string vnp_Returnurl = _configuration["Vnpay:vnp_Returnurl"] ?? string.Empty; // URL nhận kết quả trả về
        string vnp_Url = _configuration["Vnpay:vnp_Url"] ?? string.Empty;            // URL thanh toán của VNPAY
        string vnp_TmnCode = _configuration["Vnpay:vnp_TmnCode"] ?? string.Empty;    // Terminal Id
        string vnp_HashSecret = _configuration["Vnpay:vnp_HashSecret"] ?? string.Empty; // Secret Key

        // Get current customer from claims
        var role = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
        var idStr = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrWhiteSpace(role) || !role.Equals("Customer", StringComparison.OrdinalIgnoreCase) || !int.TryParse(idStr, out var customerId))
        {
            return Unauthorized();
        }

        // Load cart
        var cartItems = await _cartService.GetCustomerCartAsync(customerId);
        if (cartItems == null || !cartItems.Any())
        {
            return BadRequest(new { message = "Giỏ hàng trống." });
        }

        // Load products to compute totals
        var products = await _productService.GetAllAsync();
        long total = 0;
        var orderDetails = new List<OrderDetail>();
        foreach (var item in cartItems)
        {
            var p = products.FirstOrDefault(pr => pr.ProductId == item.ProductId);
            if (p == null || !p.IsActive) continue;
            var unit = p.DiscountPrice.HasValue && p.DiscountPrice.Value > 0 ? p.DiscountPrice.Value : p.Price;
            if (item.Quantity > p.StockQuantity)
            {
                return BadRequest(new { message = $"Sản phẩm {p.ProductName} không đủ tồn kho." });
            }
            total += unit * item.Quantity;
            orderDetails.Add(new OrderDetail
            {
                ProductId = p.ProductId,
                Quantity = item.Quantity,
                Price = unit
            });
        }

        // Prepare payment info
        // Create order in DB with Pending status before redirecting to payment
        var orderEntity = new Order
        {
            CustomerId = customerId,
            ShippingAddress = (await _customerRepository.GetByIdAsync(customerId))?.Address ?? string.Empty,
            TotalAmount = total,
            PaymentMethod = "VnPay",
            Status = "Pending",
            OrderDate = DateTime.UtcNow
        };
        await _orderService.CreateAsync(orderEntity, orderDetails);

        // Prepare VNPay request data
        OrderInfo order = new OrderInfo
        {
            OrderId = orderEntity.OrderId,
            Amount = total,
            Status = "0",
            CreatedDate = DateTime.Now
        };
        //Save order to db

        //Build URL for VNPAY
        VnPayLibrary vnpay = new VnPayLibrary();

        vnpay.AddRequestData("vnp_Version", VnPayLibrary.VERSION);
        vnpay.AddRequestData("vnp_Command", "pay");
        vnpay.AddRequestData("vnp_TmnCode", vnp_TmnCode);
        vnpay.AddRequestData("vnp_Amount", (order.Amount * 100).ToString());

        vnpay.AddRequestData("vnp_CreateDate", order.CreatedDate.ToString("yyyyMMddHHmmss"));
        vnpay.AddRequestData("vnp_CurrCode", "VND");
        // Client IP
        var ip = HttpContext.Request.Headers["X-Forwarded-For"].FirstOrDefault()?.Split(',')[0].Trim();
        if (string.IsNullOrWhiteSpace(ip)) { ip = HttpContext.Connection.RemoteIpAddress?.ToString(); }
        vnpay.AddRequestData("vnp_IpAddr", ip ?? "127.0.0.1:5117");

        // Locale (default vn)
        vnpay.AddRequestData("vnp_Locale", "vn");
        vnpay.AddRequestData("vnp_OrderInfo", "Thanh toan don hang:" + order.OrderId);
        vnpay.AddRequestData("vnp_OrderType", "other"); //default value: other

        vnpay.AddRequestData("vnp_ReturnUrl", vnp_Returnurl);
        vnpay.AddRequestData("vnp_TxnRef", order.OrderId.ToString()); // Mã tham chiếu của giao dịch tại hệ thống của merchant. Mã này là duy nhất dùng để phân biệt các đơn hàng gửi sang VNPAY. Không được trùng lặp trong ngày

        //Add Params of 2.1.0 Version
        //Billing

        string paymentUrl = vnpay.CreateRequestUrl(vnp_Url, vnp_HashSecret);
        // Optionally clear cart here or after IPN confirmation. We'll clear after successful payment.
        // Redirect to VNPay payment URL
        return Redirect(paymentUrl);
    }
    [HttpGet("vnpay-ipn")]
    [HttpPost("vnpay-ipn")]
    public async Task<IActionResult> VnpayIpn()
    {
        // Collect response parameters from VNPAY
        var vnpay = new VnPayLibrary();
        // VNPay may call IPN with GET or POST
        bool isPost = HttpContext.Request.Method.Equals("POST", StringComparison.OrdinalIgnoreCase);
        var enumerable = isPost
            ? (IEnumerable<KeyValuePair<string, Microsoft.Extensions.Primitives.StringValues>>)Request.Form
            : (IEnumerable<KeyValuePair<string, Microsoft.Extensions.Primitives.StringValues>>)Request.Query;
        foreach (var kv in enumerable)
        {
            vnpay.AddResponseData(kv.Key, kv.Value);
        }

        string vnp_TxnRef = vnpay.GetResponseData("vnp_TxnRef");
        string vnp_TransactionNo = vnpay.GetResponseData("vnp_TransactionNo");
        string vnp_ResponseCode = vnpay.GetResponseData("vnp_ResponseCode");
        string vnp_SecureHash = isPost ? Request.Form["vnp_SecureHash"] : Request.Query["vnp_SecureHash"];
        string vnp_AmountStr = vnpay.GetResponseData("vnp_Amount");

        string secret = _configuration["Vnpay:vnp_HashSecret"] ?? string.Empty;
        bool validSignature = vnpay.ValidateSignature(vnp_SecureHash, secret);
        if (!validSignature)
        {
            return Ok(new { RspCode = "97", Message = "Invalid signature" });
        }

        if (!int.TryParse(vnp_TxnRef, out var orderId))
        {
            return Ok(new { RspCode = "01", Message = "Order not found" });
        }

        var order = await _orderService.GetByIdAsync(orderId);
        if (order == null)
        {
            return Ok(new { RspCode = "01", Message = "Order not found" });
        }

        if (long.TryParse(vnp_AmountStr, out var vnpAmountMinor))
        {
            var expectedMinor = order.TotalAmount * 100; // VNPay sends amount x100
            if (vnpAmountMinor != expectedMinor)
            {
                return Ok(new { RspCode = "02", Message = "Invalid amount" });
            }
        }

        if (vnp_ResponseCode == "00")
        {
            // Payment success
            await _orderService.UpdateStatusAsync(orderId, "Paid");
            // Clear cart for the customer associated with the order
            if (order.CustomerId > 0)
            {
                await _cartService.ClearCartAsync(order.CustomerId);
            }
            return Ok(new { RspCode = "00", Message = "Confirm Success", TransactionNo = vnp_TransactionNo });
        }
        else
        {
            // Payment failed or canceled
            await _orderService.UpdateStatusAsync(orderId, "Failed");
            return Ok(new { RspCode = vnp_ResponseCode, Message = "Payment not successful" });
        }
    }
}