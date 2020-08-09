using Biovation.CommonClasses;
using Biovation.CommonClasses.Models;
using Biovation.CommonClasses.Service;
using System.Collections.Generic;
using System.Web.Http;
using Biovation.CommonClasses.Manager;

namespace Biovation.WebService.APIControllers
{
    public class UserCardController : ApiController
    {
        private readonly CommunicationManager<int> _communicationManager = new CommunicationManager<int>();
        private readonly UserCardService _userCard = new UserCardService();

        public UserCardController()
        {
            _communicationManager.SetServerAddress($"http://localhost:{ConfigurationManager.BiovationWebServerPort}");
        }
        [HttpPost]
        public ResultViewModel ModifyUserCard([FromBody]UserCard userCard)
        {
            var res = _userCard.ModifyUserCard(userCard);
            return res;
        }

        [HttpGet]
        public List<UserCard> GetUserCard(int userId = 0)
        {
            var res = _userCard.GetAllUserCardsOfUser(userId);
            return res;
        }

        [HttpPost]
        public ResultViewModel DeleteUserCard([FromBody]int id)
        {
            var res = _userCard.DeleteUserCard(id);
            return res;
        }

        [HttpGet]
        public int ReadCardNumber(string brandName, int deviceId)
        {
            var param = $"deviceId={deviceId}";

            return _communicationManager.CallRest($"/biovation/api/{brandName}/VirdiDevice/ReadCardNum", "Get", new List<object> { param }, null);
        }
    }
}
