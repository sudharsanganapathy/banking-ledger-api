
using BankingLedgerApi.Models;
using MongoDB.Driver;

namespace BankingLedgerApi.Data;

public class MongoDbContext
{
    private readonly IMongoDatabase _database;

    public IMongoCollection<Customer> Customers { get; }

    public IMongoCollection<BankAccount> Accounts { get; }

    public IMongoCollection<LedgerTransaction> Transactions { get; }

    public MongoDbContext(IConfiguration configuration)
    {
        string connectionString =
            configuration["MongoDbSettings:ConnectionString"]
            ?? throw new InvalidOperationException(
                "MongoDB connection string is missing.");

        string databaseName =
            configuration["MongoDbSettings:DatabaseName"]
            ?? throw new InvalidOperationException(
                "MongoDB database name is missing.");

        var client = new MongoClient(connectionString);

        _database = client.GetDatabase(databaseName);

        Customers = _database.GetCollection<Customer>("customers");

        Accounts = _database.GetCollection<BankAccount>("accounts");

        Transactions = _database.GetCollection<LedgerTransaction>("transactions");
    }
}
