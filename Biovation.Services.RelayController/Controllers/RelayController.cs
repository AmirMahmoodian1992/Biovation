using System;
using System.Linq;
using Biovation.CommonClasses;
using Biovation.Constants;
using Biovation.Domain;
using Microsoft.AspNetCore.Mvc;
using Biovation.Services.RelayController.Commands;
using Biovation.Services.RelayController.Services;
using Newtonsoft.Json.Linq;
using Serilog;
using CommandType = Biovation.Services.RelayController.Domain.CommandType;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Biovation.Services.RelayController.Controllers
{
    [Route("biovation/api/[controller]")]

    [ApiController]
    public class RelayController : ControllerBase
    {
        private readonly Lookups _lookups;
        private readonly CommandFactory _commandFactory;
        private readonly ILogger _logger;

        public RelayController(GetRelayService tcpClientGetter, Lookups lookups, ILogger logger)
        {
            _lookups = lookups;
            _commandFactory = new CommandFactory(tcpClientGetter);

            _logger = logger.ForContext<RelayController>();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="relayId"></param>
        /// <returns></returns>
        [Route("Contact/{relayId:int}")]
        [HttpPost]
        public ResultViewModel Contact([FromRoute] int relayId, string messagePriority = "13003")
        {
            
            try
            {
                var priority = _lookups.TaskPriorities.FirstOrDefault(tp =>
                    string.Equals(tp.Code, messagePriority, StringComparison.InvariantCultureIgnoreCase));
                 //_logger.Debug("Log for relay {relayId}", relayId);
                var command = _commandFactory.Factory(CommandType.Contact, relayId);
                var result = command.Data.Execute(priority);
                return result;
            }
            catch (Exception e)
            {
                return new ResultViewModel{Message = e.Message, Code = 500};
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="relayId"></param>
        /// <param name="messagePriority"></param>
        /// <returns></returns>
        [Route("TurnOn/{relayId:int}")]
        [HttpPost]
        public ResultViewModel TurnOn([FromRoute] int relayId, string messagePriority = "13003")
        {
            try
            {
                var priority = _lookups.TaskPriorities.FirstOrDefault(tp =>
                    string.Equals(tp.Code, messagePriority, StringComparison.InvariantCultureIgnoreCase));
                
                var command = _commandFactory.Factory(CommandType.TurnOn, relayId);
                var result = command.Data.Execute(priority);
                return result;
            }
            catch (Exception e)
            {
                return new ResultViewModel { Message = e.Message, Code = 500 };
            }
        }

        [Route("TurnOff/{relayId:int}")]
        [HttpPost]
        public ResultViewModel TurnOff([FromRoute] int relayId, string messagePriority = "13003")
        {
            try
            {
                var priority = _lookups.TaskPriorities.FirstOrDefault(tp =>
                    string.Equals(tp.Code, messagePriority, StringComparison.InvariantCultureIgnoreCase));

                var command = _commandFactory.Factory(CommandType.TurnOff, relayId);
                var result = command.Data.Execute(priority);
                return result;
            }
            catch (Exception e)
            {
                return new ResultViewModel { Message = e.Message, Code = 500 };
            }
        }

        [Route("FlashOn/{relayId:int}")]
        [HttpPost]
        public ResultViewModel FlashOn([FromRoute] int relayId, string messagePriority = "13003")
        {
            try
            {
                var priority = _lookups.TaskPriorities.FirstOrDefault(tp =>
                    string.Equals(tp.Code, messagePriority, StringComparison.InvariantCultureIgnoreCase));

                var command = _commandFactory.Factory(CommandType.FlashOn, relayId);
                var result = command.Data.Execute(priority);
                return result;
            }
            catch (Exception e)
            {
                return new ResultViewModel { Message = e.Message, Code = 500 };
            }
        }

        [Route("FlashOff/{relayId:int}")]
        [HttpPost]
        public ResultViewModel FlashOff([FromRoute] int relayId, string messagePriority = "13003")
        {
            try
            {
                var priority = _lookups.TaskPriorities.FirstOrDefault(tp =>
                    string.Equals(tp.Code, messagePriority, StringComparison.InvariantCultureIgnoreCase));

                var command = _commandFactory.Factory(CommandType.FlashOff, relayId);
                var result = command.Data.Execute(priority);
                return result;
            }
            catch (Exception e)
            {
                return new ResultViewModel { Message = e.Message, Code = 500 };
            }
        }
    }
}
