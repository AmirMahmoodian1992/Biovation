using System.Threading.Tasks;
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

        public async Task<ResultViewModel<PagingResult<TaskInfo>>> GetTasks(int taskId = default, string brandCode = default,
            string instanceId = default, int deviceId = default, string taskTypeCode = default, string taskStatusCodes = default,
            string excludedTaskStatusCodes = default, int pageNumber = default,
            int pageSize = default, int taskItemId = default, string token =default)
        {
            var restRequest = new RestRequest("Queries/v2/task", Method.GET);
            restRequest.AddQueryParameter("taskId", taskId.ToString());
            restRequest.AddQueryParameter("brandCode", brandCode ?? string.Empty);
            restRequest.AddQueryParameter("instanceId", instanceId ?? string.Empty);
            restRequest.AddQueryParameter("deviceId", deviceId.ToString());
            restRequest.AddQueryParameter("taskTypeCode", taskTypeCode ?? string.Empty);
            restRequest.AddQueryParameter("taskStatusCodes", taskStatusCodes ?? string.Empty);
            restRequest.AddQueryParameter("excludedTaskStatusCodes", excludedTaskStatusCodes ?? string.Empty);
            restRequest.AddQueryParameter("pageNumber", pageNumber.ToString());
            restRequest.AddQueryParameter("pageSize", pageSize.ToString());
            restRequest.AddQueryParameter("taskItemId", taskItemId.ToString());
            token ??= _biovationConfigurationManager.DefaultToken;
            restRequest.AddHeader("Authorization", token);
            var requestResult = await _restClient.ExecuteAsync<ResultViewModel<PagingResult<TaskInfo>>>(restRequest);
            return requestResult.Data;
        }

        public async Task<ResultViewModel<TaskItem>> GetTaskItem(int taskItemId = default, string token = default)
        {
            var restRequest = new RestRequest("Queries/v2/task/{taskItemId}", Method.GET);
            restRequest.AddUrlSegment("taskItemId", taskItemId.ToString());
            token ??= _biovationConfigurationManager.DefaultToken;
            restRequest.AddHeader("Authorization", token);
            var requestResult = await _restClient.ExecuteAsync<ResultViewModel<TaskItem>>(restRequest);
            return requestResult.Data;
        }

        public async Task<ResultViewModel> InsertTask( TaskInfo task, string token = default)
        {
            var restRequest = new RestRequest("Commands/v2/task", Method.POST);
            restRequest.AddJsonBody(task);
            token ??= _biovationConfigurationManager.DefaultToken;
            restRequest.AddHeader("Authorization", token);
            var requestResult = await _restClient.ExecuteAsync<ResultViewModel>(restRequest);
            return requestResult.Data;
        }

        public async Task<ResultViewModel> UpdateTaskStatus(TaskItem taskItem, string token = default)
        {
            var restRequest = new RestRequest("Commands/v2/task", Method.PUT);
            restRequest.AddJsonBody(taskItem);
            token ??= _biovationConfigurationManager.DefaultToken;
            restRequest.AddHeader("Authorization", token); 
            var requestResult = await _restClient.ExecuteAsync<ResultViewModel>(restRequest);
            return requestResult.Data;
        }

        public async Task<ResultViewModel> ProcessQueue(Lookup brand, int deviceId = default, string token = default)
        {
            var restRequest = new RestRequest($"{brand.Name}/{brand.Name}Task/RunProcessQueue", Method.POST);
            restRequest.AddQueryParameter("deviceId", deviceId.ToString());
            token ??= _biovationConfigurationManager.DefaultToken;
            restRequest.AddHeader("Authorization", token); var requestResult = _restClient.ExecuteAsync<ResultViewModel>(restRequest);
            return (await requestResult).Data;
        }
    }
}