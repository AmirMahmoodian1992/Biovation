using Biovation.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Biovation.Service.Api.v2;
using Biovation.CommonClasses.Interface;
using Biovation.Domain;

namespace Biovation.Brands.Paliz.Command
{
    public class CommandFactory
    {
        private readonly PalizServer _palizServer;
        private readonly LogEvents _logEvents;
        private readonly LogService _logService;
        private readonly LogSubEvents _logSubEvents;
        private readonly MatchingTypes _matchingTypes;
        private readonly FingerTemplateTypes _fingerTemplateTypes;

        public CommandFactory(PalizServer virdiServer, LogService logService, LogEvents logEvents, LogSubEvents logSubEvents)
        {
            _palizServer = virdiServer;
            _logService = logService;
            _logEvents = logEvents;
            _logSubEvents = logSubEvents;
        }

        public ICommand Factory(int eventId, List<object> items)
        {
            return Factory(new DataTransferModel { EventId = eventId, Items = items });
        }

        /// <summary>
        /// <En>Create and return an instance of requested event handler.</En>
        /// <Fa>بر اساس نوع درخواست، یک نمونه از کنترل کننده درخواست می سازد.</Fa>
        /// </summary>
        /// <param name="transferModelData">داده ی دریافتی از کلاینت، بانک، sdk و یا...</param>
        /// <returns></returns>
        public ICommand Factory(DataTransferModel transferModelData)
        {
            switch (transferModelData.EventId)
            {
                case CommandType.PersonnelEvent:
                    //Change in Personnel
                    throw new NotImplementedException();

                case CommandType.GuestEvent:
                    //Guest request
                    throw new NotImplementedException();

                case CommandType.ServerEventLogForceUpdate:
                    //Force Update from Server_Event_Log request
                    throw new NotImplementedException();

                case CommandType.ForceUpdateForSpecificDevice:
                    //Force Update for Specific Device request
                    throw new NotImplementedException();

                case CommandType.SyncAllUsers:
                    //Sync Update request
                    throw new NotImplementedException();

                case CommandType.SetTime:
                    //Update time in all devices
                    //var timeToSet = Convert.ToInt32(transferModelData.Items.FirstOrDefault());
                    throw new NotImplementedException();

                case CommandType.EnrollFromTerminal:
                    //Unlocks the device
                    {
                        //var code = Convert.ToUInt32(transferModelData.Items[0]);
                        //return new VirdiEnrollFromTerminal(transferModelData.Items, VirdiServer.GetOnlineDevices());
                        throw new NotImplementedException();
                    }

                case CommandType.GetUsersOfDevice:
                    //Gets users of devices
                    throw new NotImplementedException();

                case CommandType.GetOnlineDevices:
                    //Gets online devices
                    throw new NotImplementedException();

                default:
                    return null;
            }
        }
    }
}
