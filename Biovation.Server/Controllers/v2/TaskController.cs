using System.Threading.Tasks;
using Biovation.Service;
using Microsoft.AspNetCore.Mvc;

namespace Biovation.Server.Controllers.v2
{
    [Route("biovation/api/v{version:apiVersion}/[controller]")]
    [ApiVersion("2.0")]
    public class TaskController : Controller
    {
        private readonly TaskService _taskService;

        public TaskController(TaskService taskService)
        {
            _taskService = taskService;
        }

        [HttpPatch]
        public Task<IActionResult> TaskExecutionStatus(int taskItemId = default, string taskStatusId = default)
        {
            throw null;
        }
    }
}