using Biovation.CommonClasses;
using Biovation.CommonClasses.Interface;
using Biovation.Constants;
using Biovation.Domain;
using Biovation.Service.Api.v2;
using PalizTiara.Api.CallBacks;
using PalizTiara.Api.Helpers;
using PalizTiara.Api.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;

namespace Biovation.Brands.Paliz.Command
{
    public class PalizGetAdditionalData : ICommand
    {
        private Dictionary<uint, DeviceBasicInfo> _onlineDevices { get; }
        private readonly string _terminalName;
        private readonly int _terminalCode;
        private readonly PalizServer _palizServer;
        private Dictionary<string, string> _additionalDataDict;
        private readonly AutoResetEvent _autoResetEvent;
        private int _numerOfThreadsNotYetCompleted;

        public PalizGetAdditionalData(IReadOnlyList<object> items, PalizServer palizServer
                , DeviceService deviceService)
        {
            _terminalCode = Convert.ToInt32(items[0]);
            //var taskItem = taskService.GetTaskItem(TaskItemId)?.GetAwaiter().GetResult().Data ?? new TaskItem();
            //var data = (JObject)JsonConvert.DeserializeObject(taskItem.Data);
            //if (data != null)
            //{
            //    UserId = (int)data["userId"];
            //}

            _palizServer = palizServer;

            var devices = deviceService.GetDevices(brandId: DeviceBrands.PalizCode).GetAwaiter().GetResult();
            if (devices is null)
            {
                _onlineDevices = new Dictionary<uint, DeviceBasicInfo>();
                return;
            }

            //_code = devices.Data?.Data.FirstOrDefault(d => d.DeviceId == _terminalId)?.Code ?? 0;
            _terminalName = devices.Data?.Data.FirstOrDefault(d => d.Code == _terminalCode)?.Name ?? string.Empty;
            _onlineDevices = _palizServer.GetOnlineDevices();
            _autoResetEvent = new AutoResetEvent(false);
        }

        public object Execute()
        {
            if (_onlineDevices.All(device => device.Key != _terminalCode))
            {
                Logger.Log($"GetAdditionalData,The device: {_terminalCode} is not connected.");
                return new Dictionary<string, string>();
            }

            try
            {
                var additionalData = GetAdditionalData(_terminalName);
                return additionalData;
            }
            catch (Exception exception)
            {
                Logger.Log(exception);
                return new Dictionary<string, string>();
            }
        }

        private Dictionary<string, string> GetAdditionalData(string terminalName)
        {
            lock (_palizServer)
            {
                _additionalDataDict = new Dictionary<string, string>();

                _palizServer._serverManager.UserListEvent += GetUserListEventCallBack;
                _palizServer._serverManager.MassUserInfoSummaryEvent += GetMassUserSummaryInfoEventCallBack;
                _palizServer._serverManager.TiaraSettingsEvent += GetTiaraSettingsCallback;
                _palizServer._serverManager.GetDateTimeInfoEvent += GetDateTimeCallback;

                _numerOfThreadsNotYetCompleted = 3;
                Thread.Sleep(10000);
                _palizServer._serverManager.GetUserListAsyncTask(terminalName).Wait();
                Thread.Sleep(10000);
                _palizServer._serverManager.GetTiaraSettingsAsyncTask(terminalName).Wait();
                Thread.Sleep(10000);
                _palizServer._serverManager.GetDateTimeInfoAsyncTask(terminalName).Wait();

                _palizServer._serverManager.UserListEvent -= GetUserListEventCallBack;
                _palizServer._serverManager.MassUserInfoSummaryEvent -= GetMassUserSummaryInfoEventCallBack;
                _palizServer._serverManager.TiaraSettingsEvent -= GetTiaraSettingsCallback;
                _palizServer._serverManager.GetDateTimeInfoEvent -= GetDateTimeCallback;

                //dic.Add("OpLogCount", opLogCnt.ToString());
                //dic.Add("RecordCount", recordCnt.ToString());

                //var sFirmwareVersion = "";
                //var sMac = "";
                //var sPlatform = "";
                //var sProducer = "";

                //ZkTecoSdk.GetSysOption(code, "~ZKFPVersion", out var iFpAlg);
                //ZkTecoSdk.GetSysOption(code, "ZKFaceVersion", out var iFaceAlg);
                //ZkTecoSdk.GetVendor(ref sProducer);
                //ZkTecoSdk.GetProductCode(code, out var sDeviceName);
                //ZkTecoSdk.GetDeviceMAC(code, ref sMac);
                //ZkTecoSdk.GetFirmwareVersion(code, ref sFirmwareVersion);
                //ZkTecoSdk.GetPlatform(code, ref sPlatform);
                //ZkTecoSdk.GetSerialNumber(code, out var sn);
                //ZkTecoSdk.GetDeviceStrInfo(code, 1, out var sProductTime);

                //dic.Add("FPAlg", iFpAlg);
                //dic.Add("FaceAlg", iFaceAlg);
                //dic.Add("Producer", sProducer);
                //dic.Add("Firmware", sFirmwareVersion);
                //dic.Add("Platform", sPlatform);
                //dic.Add("SN", sn);
                //dic.Add("ProductTime", sProductTime);
                //return dic;

                _autoResetEvent.WaitOne(20000);

                //_additionalDataDict.Add("OpLogCount", _opLogCnt.ToString());
                //_additionalDataDict.Add("RecordCount", _recordCnt.ToString());
                _additionalDataDict.Add("DeviceName", terminalName);

                //_additionalDataDict.Add("Date", deviceDateTime.Date.ToString(CultureInfo.InvariantCulture));

                return _additionalDataDict;
            }
        }

        private void GetDateTimeCallback(object sender, DeviceDateTimeInfoEventArgs args)
        {
            try
            {
                if (args.DeviceDateTimeInfoModel is null)
                {
                    return;
                }

                var deviceDateTime = StaticHelpers.GetDateTimeFromEpoch(args.DeviceDateTimeInfoModel.DateModel.Date);

                lock (_additionalDataDict)
                {
                    _additionalDataDict.Add("Date", deviceDateTime.Date.ToString(CultureInfo.InvariantCulture));
                    _additionalDataDict.Add("Time", deviceDateTime.TimeOfDay.ToString());
                }
            }
            finally
            {
                if (Interlocked.Decrement(ref _numerOfThreadsNotYetCompleted) == 0)
                {
                    _autoResetEvent.Set();
                }
            }
        }

        private void GetTiaraSettingsCallback(object sender, TiaraSettingsEventArgs args)
        {
            try
            {
                if (args.TiaraSettings is null)
                {
                    return;
                }

                lock (_additionalDataDict)
                {
                    //dic.Add("FPAlg", iFpAlg);
                    //dic.Add("FaceAlg", iFaceAlg);
                    //dic.Add("Producer", sProducer);
                    //dic.Add("Firmware", sFirmwareVersion);
                    //dic.Add("Platform", sPlatform);
                    //dic.Add("SN", sn);
                    //dic.Add("ProductTime", sProductTime);
                    _additionalDataDict.Add("Mac", args.TiaraSettings.LanSetting.LanMac);
                }

            }
            finally
            {
                if (Interlocked.Decrement(ref _numerOfThreadsNotYetCompleted) == 0)
                {
                    _autoResetEvent.Set();
                }
            }
        }

        private void GetUserListEventCallBack(object sender, UserListEventArgs args)
        {
            try
            {
                if (args?.UserListModel?.UserIds is null)
                {
                    return;
                }

                _additionalDataDict.Add("UserCount", args.UserListModel.UserIds.Length.ToString());
                var request = new MassUserIdModel(args.UserListModel.UserIds.Min(), args.UserListModel.UserIds.Max());

                _numerOfThreadsNotYetCompleted++;
                _palizServer._serverManager.GetMassUserInfoSummaryTask(_terminalName, request);
            }
            finally
            {
                if (Interlocked.Decrement(ref _numerOfThreadsNotYetCompleted) == 0)
                {
                    _autoResetEvent.Set();
                }
            }
        }

        private void GetMassUserSummaryInfoEventCallBack(object sender, MassUserInfoSummaryEventArgs args)
        {
            try
            {
                if (args?.Users?.UserInfoSummaryModels != null)
                {
                    int adminCount, fpCount, pwdCount, faceCount;
                    adminCount = fpCount = pwdCount = faceCount = 0;

                    foreach (var infoModel in args.Users.UserInfoSummaryModels)
                    {
                        if (infoModel.Level == 1)
                        {
                            adminCount++;
                        }

                        if (infoModel.Password)
                        {
                            pwdCount++;
                        }

                        fpCount += infoModel.Fingerprints;
                        faceCount += infoModel.Faces;
                    }

                    lock (_additionalDataDict)
                    {
                        _additionalDataDict.Add("AdminCount", adminCount.ToString());
                        _additionalDataDict.Add("FPCount", fpCount.ToString());
                        _additionalDataDict.Add("FaceCount", faceCount.ToString());
                        _additionalDataDict.Add("PasswordCount", pwdCount.ToString());
                    }
                }
            }
            finally
            {
                if (Interlocked.Decrement(ref _numerOfThreadsNotYetCompleted) == 0)
                {
                    _autoResetEvent.Set();
                }
            }
        }

        public string GetTitle()
        {
            return "Get device additional data";
        }

        public string GetDescription()
        {
            return $"Getting device additional data from device with code: {_terminalCode}.";
        }

        public void Rollback()
        {

        }
    }
}
