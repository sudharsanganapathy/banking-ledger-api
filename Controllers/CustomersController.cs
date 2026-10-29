using BankingLedgerApi.DTOs;
using BankingLedgerApi.Services;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace BankingLedgerApi.Controllers;

[ApiController]
[Route("api/customers")]
[Authorize]
public class CustomersController : ControllerBase
{
    private readonly CustomerService _customerService;

    public CustomersController(CustomerService customerService)
    {
        _customerService = customerService;
    }

    [HttpPost]
    public async Task<IActionResult> CreateCustomer(
        [FromBody] CreateCustomerDto dto)
    {
        try
        {
            var customer = await _customerService.CreateAsync(dto);

            return CreatedAtAction(
                nameof(GetCustomerById),
                new { id = customer.Id },
                customer);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
        catch (MongoWriteException ex)
            when (ex.WriteError?.Category == ServerErrorCategory.DuplicateKey)
        {
            return Conflict(new
            {
                message = "A customer with this email already exists."
            });
        }
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetCustomerById(string id)
{
    var customer = await _customerService.GetByIdAsync(id);

    if (customer is null)
    {
        return NotFound(new { message = "Customer not found." });
    }

    var loggedInUserId = User.FindFirstValue(
        ClaimTypes.NameIdentifier);

    if (customer.AppUserId != loggedInUserId)
    {
        return NotFound(new { message = "Customer not found." });
    }

    return Ok(customer);
  }
}
