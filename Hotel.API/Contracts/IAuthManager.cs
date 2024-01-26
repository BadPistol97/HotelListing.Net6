using Hotel.API.Models.Users;
using Microsoft.AspNetCore.Identity;

namespace Hotel.API.Contracts
{
    public interface IAuthManager
    {
        Task<IEnumerable<IdentityError>> Register(ApiUserDto userDto, bool IsAdmin = true);

        Task<AuthResponseDto> Login(LoginDto loginDto);

        Task<string> GenerateRefreshToken();

        Task<AuthResponseDto> VerifyRefreshToken(AuthResponseDto request);
    }
}
