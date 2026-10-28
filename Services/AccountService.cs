using BankingLedgerApi.DTOs;
using BankingLedgerApi.Models;
using BankingLedgerApi.Repositories;
using MongoDB.Bson;

namespace BankingLedgerApi.Services;

public class AccountService
{
    private readonly ICustomerRepository _customerRepository;
    private readonly IAccountRepository _accountRepository;

    public AccountService(
        ICustomerRepository customerRepository,
        IAccountRepository accountRepository)
    {
        _customerRepository = customerRepository;
        _accountRepository = accountRepository;
    }

    public async Task<BankAccount> CreateAsync(CreateAccountDto dto)
    {
        if (!ObjectId.TryParse(dto.CustomerId, out _))
        {
            throw new ArgumentException("Invalid customer ID.");
        }

        var customer =
            await _customerRepository.GetByIdAsync(dto.CustomerId);

        if (customer is null)
        {
            throw new KeyNotFoundException("Customer not found.");
        }

        var account = new BankAccount
        {
            AccountNumber = $"BA{Guid.NewGuid():N}"[..12].ToUpperInvariant(),
            CustomerId = customer.Id!,
            Balance = 0m,
            Currency = "INR",
            Status = "Active",
            CreatedAt = DateTime.UtcNow
        };

        await _accountRepository.CreateAsync(account);

        return account;
    }

    public async Task<BankAccount?> GetByAccountNumberAsync(string accountNumber)
    {
        return await _accountRepository
            .GetByAccountNumberAsync(accountNumber);
    }

    public async Task<decimal?> GetBalanceAsync(string accountNumber)
    {
        var account = await GetByAccountNumberAsync(accountNumber);

        return account?.Balance;
    }
}
