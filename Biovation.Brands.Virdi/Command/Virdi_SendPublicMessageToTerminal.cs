//Todo: Reimplement and use in command factory

//using Biovation.CommonClasses;
//using Biovation.CommonClasses.Interface;
//using Biovation.CommonClasses.Models;
//using Biovation.CommonClasses.Models.ConstantValues;
//using System;
//using System.Collections.Generic;
//using System.Linq;

//namespace Biovation.Brands.Virdi.Command
//{
//    public class VirdiSendPublicMessageToTerminal : ICommand
//    {
//        /// <summary>
//        /// All connected devices
//        /// </summary>
//        private Dictionary<uint, DeviceBasicInfo> OnlineDevices { get; }
//        private uint Code { get; }
//        public string txtStartDate, txtEndDate, txtStartTime, txtEndTime;
//        public string txtMessage;

//        public VirdiSendPublicMessageToTerminal(IReadOnlyList<object> items, Dictionary<uint, DeviceBasicInfo> devices)
//        {

//            //uint code, string startDate,  string endDate, string startTime,  string endTime, string Message

//            Code = (uint)items[0];
//            OnlineDevices = devices;
//            txtStartDate = (string)items[1];
//            txtEndDate = (string)items[2];
//            txtStartTime = (string)items[3];
//            txtEndTime = (string)items[4];
//            txtMessage = (string)items[5];
//        }

//        public object Execute()
//        {
//            if (OnlineDevices.All(device => device.Key != Code))
//            {
//                Logger.Log($"The device: {Code} is not connected.");
//                return new ResultViewModel { Code = Convert.ToInt64(TaskStatuses.DeviceDisconnectedCode), Id = Code, Message = $"The device: {Code} is not connected." };
//            }

//            try
//            {
//                var callbackInstance = Callbacks.GetInstance();
//                callbackInstance.UcsApi.SendPublicMessageToTerminal(0, (int)Code, 1, txtStartDate, txtEndDate, txtStartTime, txtEndTime, txtMessage);
//                //callbackInstance.AccessControlData.InitData();
//            }
//            catch (Exception exception)
//            {
//                Logger.Log(exception);
//            }

//            return true;
//        }

//        public void Rollback()
//        {
//            throw new NotImplementedException();
//        }

//        public string GetTitle()
//        {
//            return "Send Message to terminal";
//        }

//        public string GetDescription()
//        {
//            return $"Sending Message to device: {Code}.";
//        }
//    }
//}
