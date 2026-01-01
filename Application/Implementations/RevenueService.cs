using Application.Interfaces;
using Domain.Enums;
using Domain.Interfaces;
using System.Linq;
using System.Threading.Tasks;

namespace Application.Implementations
{
    public class RevenueService : IRevenueService
    {
        private readonly IUnitOfWork _unitOfWork;

        public RevenueService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<decimal> GetTotalRevenueByDayAsync(DateOnly day)
        {
            var orders = await _unitOfWork.OrderRepository.GetAllAsync(o =>
                o.OrderStatus == EnumOrderStatus.Delivered &&
                o.CreatedAt == day);

            return orders.Sum(o => o.Total);
        }

        public async Task<decimal> GetTotalRevenueByMonthAsync(int year, int month)
        {
            var orders = await _unitOfWork.OrderRepository.GetAllAsync(o =>
                o.OrderStatus == EnumOrderStatus.Delivered &&
                o.CreatedAt.Year == year &&
                o.CreatedAt.Month == month);

            return orders.Sum(o => o.Total);
        }
    }
}
