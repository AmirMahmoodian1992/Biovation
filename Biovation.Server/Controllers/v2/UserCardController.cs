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
        public Task<IActionResult> AddUserCard([FromBody]UserCard card = default)
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
        public Task<ResultViewModel<PagingResult<UserCard>>> GetUserCard(long userId, bool isActive,
        int pageNumber = default, int pageSize = default)
        {
            return Task.Run(async () => { return _userCard.GetCardsByFilter(userId, isActive, pageNumber, pageSize); });
        }

        [HttpDelete]
        [Route("{id}")]
        public Task<IActionResult> DeleteUserCard(int id = default)
        {
            throw null;
        }

        [HttpGet]
        [Route("cardNumber/{deviceId}")]
        public Task<int> ReadCardNumber(string brandName = default, int deviceId = default)
        {
            return Task.Run(async () => { return _userCard.ReadCardNumber(brandName,deviceId); });
        }
    }
}
