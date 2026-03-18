using MediatR;
using Microsoft.AspNetCore.Mvc;
using Orders.Application.Commands.ConfirmOrder;
using Orders.Application.Commands.FailOrder;
using Orders.Application.Commands.PlaceOrder;
using Orders.Application.Queries.GetCustomerOrders;
using Orders.Application.Queries.GetOrder;

namespace Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrdersController : ControllerBase
{
    private readonly IMediator _mediator;

    public OrdersController(IMediator mediator) => _mediator = mediator;

    // GET api/orders/{id}
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
    {
        var order = await _mediator.Send(new GetOrderQuery(id), ct);
        return order is null ? NotFound() : Ok(order);
    }

    // GET api/orders?customerId={customerId}
    [HttpGet]
    public async Task<IActionResult> GetByCustomer([FromQuery] Guid customerId, CancellationToken ct)
    {
        var orders = await _mediator.Send(new GetCustomerOrdersQuery(customerId), ct);
        return Ok(orders);
    }

    // POST api/orders
    [HttpPost]
    public async Task<IActionResult> Place([FromBody] PlaceOrderRequest request, CancellationToken ct)
    {
        var orderId = await _mediator.Send(new PlaceOrderCommand(
            request.CustomerId,
            new PlaceOrderBillingAddress(
                request.BillingAddress.Street,
                request.BillingAddress.City,
                request.BillingAddress.PostalCode,
                request.BillingAddress.Country),
            request.OrderLines.Select(l => new PlaceOrderLine(
                l.ProductId, l.ProductName,
                l.Quantity, l.Price, l.Currency)).ToList(),
            request.CouponCode), ct);

        return CreatedAtAction(nameof(GetById), new { id = orderId }, new { id = orderId });
    }

    // PUT api/orders/{id}/confirm
    [HttpPut("{id:guid}/confirm")]
    public async Task<IActionResult> Confirm(Guid id, [FromBody] ConfirmRequest request, CancellationToken ct)
    {
        await _mediator.Send(new ConfirmOrderCommand(id, request.PaymentReference), ct);
        return NoContent();
    }

    // PUT api/orders/{id}/fail
    [HttpPut("{id:guid}/fail")]
    public async Task<IActionResult> Fail(Guid id, [FromBody] FailRequest request, CancellationToken ct)
    {
        await _mediator.Send(new FailOrderCommand(id, request.Reason), ct);
        return NoContent();
    }
}

// ── Request DTOs ─────────────────────────────────────────────────────────────

public record PlaceOrderRequest(
    Guid CustomerId,
    PlaceOrderAddressRequest BillingAddress,
    List<PlaceOrderLineRequest> OrderLines,
    string? CouponCode);

public record PlaceOrderAddressRequest(string Street, string City, string PostalCode, string Country);

public record PlaceOrderLineRequest(
    Guid ProductId, string ProductName,
    int Quantity, decimal Price, string Currency);

public record ConfirmRequest(string PaymentReference);
public record FailRequest(string Reason);
