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
        Task ShipOrderAsync(string orderId);
        Task CancelOrderAsync(string orderId);
    }
}
