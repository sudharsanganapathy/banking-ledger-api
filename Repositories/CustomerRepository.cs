using BankingLedgerApi.Data;
using BankingLedgerApi.Models;
using MongoDB.Driver;

namespace BankingLedgerApi.Repositories;

public class CustomerRepository : ICustomerRepository
{
    private readonly MongoDbContext _context;

    public CustomerRepository(MongoDbContext context)
    {
        _context = context;
    }

    public async Task<Customer?> GetByEmailAsync(string email)
    {
        return await _context.Customers
            .Find(customer => customer.Email == email)
            .FirstOrDefaultAsync();
    }

    public async Task<Customer?> GetByIdAsync(string id)
    {
        return await _context.Customers
            .Find(customer => customer.Id == id)
            .FirstOrDefaultAsync();
    }

    
    public async Task<Customer?> GetByAppUserIdAsync(string appUserId)
    {
        return await _context.Customers
            .Find(customer => customer.AppUserId == appUserId)
            .FirstOrDefaultAsync();
    }

    public async Task CreateAsync(Customer customer)
    {
        await _context.Customers.InsertOneAsync(customer);
    }

    public async Task UpdateAsync(Customer customer)
    {
        await _context.Customers.ReplaceOneAsync(
            existingCustomer => existingCustomer.Id == customer.Id, customer);
    }

}
