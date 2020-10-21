using Biovation.CommonClasses.Manager;
using Biovation.Domain;
using Biovation.Repository.Api.v2;
using Kasra.MessageBus.Domain.Enumerators;
using Kasra.MessageBus.Domain.Interfaces;
using Kasra.MessageBus.Infrastructure;
using Kasra.MessageBus.Managers.Sinks.EventBus;
using Kasra.MessageBus.Managers.Sinks.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Biovation.Service.Api.v1
{
    public class TaskService
    {
        private readonly TaskRepository _taskRepository;

        private ISource<DataChangeMessage<TaskInfo>> _biovationInternalSource;
        private ConnectorNode<DataChangeMessage<TaskInfo>> _biovationTaskConnectorNode;
        private const string _biovationTopicName = "BiovationTaskStatusUpdateEvent";

        public TaskService(TaskRepository taskRepository, BiovationConfigurationManager BiovationConfiguration)
        {
            _taskRepository = taskRepository;

            var kafkaServerAddress = BiovationConfiguration.KafkaServerAddress;
            _biovationInternalSource = InternalSourceBuilder.Start().SetPriorityLevel(PriorityLevel.Medium)
               .Build<DataChangeMessage<TaskInfo>>();

            var biovationKafkaTarget = KafkaTargetBuilder.Start().SetBootstrapServer(kafkaServerAddress).SetTopicName(_biovationTopicName)
                .BuildTarget<DataChangeMessage<TaskInfo>>();

            _biovationTaskConnectorNode = new ConnectorNode<DataChangeMessage<TaskInfo>>(_biovationInternalSource, biovationKafkaTarget);
            _biovationTaskConnectorNode.StartProcess();
        }

        public Task<List<TaskInfo>> GetTasks(int taskId = default, string brandCode = default,
            int deviceId = default, string taskTypeCode = default, string taskStatusCodes = default,
            string excludedTaskStatusCodes = default, int pageNumber = default,
            int pageSize = default, int taskItemId = default, string token = default)
        {
            return Task.Run(() => _taskRepository.GetTasks(taskId, brandCode, deviceId, taskTypeCode, taskStatusCodes,
                                      excludedTaskStatusCodes, pageNumber, pageSize,taskItemId, token)?.Data?.Data ?? new List<TaskInfo>());
        }

        public Task<List<TaskInfo>> GetTasks(int taskId = default, string brandCode = default,
            int deviceId = default, string taskTypeCode = default, string taskStatusCodes = default,
            List<string> excludedTaskStatusCodes = default, int pageNumber = default,
            int pageSize = default,int taskItemId = default, string token = default)
        {
            return Task.Run(() =>
            {
                var excludedTaskStatusCodesString = string.Empty;
                if (excludedTaskStatusCodes != null)
                {
                    excludedTaskStatusCodesString += '(';
                    foreach (var excludedTaskStatusCode in excludedTaskStatusCodes)
                    {
                        excludedTaskStatusCodesString += $"{excludedTaskStatusCode},";
                    }

                    excludedTaskStatusCodesString = excludedTaskStatusCodesString.Trim(',');
                    excludedTaskStatusCodesString += ')';
                }

                return _taskRepository.GetTasks(taskId, brandCode, deviceId, taskTypeCode, taskStatusCodes,
                           excludedTaskStatusCodesString, pageNumber, pageSize, taskItemId, token)?.Data?.Data ?? new List<TaskInfo>();
            });
        }

        public TaskItem GetTaskItem(int taskItemId = default, string token = default)
        {
            return _taskRepository.GetTaskItem(taskItemId, token)?.Data ?? new TaskItem();
        }

        /*public ResultViewModel InsertTask(TaskInfo task)
        {
            var taskInsertionResult = _taskRepository.InsertTask(task);
            if (taskInsertionResult.Success)
            {
                //integration
                Task.Run(() =>
                {
                    task.Id = (int) taskInsertionResult.Id;
                    var taskList = new List<TaskInfo> { task };

                    var biovationBrokerMessageData = new List<DataChangeMessage<TaskInfo>>
                    {
                        new DataChangeMessage<TaskInfo>
                        {
                            Id = Guid.NewGuid().ToString(), EventId = 1, SourceName = "BiovationCore",
                            TimeStamp = DateTimeOffset.Now, SourceDatabaseName = "biovation", Data = taskList
                        }
                    };

                    _biovationInternalSource.PushData(biovationBrokerMessageData);

                });
            }

            return taskInsertionResult;
        }*/
        public ResultViewModel InsertTask(TaskInfo task, string token = default)
        {
            return _taskRepository.InsertTask(task);
        }

        public ResultViewModel UpdateTaskStatus(TaskItem taskItem, string token = default)
        {
            return _taskRepository.UpdateTaskStatus(taskItem);
        }
    }
}
