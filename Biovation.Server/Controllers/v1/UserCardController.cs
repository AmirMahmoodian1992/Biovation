using System.Collections.Generic;
using Biovation.Domain;
using Biovation.Service.Api.v1;
using Microsoft.AspNetCore.Mvc;
using RestSharp;

namespace Biovation.Server.Controllers.v1
{
    [Route("biovation/api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    public class UserCardController : Controller
    {
        //private readonly CommunicationManager<int> _communicationManager = new CommunicationManager<int>();
        private readonly UserCardService _userCard;
        private readonly RestClient _restClient;

        public UserCardController(UserCardService userCard, RestClient restClient)
        {
            _userCard = userCard;
            _restClient = restClient;
        }
        [HttpPost]
        [Route("ModifyUserCard")]
        public ResultViewModel ModifyUserCard([FromBody]UserCard userCard)
        {
            var res = _userCard.ModifyUserCard(userCard);
            return res;
        }

        [HttpGet]
        [Route("GetUserCard")]
        public List<UserCard> GetUserCard(int userId = 0)
        {
            return _userCard.GetCardsByFilter(userId: userId);
        }

        [HttpPost]
        [Route("DeleteUserCard")]
        public ResultViewModel DeleteUserCard([FromBody]int id)
        {
            return _userCard.DeleteUserCard(id);
        }

        [HttpGet]
        [Route("ReadCardNumber")]
        public int ReadCardNumber(string brandName, int deviceId)
        {
            var resultRequest =
                new RestRequest(
                    $"/{brandName}/VirdiDevice/ReadCardNum",
                    Method.GET);
            resultRequest.AddParameter("deviceId", deviceId);
            return _restClient.ExecuteAsync<int>(resultRequest).Result.Data;
        }
    }
}
