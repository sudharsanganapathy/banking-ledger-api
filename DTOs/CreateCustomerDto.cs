using System.ComponentModel.DataAnnotations;

namespace BankingLedgerApi.DTOs;

public class CreateCustomerDto
{
    [Required(ErrorMessage = "Full name is required.")]
    [StringLength(100, MinimumLength = 2,
        ErrorMessage = "Full name must be between 2 and 100 characters.")]
    public string FullName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress(ErrorMessage = "Enter a valid email address.")]
    [StringLength(150)]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Phone number is required.")]
    [RegularExpression(@"^[0-9]{10}$",
        ErrorMessage = "Phone number must contain exactly 10 digits.")]
    public string PhoneNumber { get; set; } = string.Empty;
}
