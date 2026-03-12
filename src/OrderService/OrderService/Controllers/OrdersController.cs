using Microsoft.AspNetCore.Mvc;
using OrderService.Domain.Aggregates;
using OrderService.Domain.Entities;
using OrderService.Domain.Enums;
using OrderService.Domain.ValueObjects;
using AppId = OrderService.Domain.ValueObjects.ApplicationId;

namespace OrderService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrdersController : ControllerBase
{
    // In-memory store — replace with a real repository when persistence is added
    private static readonly Dictionary<Guid, Order> _store = [];

    // GET api/orders
    [HttpGet]
    public IActionResult GetAll() =>
        Ok(_store.Values.Select(MapToResponse));

    // GET api/orders/{id}
    [HttpGet("{id:guid}")]
    public IActionResult GetById(Guid id)
    {
        if (!_store.TryGetValue(id, out var order))
            return NotFound();

        return Ok(MapToResponse(order));
    }

    // POST api/orders
    [HttpPost]
    public IActionResult Create([FromBody] CreateOrderRequest request)
    {
        var customerId = CustomerId.From(request.CustomerId);
        var billingAddress = BillingAddress.Of(
            request.BillingAddress.Street,
            request.BillingAddress.City,
            request.BillingAddress.PostalCode,
            request.BillingAddress.Country);

        var orderLines = request.OrderLines.Select(l => OrderLine.Create(
            AppId.From(l.ApplicationId),
            l.ApplicationName,
            PlanId.From(l.PlanId),
            l.PlanName,
            l.BillingCycle,
            Money.Of(l.Price, l.Currency)));

        CouponCode? couponCode = request.CouponCode is not null
            ? CouponCode.From(request.CouponCode)
            : null;

        var order = Order.Place(customerId, request.OrderType, billingAddress, orderLines, couponCode);
        _store[order.Id.Value] = order;

        return CreatedAtAction(nameof(GetById), new { id = order.Id.Value }, MapToResponse(order));
    }

    // PUT api/orders/{id}/confirm
    [HttpPut("{id:guid}/confirm")]
    public IActionResult Confirm(Guid id, [FromBody] ConfirmOrderRequest request)
    {
        if (!_store.TryGetValue(id, out var order))
            return NotFound();

        order.Confirm(PaymentReference.From(request.PaymentReference));
        return Ok(MapToResponse(order));
    }

    // PUT api/orders/{id}/fail
    [HttpPut("{id:guid}/fail")]
    public IActionResult Fail(Guid id, [FromBody] FailOrderRequest request)
    {
        if (!_store.TryGetValue(id, out var order))
            return NotFound();

        order.Fail(request.Reason);
        return Ok(MapToResponse(order));
    }

    // DELETE api/orders/{id}
    [HttpDelete("{id:guid}")]
    public IActionResult Delete(Guid id)
    {
        if (!_store.Remove(id))
            return NotFound();

        return NoContent();
    }

    private static object MapToResponse(Order o) => new
    {
        Id = o.Id.Value,
        CustomerId = o.CustomerId.Value,
        o.OrderType,
        o.Status,
        BillingAddress = new
        {
            o.BillingAddress.Street,
            o.BillingAddress.City,
            o.BillingAddress.PostalCode,
            o.BillingAddress.Country
        },
        OrderLines = o.OrderLines.Select(l => new
        {
            l.Id,
            ApplicationId = l.ApplicationId.Value,
            l.ApplicationName,
            PlanId = l.PlanId.Value,
            l.PlanName,
            l.BillingCycle,
            Price = l.Price.Amount,
            l.Price.Currency
        }),
        CouponCode = o.CouponCode?.Value,
        TotalAmount = o.TotalAmount.Amount,
        Currency = o.TotalAmount.Currency,
        PaymentReference = o.PaymentReference?.Value,
        o.CreatedAt
    };
}

// ── Request DTOs ──────────────────────────────────────────────────────────────

public record CreateOrderRequest(
    Guid CustomerId,
    OrderType OrderType,
    BillingAddressDto BillingAddress,
    List<OrderLineDto> OrderLines,
    string? CouponCode);

public record BillingAddressDto(string Street, string City, string PostalCode, string Country);

public record OrderLineDto(
    Guid ApplicationId,
    string ApplicationName,
    Guid PlanId,
    string PlanName,
    string BillingCycle,
    decimal Price,
    string Currency);

public record ConfirmOrderRequest(string PaymentReference);

public record FailOrderRequest(string Reason);
