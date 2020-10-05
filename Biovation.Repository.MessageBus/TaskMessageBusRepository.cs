using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Biovation.CommonClasses;
using Biovation.CommonClasses.Manager;
using Biovation.Domain;
using Kasra.MessageBus.Domain.Enumerators;
using Kasra.MessageBus.Domain.Interfaces;
using Kasra.MessageBus.Infrastructure;
using Kasra.MessageBus.Managers.Sinks.EventBus;
using Kasra.MessageBus.Managers.Sinks.Internal;

namespace Biovation.Repository.MessageBus
{
    public class TaskMessageBusRepository
    {

        private readonly ISource<DataChangeMessage<TaskInfo>> _taskInternalSource;
        private const string TaskTopicName = "BiovationTaskUpdateEvent";
     
        public TaskMessageBusRepository(BiovationConfigurationManager biovationConfiguration)
        {
           
            var kafkaServerAddress = biovationConfiguration.KafkaServerAddress;
            _taskInternalSource = InternalSourceBuilder.Start().SetPriorityLevel(PriorityLevel.Medium)
                .Build<DataChangeMessage<TaskInfo>>();

            var biovationKafkaTarget = KafkaTargetBuilder.Start().SetBootstrapServer(kafkaServerAddress).SetTopicName(TaskTopicName)
                .BuildTarget<DataChangeMessage<TaskInfo>>();

            var biovationTaskConnectorNode = new ConnectorNode<DataChangeMessage<TaskInfo>>(_taskInternalSource, biovationKafkaTarget);
            biovationTaskConnectorNode.StartProcess();
        }


        public Task<ResultViewModel> SendTask(List<TaskInfo> taskList)
        {
            return Task.Run(() =>
            {
                
                try
                {
                    var biovationBrokerMessageData = new List<DataChangeMessage<TaskInfo>>
                    {
                        new DataChangeMessage<TaskInfo>
                        {
                            Id = Guid.NewGuid().ToString(), EventId = 1, SourceName = "BiovationCore",
                            TimeStamp = DateTimeOffset.Now, SourceDatabaseName = "biovation", Data = taskList
                        }
                    };

                    _taskInternalSource.PushData(biovationBrokerMessageData);
                    return new ResultViewModel { Validate = 1 };

                }
                catch (Exception exception)
                {
                    Logger.Log(exception);
                    return new ResultViewModel { Validate = 0, Message = exception.ToString() };

                }
            });
        }
    }
}