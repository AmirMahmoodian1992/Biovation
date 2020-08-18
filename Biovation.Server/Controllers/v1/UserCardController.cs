using System.Collections.Generic;
using Biovation.CommonClasses.Manager;
using Biovation.CommonClasses.Models;
using Biovation.CommonClasses.Service;
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
        private readonly RestClient _restServer;

        public UserCardController(UserCardService userCard)
        {
            _userCard = userCard;
            _restServer = new RestClient(($"http://localhost:{BiovationConfigurationManager.BiovationWebServerPort}"));
            //_communicationManager.SetServerAddress($"http://localhost:{ConfigurationManager.BiovationWebServerPort}");
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
            var res = _userCard.GetAllUserCardsOfUser(userId);
            return res;
        }

        [HttpPost]
        [Route("DeleteUserCard")]
        public ResultViewModel DeleteUserCard([FromBody]int id)
        {
            var res = _userCard.DeleteUserCard(id);
            return res;
        }

        [HttpGet]
        [Route("ReadCardNumber")]
        public int ReadCardNumber(string brandName, int deviceId)
        {
            //var param = $"deviceId={deviceId}";

            //return _communicationManager.CallRest($"/biovation/api/{brandName}/VirdiDevice/ReadCardNum", "Get", new List<object> { param }, null);

            var restRequest =
                new RestRequest(
                    $"/biovation/api/{brandName}/VirdiDevice/ReadCardNum",
                    Method.GET);
            restRequest.AddParameter("deviceId", deviceId);
            return _restServer.ExecuteAsync<int>(restRequest).Result.Data;
        }
    }
}
