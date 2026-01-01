using Application.DTOs.Requests;
using Application.DTOs.Responses;
using Application.Interfaces;
using Domain.Constants;
using Domain.Entities;
using Domain.Enums;
using Domain.Interfaces;
using System;

namespace Application.Implementations
{
    public class OrderService : IOrderService
    {
        private readonly IUnitOfWork _unitOfWork;

        public OrderService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<string> CreateOrderAsync(CreateOrderRequest request)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));
            if (request.Items == null || !request.Items.Any()) throw new ArgumentException("Order must contain at least one item.");

            decimal subtotal = request.Items.Sum(i => i.UnitPrice * i.Quantity);
            decimal discountAmount = 0m;

            // discount logic omitted intentionally; leave hooks here to query discount repository later
            var orderId = Prefixes.ORDER_ID_PREFIX + string.Format(Prefixes.ID_FORMAT, await _unitOfWork.OrderRepository.CountAsync() + 1);

            var order = new Order
            {
                OrderId = orderId,
                OrderStatus = EnumOrderStatus.Pending,
                SubTotal = subtotal,
                DiscountAmount = discountAmount,
                Total = subtotal - discountAmount,
                ShippingAddress = request.ShippingAddress,
                Note = request.Note ?? string.Empty,
                CreatedAt = DateOnly.FromDateTime(DateTime.Now),
                UpdatedAt = DateOnly.FromDateTime(DateTime.Now),
                CustomerId = request.CustomerId
            };

            foreach (var item in request.Items)
            {
                var od = new OrderDetail
                {
                    OrderDetailId = Prefixes.ORDER_ID_PREFIX + Guid.NewGuid().ToString(),
                    OrderId = orderId,
                    VariationId = item.VariationId,
                    UnitPrice = item.UnitPrice,
                    Quantity = item.Quantity,
                    DiscountAmount = 0m,
                    Total = item.UnitPrice * item.Quantity
                };
                order.OrderDetails.Add(od);
            }

            await _unitOfWork.ExecuteInTransactionAsync(async () =>
            {
                await _unitOfWork.OrderRepository.AddAsync(order);
            });

            return orderId;
        }

        public async Task<IEnumerable<OrderResponse>> GetAllAsync()
        {
            var orders = await _unitOfWork.OrderRepository.GetAllAsync(
                null,
                o => o.OrderDetails,
                o => o.Payments,
                o => o.DiscountUsages
            );

            return orders.Select(o => new OrderResponse
            {
                OrderId = o.OrderId,
                OrderStatus = (int)o.OrderStatus,
                SubTotal = o.SubTotal,
                DiscountAmount = o.DiscountAmount,
                Total = o.Total,
                ShippingAddress = o.ShippingAddress,
                Note = o.Note,
                CreatedAt = o.CreatedAt,
                UpdatedAt = o.UpdatedAt,
                CustomerId = o.CustomerId,
                Items = o.OrderDetails?.Select(od => new OrderItemResponse
                {
                    VariationId = od.VariationId,
                    Quantity = od.Quantity,
                    UnitPrice = od.UnitPrice,
                    DiscountAmount = od.DiscountAmount,
                    Total = od.Total
                }) ?? Array.Empty<OrderItemResponse>()
            });
        }

        public async Task<OrderResponse?> GetByIdAsync(string orderId)
        {
            var o = await _unitOfWork.OrderRepository.GetByIdWithDetailsAsync(orderId);
            if (o == null) return null;
            return new OrderResponse
            {
                OrderId = o.OrderId,
                OrderStatus = (int)o.OrderStatus,
                SubTotal = o.SubTotal,
                DiscountAmount = o.DiscountAmount,
                Total = o.Total,
                ShippingAddress = o.ShippingAddress,
                Note = o.Note,
                CreatedAt = o.CreatedAt,
                UpdatedAt = o.UpdatedAt,
                CustomerId = o.CustomerId,
                Items = o.OrderDetails?.Select(od => new OrderItemResponse
                {
                    VariationId = od.VariationId,
                    Quantity = od.Quantity,
                    UnitPrice = od.UnitPrice,
                    DiscountAmount = od.DiscountAmount,
                    Total = od.Total
                }) ?? Array.Empty<OrderItemResponse>()
            };
        }

        public async Task<bool> UpdateOrderAsync(UpdateOrderRequest request)
        {
            var order = await _unitOfWork.OrderRepository.GetByIdWithDetailsAsync(request.OrderId);
            if (order == null) return false;

            if (order.OrderStatus == EnumOrderStatus.Delivered || order.OrderStatus == EnumOrderStatus.Cancelled)
                throw new InvalidOperationException("Cannot update delivered or cancelled order.");

            await _unitOfWork.ExecuteInTransactionAsync(async () =>
            {
                order.ShippingAddress = request.ShippingAddress;
                order.Note = request.Note ?? order.Note;
                order.UpdatedAt = DateOnly.FromDateTime(DateTime.Now);

                if (request.Items != null && request.Items.Any())
                {
                    // remove current details and add new ones
                    // since OrderDetails are owned by Order, replace collection
                    order.OrderDetails.Clear();
                    decimal subtotal = 0m;
                    foreach (var item in request.Items)
                    {
                        var od = new OrderDetail
                        {
                            OrderDetailId = Prefixes.ORDER_ID_PREFIX + Guid.NewGuid().ToString(),
                            OrderId = order.OrderId,
                            VariationId = item.VariationId,
                            UnitPrice = item.UnitPrice,
                            Quantity = item.Quantity,
                            DiscountAmount = 0m,
                            Total = item.UnitPrice * item.Quantity
                        };
                        order.OrderDetails.Add(od);
                        subtotal += od.Total;
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
            if (string.IsNullOrWhiteSpace(orderId)) throw new ArgumentException(nameof(orderId));
            var order = await _unitOfWork.OrderRepository.GetByIdWithDetailsAsync(orderId);
            if (order == null) throw new InvalidOperationException("Order not found");
            if (order.OrderStatus != EnumOrderStatus.Pending) throw new InvalidOperationException("Only pending orders can be confirmed");

            await _unitOfWork.ExecuteInTransactionAsync(async () =>
            {
                // check and deduct stock from variations
                foreach (var od in order.OrderDetails)
                {
                    var variation = await _unitOfWork.ProductVariationRepository.GetAsync(v => v.VariationId == od.VariationId);
                    if (variation == null) throw new InvalidOperationException($"Variation not found: {od.VariationId}");
                    if (variation.StockQuantity < od.Quantity) throw new InvalidOperationException($"Insufficient stock for variation {variation.VariationId}");
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
            if (string.IsNullOrWhiteSpace(orderId)) throw new ArgumentException(nameof(orderId));

            await _unitOfWork.ExecuteInTransactionAsync(async () =>
            {
                var order = await _unitOfWork.OrderRepository.GetAsync(o => o.OrderId == orderId);
                if (order == null) throw new InvalidOperationException("Order not found");
                if (order.OrderStatus != EnumOrderStatus.Shipped) throw new InvalidOperationException("Order must be in shipped state to mark delivered");
                order.OrderStatus = EnumOrderStatus.Delivered;
                order.UpdatedAt = DateOnly.FromDateTime(DateTime.Now);
                _unitOfWork.OrderRepository.Update(order);
            });
        }

        public async Task CancelOrderAsync(string orderId)
        {
            if (string.IsNullOrWhiteSpace(orderId)) throw new ArgumentException(nameof(orderId));
            var order = await _unitOfWork.OrderRepository.GetByIdWithDetailsAsync(orderId);
            if (order == null) throw new InvalidOperationException("Order not found");
            if (order.OrderStatus == EnumOrderStatus.Delivered) throw new InvalidOperationException("Cannot cancel delivered order");

            await _unitOfWork.ExecuteInTransactionAsync(async () =>
            {
                // restore stock if it was already deducted
                if (order.OrderStatus == EnumOrderStatus.Shipped || order.OrderStatus == EnumOrderStatus.Pending)
                {
                    foreach (var od in order.OrderDetails)
                    {
                        var variation = await _unitOfWork.ProductVariationRepository.GetAsync(v => v.VariationId == od.VariationId);
                        if (variation != null)
                        {
                            variation.StockQuantity += od.Quantity;
                            _unitOfWork.ProductVariationRepository.Update(variation);
                        }
                    }
                }

                order.OrderStatus = EnumOrderStatus.Cancelled;
                order.UpdatedAt = DateOnly.FromDateTime(DateTime.Now);
                _unitOfWork.OrderRepository.Update(order);
            });
        }
    }
}
