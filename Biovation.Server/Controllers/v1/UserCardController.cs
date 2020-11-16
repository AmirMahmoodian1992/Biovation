using System.Collections.Generic;
using Biovation.CommonClasses.Manager;
using Biovation.Domain;
using Biovation.Service.Api.v1;
using Microsoft.AspNetCore.Mvc;
using RestSharp;

namespace Biovation.Server.Controllers.v1
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("biovation/api/v{version:apiVersion}/[controller]")]
    public class UserCardController : ControllerBase
    {
        //private readonly CommunicationManager<int> _communicationManager = new CommunicationManager<int>();
        private readonly UserCardService _userCard;
        private readonly RestClient _restClient;
        private readonly string _kasraAdminToken;
        private readonly BiovationConfigurationManager _biovationConfigurationManager;

        public UserCardController(UserCardService userCard, RestClient restClient, BiovationConfigurationManager biovationConfigurationManager)
        {
            _userCard = userCard;
            _restClient = restClient;
            _biovationConfigurationManager = biovationConfigurationManager;
            _kasraAdminToken = _biovationConfigurationManager.KasraAdminToken;
        }
        [HttpPost]
        [Route("ModifyUserCard")]
        public ResultViewModel ModifyUserCard([FromBody]UserCard userCard)
        {
            var res = _userCard.ModifyUserCard(userCard, token: _kasraAdminToken);
            return res;
        }

        [HttpGet]
        [Route("GetUserCard")]
        public List<UserCard> GetUserCard(int userId = 0)
        {
            return _userCard.GetCardsByFilter(userId: userId, token: _kasraAdminToken);
        }

        [HttpPost]
        [Route("DeleteUserCard")]
        public ResultViewModel DeleteUserCard(int id)
        {
            return _userCard.DeleteUserCard(id, token: _kasraAdminToken);
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
            resultRequest.AddHeader("Authorization", _biovationConfigurationManager.KasraAdminToken);
            return _restClient.ExecuteAsync<int>(resultRequest).Result.Data;
        }
    }
}
