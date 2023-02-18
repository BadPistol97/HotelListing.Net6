using Hotel.API.Models.Users;
using Microsoft.AspNetCore.Identity;

namespace Hotel.API.Contracts
{
    public interface IAuthManager
    {
        Task<IEnumerable<IdentityError>> Register(ApiUserDto userDto); 
    }
}
