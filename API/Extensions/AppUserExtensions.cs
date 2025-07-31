using System;
using API.DTOs;
using API.entities;
using API.interfaces;
namespace API.Extensions;

public static class AppUserExtensions
{
    public static UserDto ToDto(this AppUser user, ITokenService tokenService)
    {
      return new UserDto
        {
            ID = user.ID,
            DisplayName = user.DisplayName,
            Email = user.Email,
            Token = tokenService.CreateToken(user)

        
        };
    }
}