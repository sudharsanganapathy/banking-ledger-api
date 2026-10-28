
using BankingLedgerApi.Models;

namespace BankingLedgerApi.Repositories;

public interface IAccountRepository
{
    Task<BankAccount?> GetByAccountNumberAsync(string accountNumber);

    Task CreateAsync(BankAccount account);
}
