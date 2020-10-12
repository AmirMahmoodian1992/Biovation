using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Biovation.CommonClasses.Manager;
using Biovation.Domain;
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
        
            

        public LoginController(UserService userService, BiovationConfigurationManager biovationConfigurationManager)
        {
            _biovationConfigurationManager = biovationConfigurationManager;
            _userService = userService;
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
                var tokenString = GenerateJWTToken(user);
                response = Ok(new
                {
                    token = tokenString,
                    //userDetails = user,
                });
            }
            return response;
        }



        string GenerateJWTToken(User userInfo)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_biovationConfigurationManager.JwtKey()));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, userInfo.FirstName),
                new Claim("Id", userInfo.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            };

            //var token = new JwtSecurityToken(
            //    issuer: _biovationConfigurationManager.JwtIssuer(),
            //    audience: _biovationConfigurationManager.JwtAudience(),
            //    claims: claims,
            //    expires: DateTime.Now.AddMinutes(30),
            //    signingCredentials: credentials
            //);
            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddMinutes(1130),
                signingCredentials: credentials
            );


            //var token = new JwtSecurityToken(_biovationConfigurationManager.JwtIssuer(),
            //  _biovationConfigurationManager.JwtAudience(),
            //  null,
            //  expires: DateTime.Now.AddMinutes(12000),
            //  signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

    }
}