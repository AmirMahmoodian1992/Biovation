using System;
using Biovation.Constants;
using Biovation.Domain;
using CommandType = Biovation.Services.RelayController.Domain.CommandType;

namespace Biovation.Services.RelayController.Domain
{
    public class Criteria
    {
        private Lookup _relayType;
        private int _lastExecutedCommand;

        public Criteria(Lookup relayType, int lastExecutedCommand)
        {
            _relayType = relayType;
            _lastExecutedCommand = lastExecutedCommand;
        }

        public TimeSpan getCrucialTime()
        {
            if (_relayType.Code == relayTypes.HumanGateCode && _lastExecutedCommand == CommandType.Open )
                return    new TimeSpan(0, 0, 0,5);
            if (_relayType.Code == relayTypes.CarGateCode && _lastExecutedCommand == CommandType.Open)
                return new TimeSpan(0, 0, 0, 10);
            if (_relayType.Code == relayTypes.MultiUseGateCode && _lastExecutedCommand == CommandType.Open)
                return new TimeSpan(0, 0, 0, 15);
            if (_relayType.Code == relayTypes.FixedLightCode && _lastExecutedCommand == CommandType.Open)
                return new TimeSpan(0, 0, 0, 0);
            if (_relayType.Code == relayTypes.FlashedLightCode && _lastExecutedCommand == CommandType.Open)
                return new TimeSpan(0, 0, 0, 0);
            else
                return new TimeSpan(0,0,0,0);

        }

    }
}