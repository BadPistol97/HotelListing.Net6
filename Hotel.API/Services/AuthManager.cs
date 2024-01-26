using AutoMapper;
using Hotel.API.Contracts;
using Hotel.API.Data;
using Hotel.API.Models.Users;
using Microsoft.AspNetCore.Identity;
//using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Json;

namespace Hotel.API.Services
{
    public class AuthManager : IAuthManager
    {
        private readonly IMapper _mapper;
        private readonly UserManager<ApiUser> _userManager;
        private readonly IConfiguration _configuration;
        private readonly ILogger<AuthManager> _logger;
        private ApiUser _user;

        private const string _loginProvider = "HotelListingApi";
        private const string _refreshToken = "RefreshToken";

        public AuthManager(IMapper mapper, UserManager<ApiUser> userManager, IConfiguration configuration, ILogger<AuthManager> logger)
        {
            this._mapper = mapper;
            this._userManager = userManager;
            this._configuration = configuration;
            this._logger = logger;
        }

        public async Task<IEnumerable<IdentityError>> Register(ApiUserDto userDto, bool IsAdmin = false)
        {

            ApiUser user = _mapper.Map<ApiUser>(userDto);

            _user.UserName = user.Email;

            _user.PasswordHash = _userManager.PasswordHasher.HashPassword(_user,userDto.Password);

            var result = await _userManager.CreateAsync(_user);

            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(_user, IsAdmin ? "Administrator":"User");

            }

            return result.Errors;
        }

        public async Task<AuthResponseDto> Login(LoginDto loginDto)
        {

            //ServiceProvider serviceProvider = new ServiceCollection()
            //.AddLogging((loggingBuilder) => loggingBuilder
            //    .SetMinimumLevel(LogLevel.Trace)
            //    .AddConsole()
            //    )
            //.BuildServiceProvider();

            //var logger = serviceProvider.GetService<ILoggerFactory>().CreateLogger<Program>();

            try
            {
                _user = await _userManager.FindByEmailAsync(loginDto.Email);

                if (_user is null) return null;

                if (await _userManager.CheckPasswordAsync(_user, loginDto.Password))
                {

                    //_logger.LogInformation($"Hello {JsonSerializer.Serialize(token)}");

                    return new AuthResponseDto
                    {
                        Token = await GenerateToken(),
                        UserId = _user.Id,
                        RefreshToken = await GenerateRefreshToken()
                    };
                }

                else return null;
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex.Message);
                return null;
            }
           

        }

        private async Task<string> GenerateToken() {

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtSettings:Key"]));

            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var roles = await _userManager.GetRolesAsync(_user);

            var roleClaims = roles.Select(role => new Claim(ClaimTypes.Role, role)).ToList();

            var userClaims = await _userManager.GetClaimsAsync(_user);

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, _user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Email, _user.Email),
                new Claim("uid",_user.Id),
            }
            .Union(userClaims).Union(roleClaims);

            var token = new JwtSecurityToken(
                    issuer: _configuration["JwtSettings:Issuer"],
                    audience: _configuration["JwtSettings:Audience"],
                    claims: claims,
                    expires: DateTime.Now.AddMinutes(Convert.ToInt32(_configuration["JwtSettings:DurationInMinutes"])),
                    signingCredentials: credentials
                );


            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public async Task<string> GenerateRefreshToken()
        {
            await _userManager.RemoveAuthenticationTokenAsync(_user,_loginProvider,_refreshToken);

            var newRefreshToken = await _userManager.GenerateUserTokenAsync(_user,_loginProvider, _refreshToken);

            await _userManager.SetAuthenticationTokenAsync(_user, _loginProvider, _refreshToken, newRefreshToken);

            return newRefreshToken;
        }

        public async Task<AuthResponseDto> VerifyRefreshToken(AuthResponseDto request)
        {
            var jwtSecurityTokenHandler = new JwtSecurityTokenHandler();

            var tokenContent = jwtSecurityTokenHandler.ReadJwtToken(request.Token);

            var userEmail = tokenContent.Claims.ToList().FirstOrDefault(q => q.Value == JwtRegisteredClaimNames.Email).Value;

            _user = await _userManager.FindByEmailAsync(userEmail);

            if (_user is null || _user.Id != request.UserId) return null;

            var isValidRefershToken = await _userManager.VerifyUserTokenAsync(_user, _loginProvider, _refreshToken, request.RefreshToken);

            if (!isValidRefershToken) {
                await _userManager.UpdateSecurityStampAsync(_user);
                return null;
            }

            return new AuthResponseDto
            {
                Token = await GenerateToken(),
                UserId = _user.Id,
                RefreshToken = await GenerateRefreshToken()
            };


        }


    }
}
