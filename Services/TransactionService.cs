using System.Collections.Concurrent;
using BankingLedgerApi.DTOs;
using BankingLedgerApi.Models;
using BankingLedgerApi.Repositories;

namespace BankingLedgerApi.Services;

public class TransactionService
{
    private readonly IAccountRepository _accountRepository;
    private readonly ITransactionRepository _transactionRepository;

    // Prevent overlapping transactions for the same account
    // within this application process.
    private static readonly ConcurrentDictionary<string, SemaphoreSlim>
        AccountLocks = new();

    public TransactionService(
        IAccountRepository accountRepository,
        ITransactionRepository transactionRepository)
    {
        _accountRepository = accountRepository;
        _transactionRepository = transactionRepository;
    }

    public Task<LedgerTransaction> DepositAsync(MoneyTransactionDto dto)
    {
        return ProcessAsync(dto, isDeposit: true);
    }

    public Task<LedgerTransaction> WithdrawAsync(MoneyTransactionDto dto)
    {
        return ProcessAsync(dto, isDeposit: false);
    }

    private async Task<LedgerTransaction> ProcessAsync(
        MoneyTransactionDto dto,
        bool isDeposit)
    {
        string accountNumber = dto.AccountNumber.Trim();

        if (string.IsNullOrWhiteSpace(accountNumber))
        {
            throw new ArgumentException("Account number is required.");
        }

        if (dto.Amount <= 0 || dto.Amount > 1_000_000_000m)
        {
            throw new ArgumentException(
                "Amount must be greater than zero and cannot exceed 1,000,000,000.");
        }

        var accountLock = AccountLocks.GetOrAdd(
            accountNumber,
            _ => new SemaphoreSlim(1, 1));

        await accountLock.WaitAsync();

        try
        {
            var updatedAccount =
                await _transactionRepository.UpdateBalanceAsync(
                    accountNumber,
                    dto.Amount,
                    isDeposit);

            if (updatedAccount is null)
            {
                var existingAccount =
                    await _accountRepository.GetByAccountNumberAsync(
                        accountNumber);

                if (existingAccount is null)
                {
                    throw new KeyNotFoundException("Account not found.");
                }

                if (existingAccount.Status != "Active")
                {
                    throw new InvalidOperationException(
                        "This account is not active.");
                }

                if (!isDeposit &&
                    existingAccount.Balance < dto.Amount)
                {
                    throw new InvalidOperationException(
                        "Insufficient balance.");
                }

                throw new InvalidOperationException(
                    "Transaction could not be completed.");
            }

            decimal balanceAfter = updatedAccount.Balance;

            decimal balanceBefore = isDeposit
                ? balanceAfter - dto.Amount
                : balanceAfter + dto.Amount;

            var transaction = new LedgerTransaction
            {
                TransactionReference =
                    $"TXN{Guid.NewGuid():N}"[..15].ToUpperInvariant(),

                AccountId = updatedAccount.Id!,

                AccountNumber = updatedAccount.AccountNumber,

                Type = isDeposit ? "Deposit" : "Withdrawal",

                Amount = dto.Amount,

                BalanceBefore = balanceBefore,

                BalanceAfter = balanceAfter,

                Description = string.IsNullOrWhiteSpace(dto.Description)
                    ? (isDeposit ? "Cash deposit" : "Cash withdrawal")
                    : dto.Description.Trim(),

                Status = "Completed",

                CreatedAt = DateTime.UtcNow
            };

            try
            {
                await _transactionRepository.CreateAsync(transaction);
            }
            catch
            {
                // Best-effort compensation if the ledger write fails.
                // Only reverse the update if the balance is still the
                // value produced by this transaction.
                await _transactionRepository.TryRollbackBalanceAsync(
                    updatedAccount.Id!,
                    balanceAfter,
                    isDeposit ? -dto.Amount : dto.Amount);

                throw;
            }

            return transaction;
        }
        finally
        {
            accountLock.Release();
        }
    }

    public async Task<List<LedgerTransaction>> GetHistoryAsync(
        string accountNumber)
    {
        return await _transactionRepository.GetHistoryAsync(
            accountNumber);
    }
}
