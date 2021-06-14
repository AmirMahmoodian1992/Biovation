﻿using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Biovation.CommonClasses.Manager;
using Biovation.Domain;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;

namespace Biovation.CommonClasses.Middleware
{
    public class ServicesJwtMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly BiovationConfigurationManager _biovationConfigurationManager;
        public ServicesJwtMiddleware(RequestDelegate next, BiovationConfigurationManager biovationConfigurationManager)
        {
            _next = next;
            _biovationConfigurationManager = biovationConfigurationManager;
        }
        public async Task Invoke(HttpContext context)
        {
            var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(' ').Last();

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
                    ValidateLifetime = false,
                    // set clockskew to zero so tokens expire exactly at token expiration time (instead of 5 minutes later)
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                var jwtToken = (JwtSecurityToken)validatedToken;
                //var user = JsonConvert.DeserializeObject<User>(jwtToken.Claims.First(x => string.Equals(x.Type, "User", StringComparison.InvariantCultureIgnoreCase)).Value);
                var user = JsonConvert.DeserializeObject<User>(jwtToken.Claims.First(x => string.Equals(x.Type, "User", StringComparison.InvariantCultureIgnoreCase)).Value);
                // attach user to context on successful jwt validation
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
