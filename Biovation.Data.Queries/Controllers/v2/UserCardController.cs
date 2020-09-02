using Biovation.Domain;
using Biovation.Repository.SQL.v2;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Biovation.Data.Queries.Controllers.v2
{
    [Route("biovation/api/v2/[controller]")]
    //[ApiVersion("2.0")]
    public class UserCardController : Controller
    {

        private readonly UserCardRepository _userCardRepository;

        public UserCardController(UserCardRepository userCardRepository)
        {
            _userCardRepository = userCardRepository;
        }


        [HttpGet]
        public Task<ResultViewModel<PagingResult<UserCard>>> GetCardsByFilter(long userId, bool isActive, int pageNumber = default, int pageSize = default)
        {
            return Task.Run(() => _userCardRepository.GetCardsByFilter(userId, isActive, pageNumber, pageSize));
        }


        [HttpGet]
        public Task<ResultViewModel<User>> FindUserByCardNumber(string cardNumber)
        {
            return Task.Run(() => _userCardRepository.FindUserByCardNumber(cardNumber));
        }


    }
}
