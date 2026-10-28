using System.ComponentModel.DataAnnotations;

namespace BankingLedgerApi.DTOs;

public class CreateAccountDto
{
    [Required(ErrorMessage = "Customer ID is required.")]
    public string CustomerId { get; set; } = string.Empty;
}
