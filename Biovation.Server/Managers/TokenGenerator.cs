using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Biovation.CommonClasses.Manager;
using Biovation.Domain;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;

namespace Biovation.Server.Managers
{
    public class TokenGenerator
    {
        private readonly BiovationConfigurationManager _biovationConfigurationManager;

        public TokenGenerator(BiovationConfigurationManager biovationConfigurationManager)
        {
            _biovationConfigurationManager = biovationConfigurationManager;
        }

        public string GenerateJWTLoginToken(User userInfo)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_biovationConfigurationManager.JwtLoginKey()));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim("userCode", userInfo.Code.ToString()),
                new Claim("uniqueId", userInfo.UniqueId.ToString()),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
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
                expires: DateTime.Now.AddDays(15),
                signingCredentials: credentials
            );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
        public string GenerateToken(User userInfo)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_biovationConfigurationManager.JwtLoginKey()));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim("User",  JsonConvert.SerializeObject(userInfo)),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };
            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddYears(1),
                signingCredentials: credentials
            );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
