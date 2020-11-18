using System.Threading.Tasks;
using Biovation.Domain;
using Biovation.Repository.Sql.v2;
using Microsoft.AspNetCore.Mvc;

namespace Biovation.Data.Commands.Controllers.v2
{
    [ApiController]
    [Route("biovation/api/v2/[controller]")]
    public class UserCardController : ControllerBase
    {
        private readonly UserCardRepository _userCardRepository;

        public UserCardController(UserCardRepository userCardRepository)
        {
            _userCardRepository = userCardRepository;
        }
        
        [HttpGet]
        [Authorize]
        public Task<ResultViewModel<PagingResult<UserCard>>> GetCardsByFilter(long userId, bool isactive, int pageNumber = default, int pageSize = default)
        {
            return Task.Run(() => _userCardRepository.GetCardsByFilter(userId, isactive, pageNumber, pageSize));
        }
        
        [HttpPost]
        [Authorize]
        public Task<ResultViewModel> AddUserCard([FromBody] UserCard card = default)
        {
            return Task.Run(() => _userCardRepository.AddUserCard(card));
        }

        [HttpPut]
        [Authorize]
        public Task<ResultViewModel> ModifyUserCard([FromBody] UserCard card = default)
        {
            return Task.Run(() => _userCardRepository.ModifyUserCard(card));
        }

        [HttpDelete]
        [Authorize]
        [Route("{id}")]
        public Task<ResultViewModel> DeleteUserCard(int id = default)
        {
            return Task.Run(() => _userCardRepository.DeleteUserCard(id));
        }
    }
}
