using System;
using System.Collections.Generic;
using Biovation.Domain;
using Biovation.Services.RelayController.Common;

namespace Biovation.Services.RelayController.Services
{
    public class GetRelayService
    {
        public Dictionary<int,IRelay> ConnectedRelays { get; set; }

        public GetRelayService(Dictionary<int, IRelay> connectedRelays)
        {
            ConnectedRelays = connectedRelays;
        }

        public ResultViewModel<IRelay> GetRelay(int relayId)
        {
            try
            {
                if (ConnectedRelays.ContainsKey(relayId))
                {
                    var relay = ConnectedRelays[relayId];

                    if (relay.IsConnected())
                    {
                        return new ResultViewModel<IRelay>{ Validate = 0, Success = true , Message = $"The relay: {relayId} is connected.", Data = relay , Code = 1 , Id = relayId };
                    }
                    else return new ResultViewModel<IRelay> { Validate = 0, Success = false, Message = $"The relay: {relayId} is not connected!", Data = null, Code = 1, Id = relayId };
                }
                else return new ResultViewModel<IRelay> { Validate = 0, Success = false, Message = $"The relay: {relayId} is not connected!", Data = null, Code = 1, Id = relayId };
            }
            catch (Exception)
            {
                return new ResultViewModel<IRelay> { Validate = 0, Success = false, Message = $"The relay: {relayId} is not connected!", Data = null, Code = 1, Id = relayId };
            }
        }
    }
}