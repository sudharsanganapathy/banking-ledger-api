using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BankingLedgerApi.DTOs;
using BankingLedgerApi.Models;
using BankingLedgerApi.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

namespace BankingLedgerApi.Services;

public class AuthService
{
    private readonly IUserRepository _userRepository;
    private readonly ICustomerRepository _customerRepository;
    private readonly PasswordHasher<AppUser> _passwordHasher;
    private readonly IConfiguration _configuration;

    public AuthService(
        IUserRepository userRepository,
        IConfiguration configuration,
        ICustomerRepository customerRepository)
    {
        _userRepository = userRepository;
        _customerRepository = customerRepository;
        _configuration = configuration;
        _passwordHasher = new PasswordHasher<AppUser>();
    }

    public async Task<string> RegisterAsync(RegisterDto dto)
{
    string email = dto.Email.Trim().ToLowerInvariant();

    // 1. Check whether a user already exists.
    var existingUser =
        await _userRepository.GetByEmailAsync(email);

    if (existingUser is not null)
    {
        throw new InvalidOperationException(
            "An account with this email already exists.");
    }

    // 2. Find an existing customer with the same email.
    var customer =
        await _customerRepository.GetByEmailAsync(email);

    // Do not link a customer already associated with another user.
    if (customer?.AppUserId is not null)
    {
        throw new InvalidOperationException(
            "This customer record is already linked to a user account.");
    }

    // 3. Create the application user.
    var user = new AppUser
    {
        FullName = dto.FullName.Trim(),
        Email = email,
        Role = "Customer"
    };

    user.PasswordHash = _passwordHasher.HashPassword(
        user,
        dto.Password);

    // 4. Save the user first to generate its MongoDB ID.
    await _userRepository.CreateAsync(user);

    try
    {
        // 5. Reuse the customer record, or create one.
        if (customer is null)
        {
            customer = new Customer
            {
                FullName = dto.FullName.Trim(),
                Email = email,
                PhoneNumber = dto.PhoneNumber.Trim(),
                AppUserId = user.Id
            };

            await _customerRepository.CreateAsync(customer);
        }
        else
        {
            customer.AppUserId = user.Id;

            await _customerRepository.UpdateAsync(customer);
        }

        // 6. Save the customer's ID on the application user.
        user.CustomerId = customer.Id;

        await _userRepository.UpdateAsync(user);
    }
    catch
    {
        // Registration may be partially completed if a database
        // operation fails. We will improve rollback handling next.
        throw;
    }

    return "Registration successful.";
}

    public async Task<string> LoginAsync(LoginDto dto)
    {
        string email = dto.Email.Trim().ToLowerInvariant();

        var user = await _userRepository.GetByEmailAsync(email);

        if (user is null)
        {
            throw new UnauthorizedAccessException(
                "Invalid email or password.");
        }

        var verificationResult = _passwordHasher.VerifyHashedPassword(
            user,
            user.PasswordHash,
            dto.Password);

        if (verificationResult == PasswordVerificationResult.Failed)
        {
            throw new UnauthorizedAccessException(
                "Invalid email or password.");
        }

        string secret = _configuration["Jwt:Secret"]
            ?? throw new InvalidOperationException(
                "JWT secret is not configured.");

        string issuer = _configuration["Jwt:Issuer"]
            ?? throw new InvalidOperationException(
                "JWT issuer is not configured.");

        string audience = _configuration["Jwt:Audience"]
            ?? throw new InvalidOperationException(
                "JWT audience is not configured.");

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id!),
            new Claim(ClaimTypes.Name, user.FullName),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Role, user.Role)
        };

        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(secret));

        var credentials = new SigningCredentials(
            key,
            SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(30),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
