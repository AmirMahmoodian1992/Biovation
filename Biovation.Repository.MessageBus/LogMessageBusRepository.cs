using Biovation.CommonClasses;
using Biovation.CommonClasses.Manager;
using Biovation.Domain;
using Kasra.MessageBus.Domain.Enumerators;
using Kasra.MessageBus.Domain.Interfaces;
using Kasra.MessageBus.Infrastructure;
using Kasra.MessageBus.Managers.Sinks.EventBus;
using Kasra.MessageBus.Managers.Sinks.Internal;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Biovation.Repository.MessageBus
{
    public class LogMessageBusRepository
    {

        private readonly ISource<DataChangeMessage<Log>> _logInternalSource;
        private const string LogTopicName = "BiovationLogUpdateEvent";
      
        public LogMessageBusRepository(BiovationConfigurationManager biovationConfiguration)
        {
           
            var kafkaServerAddress = biovationConfiguration.KafkaServerAddress;
            _logInternalSource = InternalSourceBuilder.Start().SetPriorityLevel(PriorityLevel.Medium)
                .Build<DataChangeMessage<Log>>();

            var biovationKafkaTarget = KafkaTargetBuilder.Start().SetBootstrapServer(kafkaServerAddress).SetTopicName(LogTopicName)
                .BuildTarget<DataChangeMessage<Log>>();

            var biovationLogConnectorNode = new ConnectorNode<DataChangeMessage<Log>>(_logInternalSource, biovationKafkaTarget);
            biovationLogConnectorNode.StartProcess();
        }


        public Task<ResultViewModel> SendLog(List<Log >logList)
        {
            return Task.Run(() =>
            {
               
                try
                {
                    var biovationBrokerMessageData = new List<DataChangeMessage<Log>>
                    {
                        new DataChangeMessage<Log>
                        {
                            Id = Guid.NewGuid().ToString(), EventId = 1, SourceName = "BiovationCore",
                            TimeStamp = DateTimeOffset.Now, SourceDatabaseName = "biovation", Data = logList
                        }
                    };

                    _logInternalSource.PushData(biovationBrokerMessageData);
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