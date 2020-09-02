using System.Threading.Tasks;
using Biovation.Domain;
using Biovation.Repository.SQL.v2;
using Microsoft.AspNetCore.Mvc;

namespace Biovation.Data.Commands.Controllers.v2
{
    [Route("biovation/api/commands/v2/[controller]")]
    //[ApiVersion("2.0")]
    public class TaskController : Controller
    {

        private readonly TaskRepository _taskRepository;

        public TaskController(TaskRepository taskRepository)
        {
            _taskRepository = taskRepository;
        }

        [HttpPost]
        [Route("InsertTask")]
        public Task<ResultViewModel> InsertTask(TaskInfo task)
        {
            return Task.Run(() => _taskRepository.InsertTask(task));
        }

        [HttpPut]
        [Route("UpdateTaskStatus")]
        public Task<ResultViewModel> UpdateTaskStatus(TaskItem taskItem)
        {
            return Task.Run(() => _taskRepository.UpdateTaskStatus(taskItem));
        }


        
    }
}