using System.Threading.Tasks;
using Biovation.Domain;
using Biovation.Repository.SQL.v2;
using Microsoft.AspNetCore.Mvc;

namespace Biovation.Data.Commands.Controllers.v2
{
    [Route("biovation/api/v2/[controller]")]
    //[ApiVersion("2.0")]
    public class TaskController : Controller
    {

        private readonly TaskRepository _taskRepository;

        public TaskController(TaskRepository taskRepository)
        {
            _taskRepository = taskRepository;
        }

        [HttpPost]
        public Task<ResultViewModel> InsertTask([FromBody]TaskInfo task)
        {
            return Task.Run(() => _taskRepository.InsertTask(task));
        }

        [HttpPut]
        public Task<ResultViewModel> UpdateTaskStatus([FromBody]TaskItem taskItem)
        {
            return Task.Run(() => _taskRepository.UpdateTaskStatus(taskItem));
        }

    }
}