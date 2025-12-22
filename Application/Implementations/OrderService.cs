using Application.Interfaces;
using Domain.Entities;
using Domain.Enums;
using Domain.Interfaces;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Application.Implementations
{
    public class OrderService : IOrderService
    {
        private readonly IUnitOfWork _unitOfWork;

        public OrderService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task ConfirmOrderAsync(string orderId)
        {
            if (string.IsNullOrWhiteSpace(orderId)) throw new ArgumentException(nameof(orderId));
            var order = await _unitOfWork.OrderRepository.GetByIdWithDetailsAsync(orderId);
            if (order == null) throw new InvalidOperationException("Order not found");
            if (order.OrderStatus != EnumOrderStatus.Pending) throw new InvalidOperationException("Only pending orders can be confirmed");

            foreach (var od in order.OrderDetails)
            {
                var product = await _unitOfWork.ProductRepository.GetByIdAsync(od.ProductId);
                if (product == null) throw new InvalidOperationException($"Product not found: {od.ProductId}");
                if (product.StockQuantity < od.Quantity) throw new InvalidOperationException($"Insufficient stock for product {product.ProductName}");
                product.StockQuantity -= od.Quantity;
                product.UpdatedAt = DateOnly.FromDateTime(DateTime.Now);
                _unitOfWork.ProductRepository.Update(product);
            }

            order.OrderStatus = EnumOrderStatus.Shipped;
            order.UpdatedAt = DateOnly.FromDateTime(DateTime.Now);
            _unitOfWork.OrderRepository.Update(order);
            await _unitOfWork.CommitAsync();
        }

        public async Task ShipOrderAsync(string orderId)
        {
            if (string.IsNullOrWhiteSpace(orderId)) throw new ArgumentException(nameof(orderId));
            var order = await _unitOfWork.OrderRepository.GetAsync(o => o.OrderId == orderId);
            if (order == null) throw new InvalidOperationException("Order not found");
            if (order.OrderStatus != EnumOrderStatus.Shipped) throw new InvalidOperationException("Order must be in shipped state to mark delivered");
            order.OrderStatus = EnumOrderStatus.Delivered;
            order.UpdatedAt = DateOnly.FromDateTime(DateTime.Now);
            _unitOfWork.OrderRepository.Update(order);
            await _unitOfWork.CommitAsync();
        }

        public async Task CancelOrderAsync(string orderId)
        {
            if (string.IsNullOrWhiteSpace(orderId)) throw new ArgumentException(nameof(orderId));
            var order = await _unitOfWork.OrderRepository.GetByIdWithDetailsAsync(orderId);
            if (order == null) throw new InvalidOperationException("Order not found");
            if (order.OrderStatus == EnumOrderStatus.Delivered) throw new InvalidOperationException("Cannot cancel delivered order");

            // restore stock if it was already deducted (we deducted when confirming)
            if (order.OrderStatus == EnumOrderStatus.Shipped || order.OrderStatus == EnumOrderStatus.Pending)
            {
                foreach (var od in order.OrderDetails)
                {
                    var product = await _unitOfWork.ProductRepository.GetByIdAsync(od.ProductId);
                    if (product != null)
                    {
                        product.StockQuantity += od.Quantity;
                        product.UpdatedAt = DateOnly.FromDateTime(DateTime.Now);
                        _unitOfWork.ProductRepository.Update(product);
                    }
                }
            }

            order.OrderStatus = EnumOrderStatus.Cancelled;
            order.UpdatedAt = DateOnly.FromDateTime(DateTime.Now);
            _unitOfWork.OrderRepository.Update(order);
            await _unitOfWork.CommitAsync();
        }
    }
}
