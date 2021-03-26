using Biovation.CommonClasses.Manager;
using Biovation.Domain;
using Biovation.Server.Managers;
using Biovation.Service.Api.v2;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Biovation.Server.Middleware
{
    public class JwtMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly BiovationConfigurationManager _biovationConfigurationManager;
        private readonly UserService _userService;
        private readonly TokenGenerator _generateToken;
        public JwtMiddleware(RequestDelegate next, BiovationConfigurationManager biovationConfigurationManager, UserService userService, TokenGenerator generateToken)
        {
            _next = next;
            _biovationConfigurationManager = biovationConfigurationManager;
            _userService = userService;
            _generateToken = generateToken;
        }
        public async Task Invoke(HttpContext context)
        {
            var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            //var token = context.Request.Headers["typ"].FirstOrDefault()?.Split(" ").Last();

            if (token != null)
                AttachUserToContext(context, token);

            await _next(context);
        }

        private void AttachUserToContext(HttpContext context, string token)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(_biovationConfigurationManager.JwtLoginKey());
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    // set clockskew to zero so tokens expire exactly at token expiration time (instead of 5 minutes later)
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                var jwtToken = (JwtSecurityToken)validatedToken;
                var userCode = int.Parse(jwtToken.Claims.First(x => string.Equals(x.Type, "userCode", StringComparison.InvariantCultureIgnoreCase)).Value);
                var uniqueId = int.Parse(jwtToken.Claims.First(x => string.Equals(x.Type, "uniqueId", StringComparison.InvariantCultureIgnoreCase)).Value);

                // attach user to context on successful jwt validation
                var user = userCode == 987654321 ? _biovationConfigurationManager.SystemDefaultUser :
                    userCode == 123456789 ? _biovationConfigurationManager.KasraAdminUser :
                    _userService.GetUsers(code: userCode)?.Data?.Data?.FirstOrDefault();
                var generatedToken = _generateToken.GenerateToken(user);
                context.Request.Headers["Authorization"] = generatedToken;
                context.Items["Token"] = generatedToken;
                context.Items["User"] = user;
            }
            catch
            {
                // do nothing if jwt validation fails
                // user is not attached to context so request won't have access to secure routes
            }
        }


    }
}
