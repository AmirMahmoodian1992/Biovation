using System.Threading.Tasks;
using Biovation.Domain;
using Biovation.Service.Api.v2;
using Microsoft.AspNetCore.Mvc;

namespace Biovation.Server.Controllers.v2
{
    [Route("biovation/api/v{version:apiVersion}/[controller]")]
    [ApiVersion("2.0")]
    public class UserCardController : Controller
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
        public Task<ResultViewModel> ModifyUserCard([FromBody]UserCard card = default)
        {
            var token = (string)HttpContext.Items["Token"];
            return Task.Run(()=> _userCardService.ModifyUserCard(card,token));
        }


        [HttpDelete]
        [Route("{id}")]
        public Task<ResultViewModel> DeleteUserCard(int id = default)
        {
            var token = (string)HttpContext.Items["Token"];
            return Task.Run(() => _userCardService.DeleteUserCard(id, token));
        }
    }
}
