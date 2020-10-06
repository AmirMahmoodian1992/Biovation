using Biovation.Brands.Suprema.Model;
using Biovation.CommonClasses.Models;
using Biovation.CommonClasses.Service;
using Biovation.SocketHandler;
using Biovation.SocketHandler.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading;
using DeviceBrands = Biovation.CommonClasses.Models.ConstantValues.DeviceBrands;

namespace Biovation.Brands.Suprema.Devices.Suprema_Version_1
{
    /// <summary>
    /// برای کلاینت طراحی شده برای همستر Biomini
    /// </summary>
    /// <seealso cref="Device" />
    internal class BiominiClient : Device
    {
        private readonly ClientConnection _socketConnection;
        //private readonly object _object;
        private readonly UserService _userService = new UserService();
        private readonly DeviceService _deviceService = new DeviceService();


        public BiominiClient(SupremaDeviceModel info, ClientConnection socketConnection)
            : base(info)
        {
            //_object = new object();
            DeviceAccessSemaphore = new Semaphore(8, 8);
            _socketConnection = socketConnection;
            _socketConnection.AddListener(ClientConnection.ON_CLIENT_DISCONNECT, OnClientDisconnectCallback);
        }

        public override bool ExistOnDevice(uint id)
        {
            throw new NotImplementedException();
        }

        //[MethodImpl(MethodImplOptions.Synchronized)]
        public override bool TransferUser(User userData)
        {
            //var userData = _userService.GetUser(nUserIdn, ConnectionType);

            if (userData == null) return false;

            //try
            //{
            //    var t = GetAccessGroup(nUserIdn);
            //}
            //catch (Exception)
            //{
            //    userHdr.accessGroupMask = 0xffffffff;
            //}

            #region finger Print

            var fingerService = new FingerTemplateService();
            //var userTemplate = fingerService.GetFingerTemplate(nUserIdn, ConnectionType);
            var userTemplate = fingerService.GetFingerTemplateByUserId(userData.Id);

            if (userTemplate == null)
            {
                return true;
            }

            #endregion

            #region card

            //bool hasCard = false;

            //try
            //{
            //    var cardService = new CardServices();
            //    var dtCard = cardService.GetUserCard(nUserIdn, ConnectionType);
            //    int rowc = dtCard.Count;
            //    if (rowc != 0)
            //    {
            //        userHdr.cardID = Convert.ToUInt32(dtCard[rowc - 1].SCardNo);
            //        userHdr.customID = Convert.ToInt32(dtCard[rowc - 1].SCustomNo);
            //        //userHdr.Card = (byte)userCardData[rowc - 1].nBypass;
            //        userHdr.bypassCard = 1;
            //        CardValidation(dtCard[rowc - 1].SCardNo);
            //        hasCard = true;
            //    }

            //    else
            //    {
            //        userHdr.cardID = 0;
            //        userHdr.customID = 0;
            //        Logger.Log("The User : {0} Do not have card or query do not return any thing", nUserIdn);
            //    }

            //}
            //catch (Exception e)
            //{
            //    userHdr.cardID = 0;
            //    Logger.Log(e.ToString());
            //}

            #endregion

            var transferModel = new DataTransferModel
            {
                ClientName = "BiovationServer",
                EventId = 1003,
                Items = new List<object>()
            };

            transferModel.Items.Add(userData);
            transferModel.Items.Add(userTemplate);

            var data = JsonConvert.SerializeObject(transferModel);

            // Send the data through the socket.
            try
            {

                //lock (_object)
                //{
                //DeviceAccessSemaphore.WaitOne(1500);
                DeviceAccessSemaphore.WaitOne(2500);
                _socketConnection.Send(data);
                //}

                //Thread.Sleep(100);
            }
            catch (Exception)
            {
                //UserTransferMutex.ReleaseMutex();
                return false;
            }

            return false;
        }

        public override bool DeleteUser(uint userId)
        {
            var userData = _userService.GetUser(userId, false);
            if (userData == null) return false;

            var transferModel = new DataTransferModel
            {
                ClientName = "BiovationServer",
                EventId = 1005,
                Items = new List<object>()
            };

            transferModel.Items.Add(userData);

            var data = JsonConvert.SerializeObject(transferModel);

            // Send the data through the socket.
            try
            {
                DeviceAccessSemaphore.WaitOne(2500);
                _socketConnection.Send(data);
            }
            catch (Exception)
            {
                return false;
            }

            return false;
        }

        public override ResultViewModel ReadOfflineLog(object cancellationToken, bool fileSave = false)
        {
            var transferModel = new DataTransferModel
            {
                ClientName = "BiovationServer",
                EventId = 1001
            };

            var data = JsonConvert.SerializeObject(transferModel);

            // Send the data through the socket.
            _socketConnection.Send(data);

            return new ResultViewModel { Id = DeviceInfo.DeviceId, Message = "تخلیه تردد آغاز شد..", Validate = 1 };
        }

        public override List<object> ReadLogOfPeriod(int startTime, int endTime)
        {
            return null;
        }

        public override List<User> GetAllUsers()
        {
            return null;
        }

        public override User GetUser(uint id)
        {
            throw new NotImplementedException();
        }

        public override bool TransferAccessGroup(int nAccessIdn)
        {
            return true;
        }

        public override bool TransferTimeZone(int nTimeZone)
        {
            return true;
        }

        public override int AddDeviceToDataBase()
        {
            DeviceInfo.Brand = DeviceBrands.Suprema;
            DeviceInfo.ModelId = DeviceModels.Biomini;
            DeviceInfo.HardwareVersion = "Biomini Client";
            DeviceInfo.SerialNumber = "Kasra Co.";
            DeviceInfo.RegisterDate = DateTime.Now;
            DeviceInfo.Active = true;

            return Convert.ToInt32(_deviceService.ModifyDeviceBasicInfoByID(DeviceInfo).Id);
        }

        public int OnClientDisconnectCallback(ClientConnectionResult clientConnectionResult)
        {
            var bioServer = BioStarServer.FactoryBioStarServer();
            bioServer.DisconnectedProcMethod(DeviceInfo.Handle, DeviceInfo.DeviceId == default ? DeviceInfo.Code : (uint)DeviceInfo.DeviceId, 0, 0, 0, DeviceInfo.IpAddress);
            //BioStarServer.DisconnectedProcMethod(DeviceInfo.Handle, DeviceInfo.DeviceId, 0, 0, 0, DeviceInfo.IpAddress);
            return 0;
        }
    }
}
