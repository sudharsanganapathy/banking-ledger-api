using BankingLedgerApi.Data;
using BankingLedgerApi.Models;
using MongoDB.Driver;

namespace BankingLedgerApi.Repositories;

public class AccountRepository : IAccountRepository
{
    private readonly MongoDbContext _context;

    public AccountRepository(MongoDbContext context)
    {
        _context = context;
    }

    public async Task<BankAccount?> GetByAccountNumberAsync(
        string accountNumber)
    {
        return await _context.Accounts
            .Find(account => account.AccountNumber == accountNumber)
            .FirstOrDefaultAsync();
    }

    public async Task CreateAsync(BankAccount account)
    {
        await _context.Accounts.InsertOneAsync(account);
    }
}
