using BankingLedgerApi.DTOs;
using BankingLedgerApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace BankingLedgerApi.Controllers;

[ApiController]
[Route("api/transactions")]
public class TransactionsController : ControllerBase
{
    private readonly TransactionService _transactionService;

    public TransactionsController(TransactionService transactionService)
    {
        _transactionService = transactionService;
    }

    [HttpPost("deposit")]
    public async Task<IActionResult> Deposit(
        [FromBody] MoneyTransactionDto dto)
    {
        try
        {
            var transaction =
                await _transactionService.DepositAsync(dto);

            return Ok(transaction);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }

    [HttpPost("withdraw")]
    public async Task<IActionResult> Withdraw(
        [FromBody] MoneyTransactionDto dto)
    {
        try
        {
            var transaction =
                await _transactionService.WithdrawAsync(dto);

            return Ok(transaction);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }

    [HttpGet("{accountNumber}/history")]
    public async Task<IActionResult> GetHistory(string accountNumber)
    {
        if (string.IsNullOrWhiteSpace(accountNumber))
        {
            return BadRequest(new
            {
                message = "Account number is required."
            });
        }

        try
        {
            var history = await _transactionService
                .GetHistoryAsync(accountNumber.Trim());

            if (history.Count == 0)
            {
                return Ok(new
                {
                    accountNumber,
                    message = "No transactions found for this account.",
                    transactions = history
                });
            }

            return Ok(new
            {
                accountNumber,
                totalTransactions = history.Count,
                transactions = history
            });
        }
        catch (Exception)
        {
            return StatusCode(500, new
            {
                message = "Unable to retrieve transaction history."
            });
        }
    }
}
