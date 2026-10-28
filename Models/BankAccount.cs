using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace BankingLedgerApi.Models;

public class BankAccount
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    [BsonElement("accountNumber")]
    public string AccountNumber { get; set; } = string.Empty;

    [BsonElement("customerId")]
    [BsonRepresentation(BsonType.ObjectId)]
    public string CustomerId { get; set; } = string.Empty;

    [BsonElement("balance")]
    [BsonRepresentation(BsonType.Decimal128)]
    public decimal Balance { get; set; } = 0m;

    [BsonElement("currency")]
    public string Currency { get; set; } = "INR";

    [BsonElement("status")]
    public string Status { get; set; } = "Active";

    [BsonElement("createdAt")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
