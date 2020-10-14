using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Biovation.CommonClasses.Manager;
using Biovation.Domain;
using Biovation.Servers;
using Biovation.Service.Api.v2;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Linq;

namespace Biovation.Server.Controllers.v2
{
    //[Route("Biovation/Api/{controller}/{action}", Name = "Device")]
    [Route("biovation/api/v{version:apiVersion}/[controller]")]
    [ApiVersion("2.0")]
    public class LoginController : Controller
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
            var user = _userService.GetUsers(userId:id).Data.Data.FirstOrDefault();
            if (user != null)
            {
                var tokenString = _generateToken.GenerateJWTLoginToken(user);
                response = Ok(new
                {
                    token = tokenString,
                    //userDetails = user,
                });
            }
            return response;
        }



     

    }
}