using System;
using System.Collections.Generic;
using System.Linq;
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
            // Create Razorpay order
            var razorpayOrderId = await _paymentService.CreateRazorpayOrderAsync(request.OrderId, request.Amount);

            // Save payment record
            var payment = new Payment
            {
                // UserId = User.Identity.Name,
                UserId = "test-user", // Extracted from JWT
                OrderId = request.OrderId,
                Status = "Pending",
                TransactionId = razorpayOrderId
            };
            payment = await _repository.CreatePaymentAsync(payment);

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