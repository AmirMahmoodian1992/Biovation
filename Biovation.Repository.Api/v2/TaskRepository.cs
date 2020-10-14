using Biovation.CommonClasses.Manager;
using Biovation.Domain;
using RestSharp;

namespace Biovation.Repository.Api.v2
{
    public class TaskRepository
    {
        private readonly RestClient _restClient;
        private readonly BiovationConfigurationManager _biovationConfigurationManager;
        public TaskRepository(RestClient restClient, BiovationConfigurationManager biovationConfigurationManager)
        {
            _restClient = restClient;
            _biovationConfigurationManager = biovationConfigurationManager;
        }

        public ResultViewModel<PagingResult<TaskInfo>> GetTasks(int taskId = default, string brandCode = default,
            int deviceId = default, string taskTypeCode = default, string taskStatusCodes = default,
            string excludedTaskStatusCodes = default, int pageNumber = default,
            int pageSize = default, int taskItemId = default, string token =default)
        {
            var restRequest = new RestRequest("Queries/v2/task", Method.GET);
            restRequest.AddQueryParameter("taskId", taskId.ToString());
            restRequest.AddQueryParameter("brandCode", brandCode ?? string.Empty);
            restRequest.AddQueryParameter("deviceId", deviceId.ToString());
            restRequest.AddQueryParameter("taskTypeCode", taskTypeCode ?? string.Empty);
            restRequest.AddQueryParameter("taskStatusCodes", taskStatusCodes ?? string.Empty);
            restRequest.AddQueryParameter("excludedTaskStatusCodes", excludedTaskStatusCodes ?? string.Empty);
            restRequest.AddQueryParameter("pageNumber", pageNumber.ToString());
            restRequest.AddQueryParameter("pageSize", pageSize.ToString());
            restRequest.AddQueryParameter("taskItemId", taskItemId.ToString());
            token ??= _biovationConfigurationManager.DefaultToken();
            restRequest.AddHeader("Authorization", token);
            var requestResult = _restClient.ExecuteAsync<ResultViewModel<PagingResult<TaskInfo>>>(restRequest);
            return requestResult.Result.Data;
        }

        public ResultViewModel<TaskItem> GetTaskItem(int taskItemId = default, string token = default)
        {
            var restRequest = new RestRequest("Queries/v2/task/{taskItemId}", Method.GET);
            restRequest.AddUrlSegment("taskItemId", taskItemId.ToString());
            token ??= _biovationConfigurationManager.DefaultToken();
            restRequest.AddHeader("Authorization", token);
            var requestResult = _restClient.ExecuteAsync<ResultViewModel<TaskItem>>(restRequest);
            return requestResult.Result.Data;
        }

        public ResultViewModel InsertTask( TaskInfo task)
        {
            var restRequest = new RestRequest("Commands/v2/task", Method.POST);
            restRequest.AddJsonBody(task);
            var requestResult = _restClient.ExecuteAsync<ResultViewModel>(restRequest);
            return requestResult.Result.Data;
        }

        public ResultViewModel UpdateTaskStatus(TaskItem taskItem)
        {
            var restRequest = new RestRequest("Commands/v2/task", Method.PUT);
            restRequest.AddJsonBody(taskItem);
            var requestResult = _restClient.ExecuteAsync<ResultViewModel>(restRequest);
            return requestResult.Result.Data;
        }


    }
}