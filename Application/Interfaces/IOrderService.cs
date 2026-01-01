using Application.DTOs.Requests;
using Application.DTOs.Responses;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IOrderService
    {
        Task<string> CreateOrderAsync(CreateOrderRequest request);
        Task<IEnumerable<OrderResponse>> GetAllAsync();
        Task<OrderResponse?> GetByIdAsync(string orderId);
        Task<bool> UpdateOrderAsync(UpdateOrderRequest request);
        Task<bool> DeleteOrderAsync(string orderId);

        Task ConfirmOrderAsync(string orderId);

        /// <summary>
        /// Mark an order as delivered (transition shipped -> delivered).
        /// </summary>
        /// <param name="orderId">Order identifier.</param>
        Task ShipOrderAsync(string orderId);

        /// <summary>
        /// Cancel an order. If stock was deducted, restores stock where appropriate.
        /// </summary>
        /// <param name="orderId">Order identifier.</param>
        Task CancelOrderAsync(string orderId);
    }
}
