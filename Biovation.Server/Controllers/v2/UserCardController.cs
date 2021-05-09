﻿using Biovation.Domain;
using Biovation.Server.Attribute;
using Biovation.Service.Api.v2;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Biovation.Server.Controllers.v2
{
    [Authorize]
    [ApiController]
    [ApiVersion("2.0")]
    [Route("biovation/api/v{version:apiVersion}/[controller]")]
    public class UserCardController : ControllerBase
    {
        private readonly UserCardService _userCardService;

        public UserCardController(UserCardService userCard)
        {
            _userCardService = userCard;
        }

        //[HttpPost]
        //public Task<ResultViewModel> AddUserCard([FromBody]UserCard card = default)
        //{
        //    throw null;
        //}

        [HttpPut]
        public Task<ResultViewModel> ModifyUserCard([FromBody] UserCard card = default)
        {
            var token = HttpContext.Items["Token"] as string;
            return Task.Run(() => _userCardService.ModifyUserCard(card, token));
        }


        [HttpDelete]
        [Route("{id}")]
        public async Task<ResultViewModel> DeleteUserCard([FromRoute] int id = default)
        {
            return await _userCardService.DeleteUserCard(id, HttpContext.Items["Token"] as string);
        }
    }
}
