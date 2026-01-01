using Application.DTOs.Requests;
using Application.DTOs.Responses;

namespace Application.Interfaces
{
    public interface ICartService
    {
        Task<CartResponse> GetMyCartAsync(string accountId);
        Task<CartResponse> AddItemAsync(string accountId, AddCartItemRequest req);
        Task<CartResponse> UpdateItemAsync(string accountId, string variationId, int quantity);
        Task<CartResponse> RemoveItemAsync(string accountId, string variationId);
        Task<bool> ClearAsync(string accountId);
    }
}
