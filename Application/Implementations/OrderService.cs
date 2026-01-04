using Application.DTOs.Requests;
using Application.DTOs.Responses;
using Application.Interfaces;
using Domain.Constants;
using Domain.Entities;
using Domain.Enums;
using Domain.Interfaces;
using System;
using System.Linq;

namespace Application.Implementations
{
    public class OrderService : IOrderService
    {
        private readonly IUnitOfWork _unitOfWork;

        public OrderService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        private decimal CalculateDiscountAmount(Discount discount, decimal subtotal)
        {
            if (subtotal < discount.MinOrderValue)
                return 0m;

            decimal discountAmount = discount.DiscountType switch
            {
                EnumDiscountType.Percentage => subtotal * (discount.DiscountValue / 100m),
                EnumDiscountType.FixedAmount => discount.DiscountValue,
                EnumDiscountType.Freeshipping => 0m,
                _ => 0m
            };

            if (discount.MaxDiscountAmount > 0 && discountAmount > discount.MaxDiscountAmount)
                discountAmount = discount.MaxDiscountAmount;

            return discountAmount > subtotal ? subtotal : discountAmount;
        }

        public async Task<string> CreateOrderAsync(CreateOrderRequest request)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));
            if (!request.Items.Any())
                throw new ArgumentException("Order must contain at least one item.");

            decimal subtotal = 0m;
            decimal discountAmount = 0m;

            var orderId = Prefixes.ORDER_ID_PREFIX +
                          string.Format(Prefixes.ID_FORMAT,
                          await _unitOfWork.OrderRepository.CountAsync() + 1);

            var order = new Order
            {
                OrderId = orderId,
                OrderStatus = EnumOrderStatus.Pending,
                ShippingAddress = request.ShippingAddress,
                ReceiverName = request.ReceiverName,
                ReceiverPhone = request.ReceiverPhone,
                Note = request.Note ?? string.Empty,
                CreatedAt = DateOnly.FromDateTime(DateTime.Now),
                UpdatedAt = DateOnly.FromDateTime(DateTime.Now),
                CustomerId = request.CustomerId
            };

            await _unitOfWork.ExecuteInTransactionAsync(async () =>
            {
                foreach (var item in request.Items)
                {
                    var variation = await _unitOfWork.ProductVariationRepository
                        .GetAsync(v => v.VariationId == item.VariationId)
                        ?? throw new InvalidOperationException($"Variation not found: {item.VariationId}");

                    if (variation.StockQuantity < item.Quantity)
                        throw new InvalidOperationException("Insufficient stock");

                    var unitPrice = variation.Price;
                    var lineTotal = unitPrice * item.Quantity;

                    subtotal += lineTotal;

                    order.OrderDetails.Add(new OrderDetail
                    {
                        OrderDetailId = Prefixes.ORDER_ID_PREFIX + Guid.NewGuid(),
                        OrderId = orderId,
                        VariationId = item.VariationId,
                        Quantity = item.Quantity,
                        UnitPrice = unitPrice,
                        DiscountAmount = 0m,
                        Total = lineTotal
                    });
                }

                if (!string.IsNullOrWhiteSpace(request.DiscountId))
                {
                    var discount = await _unitOfWork.DiscountRepository
                        .GetAsync(d => d.DiscountId == request.DiscountId && d.IsActive)
                        ?? throw new InvalidOperationException("Discount not found or inactive");

                    if (DateTime.Now < discount.StartDate || DateTime.Now > discount.ExpireDate)
                        throw new InvalidOperationException("Discount expired");

                    if (discount.UsageLimit > 0 && discount.UsageCount >= discount.UsageLimit)
                        throw new InvalidOperationException("Discount usage limit reached");

                    discountAmount = CalculateDiscountAmount(discount, subtotal);

                    discount.UsageCount++;
                    _unitOfWork.DiscountRepository.Update(discount);

                    order.DiscountUsages.Add(new DiscountUsage
                    {
                        DiscountId = discount.DiscountId,
                        OrderId = orderId,
                        UsedAt = DateTime.Now
                    });
                }

                order.SubTotal = subtotal;
                order.DiscountAmount = discountAmount;
                order.Total = subtotal - discountAmount;

                await _unitOfWork.OrderRepository.AddAsync(order);
            });

            return orderId;
        }

        
        public async Task<IEnumerable<OrderResponse>> GetAllAsync()
        {
            var orders = await _unitOfWork.OrderRepository.GetAllAsync(
                null,
                o => o.OrderDetails
            );

            return orders.Select(o => MapToResponse(o));
        }

        
        public async Task<OrderResponse?> GetByIdAsync(string orderId)
        {
            var order = await _unitOfWork.OrderRepository.GetByIdWithDetailsAsync(orderId);
            return order == null ? null : MapToResponse(order);
        }

        
        public async Task<bool> UpdateOrderAsync(UpdateOrderRequest request)
        {
            var order = await _unitOfWork.OrderRepository.GetByIdWithDetailsAsync(request.OrderId);
            if (order == null) return false;

            if (order.OrderStatus is EnumOrderStatus.Delivered or EnumOrderStatus.Cancelled)
                throw new InvalidOperationException("Cannot update delivered or cancelled order.");

            await _unitOfWork.ExecuteInTransactionAsync(async () =>
            {
                order.ShippingAddress = request.ShippingAddress;
                order.Note = request.Note ?? order.Note;
                order.UpdatedAt = DateOnly.FromDateTime(DateTime.Now);

                if (request.Items.Any())
                {
                    order.OrderDetails.Clear();
                    decimal subtotal = 0m;

                    foreach (var item in request.Items)
                    {
                        var variation = await _unitOfWork.ProductVariationRepository
                            .GetAsync(v => v.VariationId == item.VariationId)
                            ?? throw new InvalidOperationException("Variation not found");

                        var unitPrice = variation.Price;
                        var total = unitPrice * item.Quantity;

                        subtotal += total;

                        order.OrderDetails.Add(new OrderDetail
                        {
                            OrderDetailId = Prefixes.ORDER_ID_PREFIX + Guid.NewGuid(),
                            OrderId = order.OrderId,
                            VariationId = item.VariationId,
                            Quantity = item.Quantity,
                            UnitPrice = unitPrice,
                            DiscountAmount = 0m,
                            Total = total
                        });
                    }

                    order.SubTotal = subtotal;
                    order.Total = subtotal - order.DiscountAmount;
                }

                _unitOfWork.OrderRepository.Update(order);
            });

            return true;
        }

        
        public async Task<bool> DeleteOrderAsync(string orderId)
        {
            var order = await _unitOfWork.OrderRepository.GetAsync(o => o.OrderId == orderId);
            if (order == null) return false;

            await _unitOfWork.ExecuteInTransactionAsync(async () =>
            {
                _unitOfWork.OrderRepository.Delete(order);
            });

            return true;
        }

        
        public async Task ConfirmOrderAsync(string orderId)
        {
            var order = await _unitOfWork.OrderRepository.GetByIdWithDetailsAsync(orderId)
                ?? throw new InvalidOperationException("Order not found");

            if (order.OrderStatus != EnumOrderStatus.Pending)
                throw new InvalidOperationException("Only pending orders can be confirmed");

            await _unitOfWork.ExecuteInTransactionAsync(async () =>
            {
                foreach (var od in order.OrderDetails)
                {
                    var variation = await _unitOfWork.ProductVariationRepository
                        .GetAsync(v => v.VariationId == od.VariationId)
                        ?? throw new InvalidOperationException("Variation not found");

                    if (variation.StockQuantity < od.Quantity)
                        throw new InvalidOperationException("Insufficient stock");

                    variation.StockQuantity -= od.Quantity;
                    _unitOfWork.ProductVariationRepository.Update(variation);
                }

                order.OrderStatus = EnumOrderStatus.Shipped;
                order.UpdatedAt = DateOnly.FromDateTime(DateTime.Now);
                _unitOfWork.OrderRepository.Update(order);
            });
        }

        public async Task ShipOrderAsync(string orderId)
        {
            var order = await _unitOfWork.OrderRepository.GetAsync(o => o.OrderId == orderId)
                ?? throw new InvalidOperationException("Order not found");

            if (order.OrderStatus != EnumOrderStatus.Shipped)
                throw new InvalidOperationException("Invalid order state");

            order.OrderStatus = EnumOrderStatus.Delivered;
            order.UpdatedAt = DateOnly.FromDateTime(DateTime.Now);
            _unitOfWork.OrderRepository.Update(order);
        }

        public async Task CancelOrderAsync(string orderId)
        {
            var order = await _unitOfWork.OrderRepository.GetByIdWithDetailsAsync(orderId)
                ?? throw new InvalidOperationException("Order not found");

            if (order.OrderStatus == EnumOrderStatus.Delivered)
                throw new InvalidOperationException("Cannot cancel delivered order");

            await _unitOfWork.ExecuteInTransactionAsync(async () =>
            {
                foreach (var od in order.OrderDetails)
                {
                    var variation = await _unitOfWork.ProductVariationRepository
                        .GetAsync(v => v.VariationId == od.VariationId);

                    if (variation != null)
                    {
                        variation.StockQuantity += od.Quantity;
                        _unitOfWork.ProductVariationRepository.Update(variation);
                    }
                }

                order.OrderStatus = EnumOrderStatus.Cancelled;
                order.UpdatedAt = DateOnly.FromDateTime(DateTime.Now);
                _unitOfWork.OrderRepository.Update(order);
            });
        }

        public async Task<IEnumerable<OrderResponse>> GetByAccountIdAsync(string accountId)
        {
            if (string.IsNullOrWhiteSpace(accountId))
                throw new ArgumentException(nameof(accountId));

            var orders = await _unitOfWork.OrderRepository.GetByAccountIdAsync(accountId);

            return orders.Select(o => new OrderResponse
            {
                OrderId = o.OrderId,
                OrderStatus = o.OrderStatus,

                ShippingAddress = o.ShippingAddress,
                ReceiverName = o.ReceiverName,
                ReceiverPhone = o.ReceiverPhone,

                SubTotal = o.SubTotal,
                DiscountAmount = o.DiscountAmount,
                Total = o.Total,

                Note = o.Note,
                CreatedAt = o.CreatedAt,
                UpdatedAt = o.UpdatedAt,

                CustomerId = o.CustomerId,

                Items = o.OrderDetails.Select(od => new OrderItemResponse
                {
                    VariationId = od.VariationId,
                    ProductName = od.Variation.Product.ProductName,
                    Quantity = od.Quantity,
                    UnitPrice = od.UnitPrice,
                    DiscountAmount = od.DiscountAmount,
                    Total = od.Total
                }).ToList()
            });
        }


        private static OrderResponse MapToResponse(Order o) => new()
        {
            OrderId = o.OrderId,
            OrderStatus = o.OrderStatus,
            SubTotal = o.SubTotal,
            DiscountAmount = o.DiscountAmount,
            Total = o.Total,
            ShippingAddress = o.ShippingAddress,
            ReceiverName = o.ReceiverName,
            ReceiverPhone = o.ReceiverPhone,
            Note = o.Note,
            CreatedAt = o.CreatedAt,
            UpdatedAt = o.UpdatedAt,
            CustomerId = o.CustomerId,
            Items = o.OrderDetails
                .Select(od => new OrderItemResponse
                {
                    VariationId = od.VariationId,
                    ProductName = od.Variation.Product.ProductName,
                    Quantity = od.Quantity,
                    UnitPrice = od.UnitPrice,
                    DiscountAmount = od.DiscountAmount,
                    Total = od.Total
                })
                .ToList()
        };
    }
}
