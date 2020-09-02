using System.Threading.Tasks;
using Biovation.Domain;
using Biovation.Repository.SQL.v2;
using Biovation.Service;
using Microsoft.AspNetCore.Mvc;

namespace Biovation.Data.Queries.Controllers.v2
{

    [Route("biovation/api/v{version:apiVersion}/[controller]")]
    //[ApiVersion("2.0")]
    public class PlateDetectionController : Controller
    {

            private readonly PlateDetectionRepository _plateDetectionRepository;

            public PlateDetectionController(PlateDetectionRepository PlateDetectionRepository)
            {
                _plateDetectionRepository = PlateDetectionRepository;
            }

        [HttpPost]
        public Task<IActionResult> AddLicensePlate(LicensePlate licensePlate = default)
        {
            throw null;
        }


    }
}