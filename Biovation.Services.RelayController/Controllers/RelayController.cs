using Biovation.Constants;
using Biovation.Domain;
using Biovation.Services.RelayController.Commands;
using Biovation.Services.RelayController.Services;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using System;
using System.Linq;
using CommandType = Biovation.Services.RelayController.Domain.CommandType;

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

        [Route("{relayId:int}/Open")]
        [HttpPost]
        public ResultViewModel Open([FromRoute] int relayId, string messagePriority = "13003")
        {
            try
            {
                var priority = _lookups.TaskPriorities.FirstOrDefault(tp =>
                    string.Equals(tp.Code, messagePriority, StringComparison.InvariantCultureIgnoreCase));

                var command = _commandFactory.Factory(CommandType.Open, relayId);

                if (!command.Success)
                    return new ResultViewModel{Success = false, Message = command.Message, Id = 1, Code = 400};

                var result = command.Data.Execute(priority);
                return result;
            }
            catch (Exception e)
            {
                return new ResultViewModel { Message = e.Message, Code = 500 };
            }
        }

        [Route("{relayId:int}/Close")]
        [HttpPost]
        public ResultViewModel Close([FromRoute] int relayId, string messagePriority = "13003")
        {
            try
            {
                var priority = _lookups.TaskPriorities.FirstOrDefault(tp =>
                    string.Equals(tp.Code, messagePriority, StringComparison.InvariantCultureIgnoreCase));

                var command = _commandFactory.Factory(CommandType.TurnOff, relayId);
                if (!command.Success)
                    return new ResultViewModel { Success = false, Message = command.Message, Id = 1, Code = 400 };

                var result = command.Data.Execute(priority);
                return result;
            }
            catch (Exception e)
            {
                return new ResultViewModel { Message = e.Message, Code = 500 };
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="relayId"></param>
        /// <param name="messagePriority"></param>
        /// <returns></returns>
        [Route("{relayId:int}/Contact")]
        [HttpPost]
        public ResultViewModel Contact([FromRoute] int relayId, string messagePriority = "13003")
        {
            try
            {
                _logger.Debug("Requesting relay contacting for relay {relayId} with priority {priorityCode}", relayId, messagePriority);
                var priority = _lookups.TaskPriorities.FirstOrDefault(tp =>
                    string.Equals(tp.Code, messagePriority, StringComparison.InvariantCultureIgnoreCase));
                var command = _commandFactory.Factory(CommandType.Contact, relayId);
                if (!command.Success)
                    return new ResultViewModel { Success = false, Message = command.Message, Id = 1, Code = 400 };

                var result = command.Data.Execute(priority);
                return result;
            }
            catch (Exception e)
            {
                return new ResultViewModel { Message = e.Message, Code = 500 };
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="relayId"></param>
        /// <param name="messagePriority"></param>
        /// <returns></returns>
        [Route("{relayId:int}/TurnOn")]
        [HttpPost]
        public ResultViewModel TurnOn([FromRoute] int relayId, string messagePriority = "13003")
        {
            try
            {
                var priority = _lookups.TaskPriorities.FirstOrDefault(tp =>
                    string.Equals(tp.Code, messagePriority, StringComparison.InvariantCultureIgnoreCase));

                var command = _commandFactory.Factory(CommandType.TurnOn, relayId);
                if (!command.Success)
                    return new ResultViewModel { Success = false, Message = command.Message, Id = 1, Code = 400 };

                var result = command.Data.Execute(priority);
                return result;
            }
            catch (Exception e)
            {
                return new ResultViewModel { Message = e.Message, Code = 500 };
            }
        }

        [Route("{relayId:int}/TurnOff")]
        [HttpPost]
        public ResultViewModel TurnOff([FromRoute] int relayId, string messagePriority = "13003")
        {
            try
            {
                var priority = _lookups.TaskPriorities.FirstOrDefault(tp =>
                    string.Equals(tp.Code, messagePriority, StringComparison.InvariantCultureIgnoreCase));

                var command = _commandFactory.Factory(CommandType.TurnOff, relayId);
                if (!command.Success)
                    return new ResultViewModel { Success = false, Message = command.Message, Id = 1, Code = 400 };

                var result = command.Data.Execute(priority);
                return result;
            }
            catch (Exception e)
            {
                return new ResultViewModel { Message = e.Message, Code = 500 };
            }
        }

        [Route("{relayId:int}/FlashOn")]
        [HttpPost]
        public ResultViewModel FlashOn([FromRoute] int relayId, string messagePriority = "13003")
        {
            try
            {
                var priority = _lookups.TaskPriorities.FirstOrDefault(tp =>
                    string.Equals(tp.Code, messagePriority, StringComparison.InvariantCultureIgnoreCase));

                var command = _commandFactory.Factory(CommandType.FlashOn, relayId);
                if (!command.Success)
                    return new ResultViewModel { Success = false, Message = command.Message, Id = 1, Code = 400 };

                var result = command.Data.Execute(priority);
                return result;
            }
            catch (Exception e)
            {
                return new ResultViewModel { Message = e.Message, Code = 500 };
            }
        }

        [Route("{relayId:int}/FlashOff")]
        [HttpPost]
        public ResultViewModel FlashOff([FromRoute] int relayId, string messagePriority = "13003")
        {
            try
            {
                var priority = _lookups.TaskPriorities.FirstOrDefault(tp =>
                    string.Equals(tp.Code, messagePriority, StringComparison.InvariantCultureIgnoreCase));

                var command = _commandFactory.Factory(CommandType.FlashOff, relayId);
                if (!command.Success)
                    return new ResultViewModel { Success = false, Message = command.Message, Id = 1, Code = 400 };

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
