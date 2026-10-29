using BankingLedgerApi.Models;

namespace BankingLedgerApi.Repositories;

public interface IUserRepository
{
    Task<AppUser?> GetByEmailAsync(string email);

    Task CreateAsync(AppUser user);

    Task UpdateAsync(AppUser user);

}
