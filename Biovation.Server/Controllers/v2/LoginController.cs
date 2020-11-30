using Biovation.Server.Managers;
using Biovation.Service.Api.v2;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace Biovation.Server.Controllers.v2
{
    [ApiController]
    [ApiVersion("2.0")]
    [Route("biovation/api/v{version:apiVersion}/[controller]")]
    public class LoginController : ControllerBase
    {
        private readonly UserService _userService;
        private readonly TokenGenerator _generateToken;

        public LoginController(UserService userService, TokenGenerator generateToken)
        {
            _userService = userService;
            _generateToken = generateToken;
        }

        [HttpGet]
        [Route("{id}")]
        [AllowAnonymous]
        public IActionResult Login([FromRoute] long id = default)
        {
            IActionResult response = Unauthorized();
            var user = _userService.GetUsers(code: id)?.Data?.Data?.FirstOrDefault();
            if (user == null) return response;

            var tokenString = _generateToken.GenerateJWTLoginToken(user);
            response = Ok(new
            {
                token = tokenString
            });
            return response;
        }

        [HttpGet]
        [Attribute.Authorize]
        [Route("InternalLogin/{id}")]
        public IActionResult InternalLogin([FromRoute] long id = default)
        {
            IActionResult response = Unauthorized();
            var user = _userService.GetUsers(userId: id)?.Data?.Data?.FirstOrDefault();
            if (user == null) return response;

            var tokenString = _generateToken.GenerateToken(user);
            response = Ok(new
            {
                token = tokenString
            });
            return response;
        }
    }
}