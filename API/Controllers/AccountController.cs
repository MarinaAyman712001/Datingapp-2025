using API.DTOs; // تأكدي من استيراده
using API.entities;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using API.Data;
using Microsoft.EntityFrameworkCore;
using API.interfaces;
using API.Extensions;

namespace API.Controllers;

public class AccountController(AppDbContext context, ITokenService tokenService) : BaseApiController
{
    [HttpPost("register")]
    public async Task<ActionResult<UserDto>> Register([FromBody] RegisterDto registerDto)
    {

        if (await EmailExisits(registerDto.Email)) return BadRequest("Email taken");

        using var hmac = new HMACSHA512();

        var user = new AppUser
        {
            DisplayName = registerDto.DisplayName,
            Email = registerDto.Email,
            passwardHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDto.Password)),
            passwardSalt = hmac.Key
        };

        context.Users.Add(user);
        await context.SaveChangesAsync();

       return user.ToDto(tokenService);

    }



    [HttpPost("login")]
    public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)
    {

        var user = await context.Users.SingleOrDefaultAsync(x => x.Email == loginDto.Email);

        if (user == null) return Unauthorized("Invalid email");

        using var hmac = new HMACSHA512(user.passwardSalt);

        var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDto.Password));

 for (int i = 0; i < computedHash.Length; i++)
    {
        if (computedHash[i] != user.passwardHash[i])
        return Unauthorized("Invalid password");
    }
        return user.ToDto(tokenService);
    }

    private async Task<bool> EmailExisits(string email)
    {
        return await context.Users.AnyAsync(x => x.Email.ToLower() == email.ToLower());
    }
}
