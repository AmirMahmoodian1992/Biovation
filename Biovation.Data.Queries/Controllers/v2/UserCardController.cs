using Biovation.Domain;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Biovation.Repository.Sql.v2;

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
        [Route("UserByCardNumber")]
        public Task<ResultViewModel<User>> FindUserByCardNumber(string cardNumber)
        {
            return Task.Run(() => _userCardRepository.FindUserByCardNumber(cardNumber));
        }


    }
}
