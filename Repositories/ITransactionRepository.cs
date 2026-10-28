
using BankingLedgerApi.Models;

namespace BankingLedgerApi.Repositories;

public interface ITransactionRepository
{
    Task<BankAccount?> UpdateBalanceAsync(
        string accountNumber,
        decimal amount,
        bool isDeposit);

    Task<bool> TryRollbackBalanceAsync(
        string accountId,
        decimal expectedBalance,
        decimal balanceAdjustment);

    Task CreateAsync(LedgerTransaction transaction);

    Task<List<LedgerTransaction>> GetHistoryAsync(
        string accountNumber);
}
