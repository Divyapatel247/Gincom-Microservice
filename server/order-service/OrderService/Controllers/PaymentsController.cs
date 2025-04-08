using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using OrderService.Dtos.Requests;
using OrderService.Interfaces;
using OrderService.Mappers;
using OrderService.Models;

namespace OrderService.Controllers
{
    [Route("api/orders/payments")]
    [ApiController]
    [Tags("Payments")]
    public class PaymentsController : ControllerBase
    {
        private readonly IOrderRepository _repository;
        private readonly IPaymentService _paymentService;

        public PaymentsController(IOrderRepository repository, IPaymentService paymentService)
        {
            _repository = repository;
            _paymentService = paymentService;
        }

        [HttpPost]
        public async Task<IActionResult> ProcessPayment([FromBody] ProcessPaymentRequestDto request)
        {
            var order = await _repository.GetOrderByIdAsync(request.OrderId);
            if (order == null || order.Status != "Pending")
                return BadRequest("Invalid or non-pending order");

            // This is now optional; Razorpay is initiated in CreateOrder
            var razorpayOrderId = await _paymentService.CreateRazorpayOrderAsync(request.OrderId, request.Amount);

            var payment = await _repository.GetPaymentByOrderIdAsync(request.OrderId);
            if (payment == null)
            {
                payment = new Payment
                {
                    UserId = "test-user", // Extracted from JWT in production
                    OrderId = request.OrderId,
                    Status = "Pending", // Admin will change
                    TransactionId = razorpayOrderId
                };
                await _repository.CreatePaymentAsync(payment);
            }
            else
            {
                payment.TransactionId = razorpayOrderId;
                await _repository.UpdatePaymentAsync(payment);
            }

            return Ok(new { RazorpayOrderId = razorpayOrderId });
        }

        [HttpGet("{orderId}")]
        public async Task<IActionResult> GetPayment(int orderId)
        {
            var payment = await _repository.GetPaymentByOrderIdAsync(orderId);
            if (payment == null) return NotFound();
            return Ok(PaymentMapper.ToPaymentResponse(payment));
        }
    }
}