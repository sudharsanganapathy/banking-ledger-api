using System.ComponentModel.DataAnnotations;

namespace BankingLedgerApi.DTOs;

public class MoneyTransactionDto
{
    [Required(ErrorMessage = "Account number is required.")]
    public string AccountNumber { get; set; } = string.Empty;

    [Range(
        typeof(decimal),
        "0.01",
        "1000000000",
        ErrorMessage = "Amount must be greater than zero and cannot exceed 1,000,000,000."
    )]
    public decimal Amount { get; set; }

    [StringLength(
        200,
        ErrorMessage = "Description cannot exceed 200 characters."
    )]
    public string Description { get; set; } = string.Empty;
}
