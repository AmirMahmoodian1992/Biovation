using Biovation.CommonClasses;
using Biovation.CommonClasses.Manager;
using Biovation.Domain;
using RestSharp;
using System;
using System.Threading.Tasks;

namespace Biovation.Data.Commands.Sinks
{
    public class TaskApiSink
    {
        private readonly RestClient _restClient;
        private readonly BiovationConfigurationManager _biovationConfigurationManager;
        public TaskApiSink(BiovationConfigurationManager biovationConfigurationManager)
        {
            _biovationConfigurationManager = biovationConfigurationManager;
            _restClient = (RestClient)new RestClient(biovationConfigurationManager.LogMonitoringApiUrl).UseSerializer(() => new RestRequestJsonSerializer());
        }

        public async Task<ResultViewModel> TransmitTaskInfo(TaskInfo taskInfo)
        {
            if (!_biovationConfigurationManager.BroadcastToApi)
                return new ResultViewModel { Success = false, Message = "The Api broadcast option is off" };

            try
            {
                try
                {
                    // صفحه مدیریت عملیات
                    var restRequest = new RestRequest("UpdateTaskStatus/UpdateTask", Method.POST);
                    restRequest.AddJsonBody(taskInfo);
                    await _restClient.ExecuteAsync(restRequest);
                }
                catch (Exception exception)
                {
                    Logger.Log(exception);
                }

                return new ResultViewModel { Success = true, Message = "Tasks are sent successfully." };
            }
            catch (Exception exception)
            {
                Logger.Log(exception);
                return new ResultViewModel { Success = false, Message = exception.Message };
            }
        }
    }
}
