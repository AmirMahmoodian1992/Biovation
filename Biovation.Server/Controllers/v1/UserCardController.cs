using System.Collections.Generic;
using Biovation.Domain;
using Biovation.Service.API.v2;
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
            var result = _userCard.ModifyUserCard(userCard);
            return result;
        }

        [HttpGet]
        [Route("GetUserCard")]
        public List<UserCard> GetUserCard(int userId = 0)
        {
            var result = _userCard.GetCardsByFilter(userId:userId);
            return result.Data.Data;
        }

        [HttpPost]
        [Route("DeleteUserCard")]
        public ResultViewModel DeleteUserCard([FromBody]int id)
        {
            var result = _userCard.DeleteUserCard(id);
            return result;
        }

        [HttpGet]
        [Route("ReadCardNumber")]
        public int ReadCardNumber(string brandName, int deviceId)
        {
            //var param = $"deviceId={deviceId}";

            //return _communicationManager.CallRest($"/biovation/api/{brandName}/VirdiDevice/ReadCardNum", "Get", new List<object> { param }, null);

            var resultRequest =
                new RestRequest(
                    $"/{brandName}/VirdiDevice/ReadCardNum",
                    Method.GET);
            resultRequest.AddParameter("deviceId", deviceId);
            return _restClient.ExecuteAsync<int>(resultRequest).Result.Data;
        }
    }
}
