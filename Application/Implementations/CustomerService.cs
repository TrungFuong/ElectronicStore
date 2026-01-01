using Application.DTOs.Requests;
using Application.DTOs.Responses;
using Application.Interfaces;
using Domain.Constants;
using Domain.Entities;
using Domain.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application.Implementations
{
    public class CustomerService : ICustomerService
    {
        private readonly IUnitOfWork _unitOfWork;

        public CustomerService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<string> CreateAsync(CreateCustomerRequest request)
        {
            var count = await _unitOfWork.CustomerRepository.CountAsync();
            var customer = new Customer
            {
                CustomerId = Prefixes.CUSTOMER_ID_PREFIX + string.Format(Prefixes.ID_FORMAT, count + 1),
                CustomerName = request.CustomerName,
                CustomerEmail = request.CustomerEmail,
                CustomerPhone = request.CustomerPhone,
                CustomerAddress = request.CustomerAddress,
                CustomerDOB = request.CustomerDOB,
                AccountId = request.AccountId
            };
            await _unitOfWork.CustomerRepository.AddAsync(customer);
            await _unitOfWork.CommitAsync();
            return customer.CustomerId;
        }

        public async Task<IEnumerable<CustomerResponse>> GetAllAsync()
        {
            var customers = await _unitOfWork.CustomerRepository.GetAllAsync();
            return customers.Select(c => new CustomerResponse
            {
                CustomerId = c.CustomerId,
                CustomerName = c.CustomerName,
                CustomerEmail = c.CustomerEmail,
                CustomerPhone = c.CustomerPhone,
                CustomerAddress = c.CustomerAddress,
                CustomerDOB = c.CustomerDOB,
                AccountId = c.AccountId
            });
        }

        public async Task<CustomerResponse?> GetByIdAsync(string customerId)
        {
            var c = await _unitOfWork.CustomerRepository.GetAsync(x => x.CustomerId == customerId);
            if (c == null) return null;
            return new CustomerResponse
            {
                CustomerId = c.CustomerId,
                CustomerName = c.CustomerName,
                CustomerEmail = c.CustomerEmail,
                CustomerPhone = c.CustomerPhone,
                CustomerAddress = c.CustomerAddress,
                CustomerDOB = c.CustomerDOB,
                AccountId = c.AccountId
            };
        }

        public async Task<bool> UpdateAsync(UpdateCustomerRequest request)
        {
            var customer = await _unitOfWork.CustomerRepository.GetAsync(c => c.CustomerId == request.CustomerId);
            if (customer == null) return false;
            customer.CustomerName = request.CustomerName;
            customer.CustomerEmail = request.CustomerEmail;
            customer.CustomerPhone = request.CustomerPhone;
            customer.CustomerAddress = request.CustomerAddress;
            customer.CustomerDOB = request.CustomerDOB;
            customer.AccountId = request.AccountId;
            _unitOfWork.CustomerRepository.Update(customer);
            await _unitOfWork.CommitAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(string customerId)
        {
            var customer = await _unitOfWork.CustomerRepository.GetAsync(c => c.CustomerId == customerId);
            if (customer == null) return false;
            _unitOfWork.CustomerRepository.Delete(customer);
            await _unitOfWork.CommitAsync();
            return true;
        }
    }
}
