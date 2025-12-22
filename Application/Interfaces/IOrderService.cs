using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IOrderService
    {
        /// <summary>
        /// Confirm an order (validate, deduct stock, mark as shipped/packaged).
        /// </summary>
        /// <param name="orderId">Order identifier.</param>
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
