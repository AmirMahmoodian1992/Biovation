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
    [Route("biovation/api/v{version:apiVersion}/[controller]")]
    [ApiVersion("2.0")]
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
        public Task<IActionResult> MAddUserCard([FromBody]UserCard card = default)
        {
            throw null;
        }

        [HttpPut]
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
        [Route("cardNumber/{deviceId}")]
        public Task<IActionResult> ReadCardNumber(string brandName = default, int deviceId = default)
        {
            throw null;
        }
    }
}
