using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using Hotel.API.Contracts;
using Hotel.API.Models.Users;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;

namespace Hotel.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountsController : ControllerBase
    {
        private readonly IAuthManager _authManager;
        private readonly ILogger<AccountsController> _logger;

        public AccountsController(IAuthManager authManager, ILogger<AccountsController> logger)
        {
            this._authManager = authManager;
            this._logger = logger;
        }

        [HttpPost]
        [Route("register")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> Register([FromBody] ApiUserDto apiUserDto)
        {

            var errors = await _authManager.Register(apiUserDto);

            if (errors.Any()) 
            {
                foreach (var error in errors) {
                    ModelState.AddModelError(error.Code, error.Description);
                }

                return BadRequest(ModelState);
            }

            return Ok();

        }

        [HttpPost]
        [Route("register/admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Authorize(Roles = "Administrator")]
        public async Task<ActionResult> RegisterAdmin([FromBody] ApiUserDto apiUserDto) 
        {
            var errors = await _authManager.Register(apiUserDto,IsAdmin:true);

            if (errors.Any())
            {
                foreach (var error in errors)
                {
                    ModelState.AddModelError(error.Code, error.Description);
                }

                return BadRequest(ModelState);
            }

            return Ok();
        }

        [HttpPost]
        [Route("login")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> login([FromBody] LoginDto loginDto)
        {

            var authResponse = await _authManager.Login(loginDto);

            _logger.LogInformation(JsonSerializer.Serialize(authResponse));

            if (authResponse is null) {
                return Unauthorized();
            }

            return Ok(authResponse);
        }

        [HttpPost]
        [Route("refreshToken")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> RefreshToken([FromBody] AuthResponseDto request)
        {

            var authResponse = await _authManager.VerifyRefreshToken(request);

            _logger.LogInformation(JsonSerializer.Serialize(authResponse));

            if (authResponse is null)
            {
                return Unauthorized();
            }

            return Ok(authResponse);
        }

    }
}
