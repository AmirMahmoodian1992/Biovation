using System.Threading.Tasks;
using Biovation.CommonClasses.Manager;
using Biovation.Domain;
using Biovation.Service.API.v2;
using Microsoft.AspNetCore.Mvc;
using RestSharp;

namespace Biovation.Server.Controllers.v2
{
    [Route("biovation/api/v{version:apiVersion}/[controller]")]
    [ApiVersion("2.0")]
    public class UserCardController : Controller
    {
        private readonly UserCardService _userCard;
        private readonly RestClient _restClient;

        public UserCardController(UserCardService userCard, RestClient restClient)
        {
            _userCard = userCard;
            _restClient = restClient;
        }

        //[HttpPost]
        //public Task<ResultViewModel> AddUserCard([FromBody]UserCard card = default)
        //{
        //    throw null;
        //}

        [HttpPut]
        public Task<ResultViewModel> ModifyUserCard([FromBody]UserCard card = default)
        {
            return Task.Run(()=> _userCard.ModifyUserCard(card));
        }

        [HttpGet]
        [Route("{id}")]
        public Task<ResultViewModel<PagingResult<UserCard>>> GetUserCard(long userId, bool isActive,
        int pageNumber = default, int pageSize = default)
        {
            return Task.Run (() =>  _userCard.GetCardsByFilter(userId, isActive, pageNumber, pageSize));
        }

        [HttpDelete]
        [Route("{id}")]
        public Task<ResultViewModel> DeleteUserCard(int id = default)
        {
            return Task.Run(() => _userCard.DeleteUserCard(id));
        }

        [HttpGet]
        [Route("cardNumber/{deviceId}")]
        public Task<int> ReadCardNumber(string brandName = default, int deviceId = default)
        {
            return Task.Run( () =>  _userCard.ReadCardNumber(brandName,deviceId));
        }
    }
}
