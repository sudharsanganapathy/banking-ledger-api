using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace BankingLedgerApi.Models;

public class LedgerTransaction
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    [BsonElement("transactionReference")]
    public string TransactionReference { get; set; } = string.Empty;

    [BsonElement("accountId")]
    [BsonRepresentation(BsonType.ObjectId)]
    public string AccountId { get; set; } = string.Empty;

    [BsonElement("accountNumber")]
    public string AccountNumber { get; set; } = string.Empty;

    // Deposit or Withdrawal
    [BsonElement("type")]
    public string Type { get; set; } = string.Empty;

    [BsonElement("amount")]
    [BsonRepresentation(BsonType.Decimal128)]
    public decimal Amount { get; set; }

    [BsonElement("balanceBefore")]
    [BsonRepresentation(BsonType.Decimal128)]
    public decimal BalanceBefore { get; set; }

    [BsonElement("balanceAfter")]
    [BsonRepresentation(BsonType.Decimal128)]
    public decimal BalanceAfter { get; set; }

    [BsonElement("description")]
    public string Description { get; set; } = string.Empty;

    [BsonElement("status")]
    public string Status { get; set; } = "Completed";

    [BsonElement("createdAt")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
