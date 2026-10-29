
using BankingLedgerApi.Models;

namespace BankingLedgerApi.Repositories;

public interface ICustomerRepository
{
    Task<Customer?> GetByEmailAsync(string email);

    Task<Customer?> GetByIdAsync(string id);

    Task<Customer?> GetByAppUserIdAsync(string appUserId);

    Task CreateAsync(Customer customer);

    Task UpdateAsync(Customer customer);
}
