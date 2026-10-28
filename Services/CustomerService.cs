using BankingLedgerApi.DTOs;
using BankingLedgerApi.Models;
using BankingLedgerApi.Repositories;
using MongoDB.Bson;

namespace BankingLedgerApi.Services;

public class CustomerService
{
    private readonly ICustomerRepository _customerRepository;

    public CustomerService(ICustomerRepository customerRepository)
    {
        _customerRepository = customerRepository;
    }

    public async Task<Customer> CreateAsync(CreateCustomerDto dto)
    {
        string email = dto.Email.Trim().ToLowerInvariant();

        var existingCustomer =
            await _customerRepository.GetByEmailAsync(email);

        if (existingCustomer is not null)
        {
            throw new InvalidOperationException(
                "A customer with this email already exists.");
        }

        var customer = new Customer
        {
            FullName = dto.FullName.Trim(),
            Email = email,
            PhoneNumber = dto.PhoneNumber.Trim(),
            CreatedAt = DateTime.UtcNow
        };

        await _customerRepository.CreateAsync(customer);

        return customer;
    }

    public async Task<Customer?> GetByIdAsync(string id)
    {
        if (!ObjectId.TryParse(id, out _))
        {
            return null;
        }

        return await _customerRepository.GetByIdAsync(id);
    }
}
