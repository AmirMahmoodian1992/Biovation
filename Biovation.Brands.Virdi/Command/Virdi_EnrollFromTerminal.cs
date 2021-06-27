//using System;
//using System.Collections.Generic;
//using System.Linq;
//using Biovation.Brands.Virdi.UniComAPI;
//using Biovation.CommonClasses;
//using Biovation.CommonClasses.Interface;
//using Biovation.Domain;
//using Biovation.Constants;
//using Biovation.Service;
//using UNIONCOMM.SDK.UCBioBSP;

//namespace Biovation.Brands.Virdi.Command
//{
//    public class VirdiEnrollFromTerminal : ICommand
//    {
//        private uint Code { get; }
//        private int DeviceId { get; }
//        private int TaskItemId { get; }
//        private readonly Callbacks _callbacks;
//        private readonly VirdiServer _virdiServer;
//        public VirdiEnrollFromTerminal(IReadOnlyList<object> items, Callbacks callbacks, VirdiServer virdiServer, DeviceService deviceService)
//        {
//            _callbacks = callbacks;
//            _virdiServer = virdiServer;
//            DeviceId = Convert.ToInt32(items[0]);
//            TaskItemId = Convert.ToInt32(items[1]);
//            Code = deviceService.GetDeviceBasicInfoByIdAndBrandId(DeviceId, DeviceBrands.VirdiCode)?.Code ?? 0;
//        }

//        public object Execute()
//        {
//            //_virdiServer._ucsApi.EnrollFromTerminal(0, DeviceId);

//            _callbacks.Device.Enumerate();

//            _callbacks.Device.Open((int)Code);

//            var pval = new object();
//            _callbacks.Device.OpenEx((int)Code, 0, ref pval);

//            var opId = _callbacks.Device.OpenedDeviceID;

//            _callbacks.Extraction.Capture(0);
//            var payload = new object();
//            _callbacks.Extraction.Enroll(payload);

//            var exData = new ExportData();
//            var res = UCSAPI.EnrollFromTerminal((ushort) TaskItemId, 45, out exData);

//            //exData.GetFingerInfo()[0].GetTemplateInfoes(new byte())[0].GetData()

//            if (_virdiServer.UcsApi.ErrorCode == 0)
//            {
//                var fingerPrintDataCount = _virdiServer.UcsApi.TotalFingerCount;

//                //for (int i = 0; i < fingerPrintDataCount; i++)
//                //{
//                //    int nFingerID = _virdiServer._ucsApi.get_FingerID(i);
//                //    var nFPDataSize1 = _virdiServer._ucsApi.get_FPSampleDataLength(nFingerID, (int)nTemplateIndex);
//                //    var biFPData1 = _virdiServer._ucsApi.get_FPSampleData(nFingerID, (int)nTemplateIndex) as byte[];
//                //    var nFPDataSize2 = _virdiServer._ucsApi.get_FPSampleDataLength(nFingerID, (int)nTemplateIndex + 1);
//                //    var biFPData2 = _virdiServer._ucsApi.get_FPSampleData(nFingerID, (int)nTemplateIndex + 1) as byte[];
//                //    if (i == 0)
//                //        bInitialize = true;
//                //    else
//                //        bInitialize = false;
//                //    _virdiServer.fi.Import(Convert.ToInt32(bInitialize), nFingerID, (int)nPurpose, (int)_mainFrm.nTemplateType400, (int)nTemplateSize, biFPData1, biFPData2);
//                //}

//                //_mainFrm.binaryEnrolledFIR = this._mainFrm.fpData.FIR as byte[];
//                //_mainFrm.szTextEnrolledFIR = _mainFrm.fpData.TextFIR;

//            }
//            var size = 1;

//            uint tCnt = 0;
//            //var res = UCSAPI.GetTerminalCount(ref tCnt);

//            TerminalInfo terInf = default(TerminalInfo);
//            res = UCSAPI.GetTerminalInfo(45, ref terInf);

//            //var exData = new ExportData();
//            //res = UCSAPI.EnrollFromTerminal(0, 45, out exData);

//            var result = _virdiServer.UCBioApi.OpenDevice((short)Code);
//            var opened = _virdiServer.UCBioApi.GetOpenedDeviceID();

//            var enr = default(UCBioAPI.Type.ENROLL_INIT_INFO);
//            var yy = new UniComAPI.UCBioApi();
//            //var uin = new UniComAPI.UCBioApi.InputDevice(yy, new DeviceInfo {DeviceID = 45});

//            byte[] finger = new byte[500];
//            byte[] fingeraud = new byte[2500];

//            //var resl = UniComAPI.UCBioApi.Capture(yy.prvHandle, FirPurpose.Enroll, out finger, 1000, out fingeraud, new WindowOption());

//            enr.Reserved1 = 0;
//            enr.Reserved2 = 0;

//            //result = yy.SetEnrollInitInfo(enr);
//            //uin.OpenDevice();
//            result = _virdiServer.UCBioApi.OpenDeviceEx((short)Code, 0);
//            opened = _virdiServer.UCBioApi.GetOpenedDeviceID();
//            result = _virdiServer.UCBioApi.CloseDevice((short)Code);
//            var devInfo = new UCBioAPI.Type.DEVICE_INFO_0();
//            var initInfo = new UCBioAPI.Type.INIT_INFO_0();
//            result = _virdiServer.UCBioApi.GetInitInfo(out initInfo);
//            result = _virdiServer.UCBioApi.GetDeviceInfo((short)Code, out devInfo);

//            Logger.Log("");
//            Logger.Log($"-->Terminal:{Code} Finger Template enrolled.");
//            Logger.Log("   +ErrorCode :" + _virdiServer.UcsApi.ErrorCode.ToString("X4") + "\n");
           
//            return new ResultViewModel { Code = _virdiServer.UcsApi.ErrorCode, Id = DeviceId, Message = $"  Enroll User from device: {Code}. Error code = {_virdiServer.UcsApi.ErrorCode}\n", Validate = 0 };
//        }

//        public void Rollback()
//        {
//            throw new NotImplementedException();
//        }

//        public string GetTitle()
//        {
//            return "Lock device";
//        }

//        public string GetDescription()
//        {
//            return "Locking given device.";
//        }
//    }
//}
