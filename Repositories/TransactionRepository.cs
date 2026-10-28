using BankingLedgerApi.Data;
using BankingLedgerApi.Models;
using MongoDB.Driver;

namespace BankingLedgerApi.Repositories;

public class TransactionRepository : ITransactionRepository
{
    private readonly MongoDbContext _context;

    public TransactionRepository(MongoDbContext context)
    {
        _context = context;
    }

    public async Task<BankAccount?> UpdateBalanceAsync(
        string accountNumber,
        decimal amount,
        bool isDeposit)
    {
        var filter =
            Builders<BankAccount>.Filter.Eq(
                account => account.AccountNumber, accountNumber)
            & Builders<BankAccount>.Filter.Eq(
                account => account.Status, "Active");

        if (!isDeposit)
        {
            filter &= Builders<BankAccount>.Filter.Gte(
                account => account.Balance, amount);
        }

        var update = Builders<BankAccount>.Update.Inc(
            account => account.Balance,
            isDeposit ? amount : -amount);

        return await _context.Accounts.FindOneAndUpdateAsync(
            filter,
            update,
            new FindOneAndUpdateOptions<BankAccount>
            {
                ReturnDocument = ReturnDocument.After
            });
    }

    public async Task<bool> TryRollbackBalanceAsync(
        string accountId,
        decimal expectedBalance,
        decimal balanceAdjustment)
    {
        var filter =
            Builders<BankAccount>.Filter.Eq(
                account => account.Id, accountId)
            & Builders<BankAccount>.Filter.Eq(
                account => account.Balance, expectedBalance);

        var update = Builders<BankAccount>.Update.Inc(
            account => account.Balance, balanceAdjustment);

        var result = await _context.Accounts.UpdateOneAsync(
            filter,
            update);

        return result.ModifiedCount > 0;
    }

    public async Task CreateAsync(LedgerTransaction transaction)
    {
        await _context.Transactions.InsertOneAsync(transaction);
    }

    public async Task<List<LedgerTransaction>> GetHistoryAsync(
        string accountNumber)
    {
        return await _context.Transactions
            .Find(transaction =>
                transaction.AccountNumber == accountNumber)
            .SortByDescending(transaction => transaction.CreatedAt)
            .ToListAsync();
    }
}
