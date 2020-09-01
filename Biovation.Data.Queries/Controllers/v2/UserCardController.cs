using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Biovation.CommonClasses.Manager;
using Biovation.Domain;
using Biovation.Repository.SQL.v2;
using Biovation.Service;
using Microsoft.AspNetCore.Mvc;
using RestSharp;

namespace Biovation.Data.Queries.Controllers.v2
{
    [Route("biovation/api/v{version:apiVersion}/[controller]")]
    //[ApiVersion("2.0")]
    public class UserCardController : Controller
    {
 
        private readonly UserCardRepository _userCardRepository;

        public UserCardController(UserCardRepository userCardRepository)
        {
            _userCardRepository = userCardRepository;
        }

        [HttpGet]
        [Route("{id}")]
        public Task<IActionResult> GetUserCard(int id = default)
        {
            throw null;
        }

        public Task<ResultViewModel<PagingResult<UserCard>>> GetCardsByFilter(long userId, bool isactive, int pageNumber = default, int PageSize = default)
        {
            return Task.Run(() => _userCardRepository.GetCardsByFilter(userId,isactive,pageNumber,PageSize));
        }



        public Task<ResultViewModel<User>> FindUserByCardNumber(string cardNumber)
        {
            return Task.Run(() => _userCardRepository.FindUserByCardNumber(cardNumber));
        }


    }
}
