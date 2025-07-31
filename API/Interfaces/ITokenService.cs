using System;
using API.entities;
namespace API.interfaces ;
public interface ITokenService
{
     string CreateToken(AppUser user);
}