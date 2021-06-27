using System;
using Microsoft.AspNetCore.Mvc;
using Biovation.Services.RelayController.Commands;
using Biovation.Services.RelayController.Domain;
using Biovation.Services.RelayController.Services;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Biovation.Services.RelayController.Controllers
{
    [Route("biovation/api/[controller]")]
    [ApiController]
    public class RelayController : ControllerBase
    {
        private readonly CommandFactory _commandFactory;

        public RelayController(GetRelayService tcpClientGetter)
        {
            _commandFactory = new CommandFactory(tcpClientGetter);
        }

        /// <summary>
        /// it contacts the relay with passed id
        /// </summary>
        /// <param name="relayId"></param>
        /// <returns></returns>
        [Route("Contact/{relayId}")]
        [HttpGet]
        public string Contact(int relayId)
        {
            try
            {
                var command = _commandFactory.Factory(CommandType.Contact, relayId);
                if (!command.Success) return command.Message;
                var result = command.Data.Execute();
                return result.Message;

            }
            catch (Exception e)
            {
                return e.Message;
            }
        }

        /// <summary>
        /// it turns on the relay with passed id
        /// </summary>
        /// <param name="relayId"></param>
        /// <returns></returns>
        [Route("TurnOn/{relayId}")]
        [HttpGet]
        public string TurnOn(int relayId)
        {
            try
            {
                var command = _commandFactory.Factory(CommandType.TurnOn, relayId);
                if (!command.Success) return command.Message;
                var result = command.Data.Execute();
                return result.Message;
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }

        /// <summary>
        /// it turns on the relay with passed id
        /// </summary>
        /// <param name="relayId"></param>
        /// <returns></returns>
        [Route("TurnOff/{relayId}")]
        [HttpGet]
        public string TurnOff(int relayId)
        {
            try
            {
                var command = _commandFactory.Factory(CommandType.TurnOff, relayId);
                if (!command.Success) return command.Message;
                var result = command.Data.Execute();
                return result.Message;
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }

        /// <summary>
        /// it turns the relay flashing on
        /// </summary>
        /// <param name="relayId"></param>
        /// <returns></returns>
        [Route("FlashOn/{relayId}")]
        [HttpGet]
        public string FlashOn(int relayId)
        {
            try
            {
                var command = _commandFactory.Factory(CommandType.FlashOn, relayId);
                if (!command.Success) return command.Message;
                var result = command.Data.Execute();
                return result.Message;
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }

        /// <summary>
        /// it turns the relay flashing off
        /// </summary>
        /// <param name="relayId"></param>
        /// <returns></returns>
        [Route("FlashOff/{relayId}")]
        [HttpGet]
        public string FlashOff(int relayId)
        {
            try
            {
                var command = _commandFactory.Factory(CommandType.FlashOff, relayId);
                if (!command.Success) return command.Message;
                var result = command.Data.Execute();
                return result.Message;
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }
    }
}
