using System.Linq;
using Biovation.CommonClasses.Manager;
using Biovation.Servers;
using Biovation.Service.Api.v2;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Biovation.Server.Controllers.v2
{
    [ApiVersion("2.0")]
    [Route("biovation/api/v2/[controller]")]
    //[Route("biovation/api/v{version:apiVersion}/[controller]")]
    public class LoginController : ControllerBase
    {
        //private readonly CommunicationManager<DeviceBasicInfo> _communicationManager = new CommunicationManager<DeviceBasicInfo>();

        private readonly UserService _userService;
        private readonly BiovationConfigurationManager _biovationConfigurationManager;
        private readonly TokenGenerator _generateToken;

        public LoginController(UserService userService, BiovationConfigurationManager biovationConfigurationManager, TokenGenerator generateToken)
        {
            _biovationConfigurationManager = biovationConfigurationManager;
            _userService = userService;
            _generateToken = generateToken;
        }


        [HttpGet]
        [Route("{id}")]
        [AllowAnonymous]
        public IActionResult Login(long id = default)
        {
            IActionResult response = Unauthorized();
            var user = _userService.GetUsers(code:id)?.Data?.Data?.FirstOrDefault();
            if (user != null)
            {
                //var tokenString = _generateToken.GenerateJWTLoginToken(user);
                var tokenString = _generateToken.GenerateToken(user);
                response = Ok(new
                {
                    token = tokenString
                    //userDetails = user,
                });
            }
            return response;
        }



     

    }
}