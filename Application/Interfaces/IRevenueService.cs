using System.Threading.Tasks;
using System;

namespace Application.Interfaces
{
    public interface IRevenueService
    {
        Task<decimal> GetTotalRevenueByDayAsync(DateOnly day);
        Task<decimal> GetTotalRevenueByMonthAsync(int year, int month);
    }
}
