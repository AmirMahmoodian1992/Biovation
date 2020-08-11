using System.Collections.Generic;
using System.Threading.Tasks;
using Biovation.CommonClasses.Manager;
using Biovation.CommonClasses.Models;
using Biovation.CommonClasses.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.SqlServer.TransactSql.ScriptDom;
using RestSharp;

namespace Biovation.Gateway.Controllers.v2
{
    [Route("biovation/api/[controller]")]
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

        [HttpPut]
        [Route("{card}")]
        public Task<IActionResult> ModifyUserCard([FromBody]UserCard card = default)
        {
            throw null;
        }

        [HttpGet]
        [Route("{id}")]
        public Task<IActionResult> GetUserCard(int id = default)
        {
            throw null;
        }

        [HttpDelete]
        [Route("{id}")]
        public Task<IActionResult> DeleteUserCard(int id = default)
        {
            throw null;
        }

        [HttpGet]
        [Route("cardNumber")]
        public Task<IActionResult> ReadCardNumber(string brandName = default, int deviceId = default)
        {
            throw null;
        }
    }
}
