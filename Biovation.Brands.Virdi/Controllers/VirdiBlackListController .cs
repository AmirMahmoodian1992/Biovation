using Biovation.Domain;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

using System.Threading.Tasks;

namespace Biovation.Brands.Virdi.Controllers
{
    [ApiController]
    [Route("Biovation/Api/[controller]/[action]")]
    public class VirdiBlackListController : ControllerBase
    {

        [HttpPost]
        [Authorize]
        public Task<List<ResultViewModel>> SendBlackLisDevice(List<BlackList> blackLists)
        {
            return Task.Run(() =>
            {
                var resultList = new List<ResultViewModel>();

                //var creatorUser = _userService.GetUsers(123456789).FirstOrDefault();
                resultList.Add(new ResultViewModel {Message = "Sending BlackList queued", Validate = 1});

                return resultList;
            });
        }
    }
}
