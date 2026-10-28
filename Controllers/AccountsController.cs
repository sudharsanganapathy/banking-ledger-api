using BankingLedgerApi.DTOs;
using BankingLedgerApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace BankingLedgerApi.Controllers;

[ApiController]
[Route("api/accounts")]
public class AccountsController : ControllerBase
{
    private readonly AccountService _accountService;

    public AccountsController(AccountService accountService)
    {
        _accountService = accountService;
    }

    [HttpPost]
    public async Task<IActionResult> CreateAccount(
        [FromBody] CreateAccountDto dto)
    {
        try
        {
            var account = await _accountService.CreateAsync(dto);

            return CreatedAtAction(
                nameof(GetAccountByNumber),
                new { accountNumber = account.AccountNumber },
                account);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    [HttpGet("{accountNumber}")]
    public async Task<IActionResult> GetAccountByNumber(
        string accountNumber)
    {
        var account = await _accountService
            .GetByAccountNumberAsync(accountNumber);

        if (account is null)
        {
            return NotFound(new { message = "Account not found." });
        }

        return Ok(account);
    }

    [HttpGet("{accountNumber}/balance")]
    public async Task<IActionResult> GetBalance(string accountNumber)
    {
        var balance = await _accountService
            .GetBalanceAsync(accountNumber);

        if (balance is null)
        {
            return NotFound(new { message = "Account not found." });
        }

        return Ok(new
        {
            accountNumber,
            balance,
            currency = "INR"
        });
    }
}
