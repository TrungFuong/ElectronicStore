using Application.DTOs.Requests;
using Application.DTOs.Responses;
using Application.Interfaces;
using Domain.Entities;
using Domain.Interfaces;

namespace Application.Implementations
{
    public class CartService : ICartService
    {
        private readonly IUnitOfWork _uow;

        public CartService(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<CartResponse> GetMyCartAsync(string accountId)
        {
            var cart = await GetOrCreateCart(accountId);
            return await BuildResponse(cart.CartId, accountId);
        }

        public async Task<CartResponse> AddItemAsync(string accountId, AddCartItemRequest req)
        {
            if (req.Quantity < 1) throw new Exception("Số lượng phải >= 1.");

            var variation = await _uow.ProductVariationRepository.GetByIdAsync(req.VariationId);
            if (variation == null || !variation.IsActive)
                throw new Exception("Variation không tồn tại hoặc đã bị vô hiệu hóa.");

            if (req.Quantity > variation.StockQuantity)
                throw new Exception("Số lượng vượt quá tồn kho.");

            var cart = await GetOrCreateCart(accountId);

            var existing = await _uow.CartItemRepository.GetByCartAndVariationAsync(cart.CartId, req.VariationId);

            if (existing == null)
            {
                await _uow.CartItemRepository.AddAsync(new CartItem
                {
                    CartId = cart.CartId,
                    VariationId = req.VariationId,
                    Quantity = req.Quantity,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                });
            }
            else
            {
                var newQty = existing.Quantity + req.Quantity;
                if (newQty > variation.StockQuantity) throw new Exception("Số lượng vượt quá tồn kho.");

                existing.Quantity = newQty;
                existing.UpdatedAt = DateTime.UtcNow;
                _uow.CartItemRepository.Update(existing);
            }

            cart.UpdatedAt = DateTime.UtcNow;
            _uow.CartRepository.Update(cart);

            await _uow.CommitAsync();
            return await BuildResponse(cart.CartId, accountId);
        }

        public async Task<CartResponse> UpdateItemAsync(string accountId, string variationId, int quantity)
        {
            if (quantity < 1) throw new Exception("Số lượng phải >= 1.");

            var cart = await GetOrCreateCart(accountId);

            var item = await _uow.CartItemRepository.GetByCartAndVariationAsync(cart.CartId, variationId);
            if (item == null) throw new Exception("Item không tồn tại trong giỏ.");

            var variation = await _uow.ProductVariationRepository.GetByIdAsync(variationId);
            if (variation == null || !variation.IsActive)
                throw new Exception("Variation không tồn tại hoặc đã bị vô hiệu hóa.");

            if (quantity > variation.StockQuantity) throw new Exception("Số lượng vượt quá tồn kho.");

            item.Quantity = quantity;
            item.UpdatedAt = DateTime.UtcNow;
            _uow.CartItemRepository.Update(item);

            cart.UpdatedAt = DateTime.UtcNow;
            _uow.CartRepository.Update(cart);

            await _uow.CommitAsync();
            return await BuildResponse(cart.CartId, accountId);
        }

        public async Task<CartResponse> RemoveItemAsync(string accountId, string variationId)
        {
            var cart = await GetOrCreateCart(accountId);

            var item = await _uow.CartItemRepository.GetByCartAndVariationAsync(cart.CartId, variationId);
            if (item == null) throw new Exception("Item không tồn tại trong giỏ.");

            _uow.CartItemRepository.Remove(item);

            cart.UpdatedAt = DateTime.UtcNow;
            _uow.CartRepository.Update(cart);

            await _uow.CommitAsync();
            return await BuildResponse(cart.CartId, accountId);
        }

        public async Task<bool> ClearAsync(string accountId)
        {
            var cart = await _uow.CartRepository.GetByAccountIdAsync(accountId);
            if (cart == null) return true;

            var items = await _uow.CartItemRepository.GetByCartIdAsync(cart.CartId, includeVariation: false);
            _uow.CartItemRepository.RemoveRange(items);

            cart.UpdatedAt = DateTime.UtcNow;
            _uow.CartRepository.Update(cart);

            await _uow.CommitAsync();
            return true;
        }

        private async Task<Cart> GetOrCreateCart(string accountId)
        {
            var cart = await _uow.CartRepository.GetByAccountIdAsync(accountId);
            if (cart != null) return cart;

            var accountExists = await _uow.AccountRepository.GetByIdAsync(accountId);
            if (accountExists == null) throw new Exception("Account không tồn tại.");

            cart = new Cart
            {
                AccountId = accountId,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _uow.CartRepository.AddAsync(cart);
            await _uow.CommitAsync();
            return cart;
        }

        private async Task<CartResponse> BuildResponse(string cartId, string accountId)
        {
            var items = await _uow.CartItemRepository.GetByCartIdAsync(cartId, includeVariation: true);

            var mapped = items.Select(ci => new CartItemResponse
            {
                CartItemId = ci.CartItemId,
                VariationId = ci.VariationId,
                ProductId = ci.Variation!.ProductId,
                Price = ci.Variation.Price,
                Quantity = ci.Quantity,
                StockQuantity = ci.Variation.StockQuantity,
                IsActive = ci.Variation.IsActive
            }).ToList();

            return new CartResponse
            {
                CartId = cartId,
                AccountId = accountId,
                TotalQuantity = mapped.Sum(x => x.Quantity),
                SubTotal = mapped.Sum(x => x.Price * x.Quantity),
                Items = mapped
            };
        }
    }
}
