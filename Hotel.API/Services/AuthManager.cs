using AutoMapper;
using Hotel.API.Contracts;
using Hotel.API.Data;
using Hotel.API.Models.Users;
using Microsoft.AspNetCore.Identity;

namespace Hotel.API.Services
{
    public class AuthManager : IAuthManager
    {
        private readonly IMapper _mapper;
        private readonly UserManager<ApiUser> _userManager;

        public AuthManager(IMapper mapper, UserManager<ApiUser> userManager)
        {
            this._mapper = mapper;
            this._userManager = userManager;
        }

        public async Task<IEnumerable<IdentityError>> Register(ApiUserDto userDto)
        {
            ApiUser user = _mapper.Map<ApiUser>(userDto);

            user.UserName = user.Email;

            var result = await _userManager.CreateAsync(user);

            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, "User");

            }

            return result.Errors;
        }
    }
}
