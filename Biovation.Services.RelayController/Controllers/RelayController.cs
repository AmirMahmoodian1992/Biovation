using System;
using Microsoft.AspNetCore.Mvc;
using Biovation.Services.RelayController.Commands;
using Biovation.Services.RelayController.Domain;
using Biovation.Services.RelayController.Models;
using Biovation.Services.RelayController.Relays;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Biovation.Services.RelayController.Controllers
{
    [Route("biovation/api/[controller]")]
    [ApiController]
    public class RelayController : ControllerBase
    {
        private readonly CommandFactory _commandFactory;
        private readonly RelayFactory _relayFactory;

        public RelayController()
        {
            _commandFactory = new CommandFactory();
            _relayFactory = new RelayFactory();
        }

        /// <summary>
        /// it contacts the relay with passed id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("Contact/{id}")]
        [HttpGet]
        public string Contact(int id)
        {
            try
            {
                var relayInfo = new Relay(ip: "192.168.1.200", port: 23, id: id, brand: RelayBrands.Behsan);
                var relay = _relayFactory.Factory(relayInfo);
                var command = _commandFactory.Factory(CommandType.Contact, relay);
                command.Execute();
                return $"relay {id} contacted successfully :) ";
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }

        /// <summary>
        /// it turns on the relay with passed id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("TurnOn/{id}")]
        [HttpGet]
        public string TurnOn(int id)
        {
            try
            {
                var relayInfo = new Relay(ip: "192.168.1.200", port: 23, id: id, brand: RelayBrands.Behsan);
                var relay = _relayFactory.Factory(relayInfo);
                var command = _commandFactory.Factory(CommandType.TurnOn, relay);
                command.Execute();
                return $"relay {id} turned on successfully :) ";
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }

        /// <summary>
        /// it turns on the relay with passed id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("TurnOff/{id}")]
        [HttpGet]
        public string TurnOff(int id)
        {
            try
            {
                var relayInfo = new Relay(ip: "192.168.1.200", port: 23, id: id, brand: RelayBrands.Behsan);
                var relay = _relayFactory.Factory(relayInfo);
                var command = _commandFactory.Factory(CommandType.TurnOff, relay);
                command.Execute();
                return $"relay {id} turned off successfully :) ";
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }

        /// <summary>
        /// it turns the relay flashing on
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("FlashOn/{id}")]
        [HttpGet]
        public string FlashOn(int id)
        {
            try
            {
                var relayInfo = new Relay(ip: "192.168.1.200", port: 23, id: id, brand: RelayBrands.Behsan);
                var relay = _relayFactory.Factory(relayInfo);
                var command = _commandFactory.Factory(CommandType.FlashOn, relay);
                command.Execute();
                return $"relay {id} started flashing successfully :) ";
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }

        /// <summary>
        /// it turns the relay flashing off
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("FlashOff/{id}")]
        [HttpGet]
        public string FlashOff(int id)
        {
            try
            {
                var relayInfo = new Relay(ip: "192.168.1.200", port: 23, id: id, brand: RelayBrands.Behsan);
                var relay = _relayFactory.Factory(relayInfo);
                var command = _commandFactory.Factory(CommandType.FlashOff, relay);
                command.Execute();
                return $"relay {id} stoped flashing successfully :) ";
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }

        /// <summary>
        /// it returns the data of the relay (it does not work)
        /// </summary>
        /// <param name="id"></param>
        /// <returns> [string] serialized data </returns>
        [Route("GetData/{id}")]
        [HttpGet]
        public string GetData(int id)
        {
            try
            {
                var relayInfo = new Relay(ip: "192.168.1.200", port: 23, id: id, brand: RelayBrands.Behsan);
                var relay = _relayFactory.Factory(relayInfo);
                var command = _commandFactory.Factory(CommandType.GetData, relay);
                var data = command.Execute();
                return $"relay {id} information: \n {data}";
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }

        /// <summary>
        /// it returns the status of the relay (it does not work)
        /// </summary>
        /// <param name="id"></param>
        /// <returns> [string] relay status </returns>
        [Route("GetStatus/{id}")]
        [HttpGet]
        public string GetStatus(int id)
        {
            try
            {
                var relayInfo = new Relay(ip: "192.168.1.200", port: 23, id: id, brand: RelayBrands.Behsan);
                var relay = _relayFactory.Factory(relayInfo);
                var command = _commandFactory.Factory(CommandType.GetStatus, relay);
                var data = command.Execute();
                return $"relay {id} status: \n {data}";
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }
    }
}
