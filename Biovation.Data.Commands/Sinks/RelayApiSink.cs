using Biovation.CommonClasses;
using Biovation.CommonClasses.Manager;
using Biovation.Domain;
using Biovation.Repository.Sql.v2.RelayController;
using Biovation.Service.Api.v2.RelayController;
using System;
using System.Threading.Tasks;

namespace Biovation.Data.Commands.Sinks
{
    public class RelayApiSink
    {
        private readonly RelayService _relayApiService;
        private readonly RelayRepository _relayRepository;
        private readonly BiovationConfigurationManager _biovationConfigurationManager;
        public RelayApiSink(BiovationConfigurationManager biovationConfigurationManager, RelayService relayApiService, RelayRepository relayRepository)
        {
            _biovationConfigurationManager = biovationConfigurationManager;
            _relayApiService = relayApiService;
            _relayRepository = relayRepository;
        }

        public Task<ResultViewModel> OpenRelays(Log log)
        {
            return Task.Run(() =>
            {
                //if (!_biovationConfigurationManager.BroadcastToApi)
                //    return new ResultViewModel { Success = false, Message = "The Api broadcast option is off" };

                try
                {
                    if (log.EventLog.Code == "16003" || log.EventLog.Code == "16004")
                    {
                        var relays = _relayRepository.GetRelay(deviceId: log.DeviceId)?.Data?.Data;
                        if (relays != null)
                        {
                            foreach (var relay in relays)
                            {
                                _ = _relayApiService.OpenRelay(relay.Id).ConfigureAwait(false);
                            }
                        }
                    }
                }
                catch (Exception exception)
                {
                    Logger.Log(exception);
                }

                return new ResultViewModel { Validate = 1 };
            });
        }
    }
}
