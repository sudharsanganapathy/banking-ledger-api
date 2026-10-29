using BankingLedgerApi.Data;
using BankingLedgerApi.Models;
using MongoDB.Driver;

namespace BankingLedgerApi.Repositories;

public class UserRepository : IUserRepository
{
    private readonly MongoDbContext _context;

    public UserRepository(MongoDbContext context)
    {
        _context = context;
    }

    public async Task<AppUser?> GetByEmailAsync(string email)
    {
        return await _context.Users
            .Find(user => user.Email == email)
            .FirstOrDefaultAsync();
    }

    public async Task CreateAsync(AppUser user)
    {
        await _context.Users.InsertOneAsync(user);
    }
    
    public async Task UpdateAsync(AppUser user)
    {
        await _context.Users.ReplaceOneAsync(
            existingUser => existingUser.Id == user.Id,
            user);
    }

}
