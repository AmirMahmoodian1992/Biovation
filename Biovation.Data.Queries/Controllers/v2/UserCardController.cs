using Biovation.Domain;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Biovation.Repository.Sql.v2;

namespace Biovation.Data.Queries.Controllers.v2
{
    [ApiController]
    [Route("biovation/api/v2/[controller]")]
    //[ApiVersion("2.0")]
    public class UserCardController : ControllerBase
    {
        private readonly UserCardRepository _userCardRepository;

        public UserCardController(UserCardRepository userCardRepository)
        {
            _userCardRepository = userCardRepository;
        }

        [HttpGet]
        [Authorize]
        public Task<ResultViewModel<PagingResult<UserCard>>> GetCardsByFilter(long userId, bool isActive, int pageNumber = default, int pageSize = default)
        {
            return Task.Run(() => _userCardRepository.GetCardsByFilter(userId, isActive, pageNumber, pageSize));
        }


        [HttpGet]
        [Authorize]
        [Route("UserByCardNumber")]
        public Task<ResultViewModel<User>> FindUserByCardNumber(string cardNumber)
        {
            return Task.Run(() => _userCardRepository.FindUserByCardNumber(cardNumber));
        }


    }
}
