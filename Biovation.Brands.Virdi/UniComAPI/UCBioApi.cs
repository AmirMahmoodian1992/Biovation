using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;

namespace Biovation.Brands.Virdi.UniComAPI
{
    public class UCBioApi : IDisposable
    {
        public UCBioApi()
        {
            var apiReturn = Init(out prvHandle);
            var flag = apiReturn > ApiReturn.BaseGeneral;
            if (flag)
            {
                prvHandle.Close();
                throw new InvalidOperationException("Can not Initialize UCBioBSP");
            }
        }

        [DllImport("UCBioBSP.dll", CharSet = CharSet.Ansi, EntryPoint = "UCBioAPI_SetAutoDetect")]
        public static extern ApiReturn SetAutoDetect(UnionSafeHandle hHandle, bool bFlag);

        [DllImport("UCBioBSP.dll", CharSet = CharSet.Ansi, EntryPoint = "UCBioAPI_GetVersion")]
        private static extern ApiReturn GetVersion(UnionSafeHandle hHandle, out ApiVersion pVersion);

        [DllImport("UCBioBSP.dll", CharSet = CharSet.Ansi, EntryPoint = "UCBioAPI_Init")]
        public static extern ApiReturn Init(out UnionSafeHandle handle);

        [DllImport("UCBioBSP.dll", CharSet = CharSet.Ansi, EntryPoint = "UCBioAPI_Capture")]
        public static extern ApiReturn Capture(int terminalId);

        [DllImport("UCBioBSP.dll", CharSet = CharSet.Ansi, EntryPoint = "UCBioAPI_Capture")]
        public static extern ApiReturn Capture(UnionSafeHandle handle, FirPurpose purpose, out uint hFinger, int timeout, out uint phAuditData, WindowOption option);

        //[DllImport("UCBioBSP.dll", CharSet = CharSet.Ansi, EntryPoint = "UCBioAPI_AuditFIRToImage")]
        //private static extern ApiReturn AuditFIRToImage(UnionSafeHandle hHandle, ref InputFir piCapturedFIR, out ExportAuditData exportData);

        [DllImport("UCBioBSP.dll", CharSet = CharSet.Ansi, EntryPoint = "UCBioAPI_Terminate")]
        private static extern ApiReturn Terminate(UnionSafeHandle handle);

        [DllImport("UCBioBSP.dll", CharSet = CharSet.Ansi, EntryPoint = "UCBioAPI_EnumerateDevice")]
        private static extern ApiReturn EnumerateDevice(UnionSafeHandle handle, out int numDevice, out IntPtr deviceIDs, out IntPtr deviceinfo);

        [DllImport("UCBioBSP.dll", CharSet = CharSet.Ansi, EntryPoint = "UCBioAPI_OpenDevice")]
        public static extern ApiReturn OpenDevice(UnionSafeHandle handle, ushort deviceID);

        [DllImport("UCBioBSP.dll", CharSet = CharSet.Ansi, EntryPoint = "UCBioAPI_CloseDevice")]
        public static extern ApiReturn CloseDevice(UnionSafeHandle handle, ushort deviceID);

        [DllImport("UCBioBSP.dll", CharSet = CharSet.Ansi, EntryPoint = "UCBioAPI_TemplateToFIREx")]
        private static extern ApiReturn TemplateToFIRExtended(UnionSafeHandle hHandle, byte[] pTemplateData, uint nTemplateDataSize, uint nOneTemplateSize, TemplateSize nTemplateDataType, FirPurpose nnPurpose, out IntPtr phProcessedFIR);

        [DllImport("UCBioBSP.dll", CharSet = CharSet.Ansi, EntryPoint = "UCBioAPI_FIRToTemplate")]
        private static extern ApiReturn FIRToTemplate(UnionSafeHandle handle, ref InputFir inputFir, out ExportData exportdata, TemplateSize templateType);

        [DllImport("UCBioBSP.dll", CharSet = CharSet.Ansi, EntryPoint = "UCBioAPI_ImportDataToFIREx")]
        private static extern ApiReturn ImportDataToFIREx(UnionSafeHandle handle, ref ExportData pExportData, FirPurpose nPurpose, FirDataType nDataType, out IntPtr phProcessedFIR, IntPtr pReserved);

        [DllImport("UCBioBSP.dll", CharSet = CharSet.Ansi, EntryPoint = "UCBioAPI_ImportDataToFIR")]
        private static extern ApiReturn ImportDataToFIR(UnionSafeHandle hHandle, ref ExportData pExportData, FirPurpose nPurpose, out uint phProcessedFIR);

        [DllImport("UCBioBSP.dll", CharSet = CharSet.Ansi, EntryPoint = "UCBioAPI_VerifyMatch")]
        private static extern ApiReturn VerifyMatch(UnionSafeHandle hHandle, ref InputFir piProcessedFIR, ref InputFir piStoredFir, out bool pbResult, IntPtr pPayload);

        [DllImport("UCBioBSP.dll", CharSet = CharSet.Ansi, EntryPoint = "UCBioAPI_CreateTemplate")]
        private static extern ApiReturn CreateTemplate(UnionSafeHandle hHandle, InputFir piCapturedFIR, InputFir piStoredFir, out IntPtr phNewFir, IntPtr pPayload);

        [DllImport("UCBioBSP.dll", CharSet = CharSet.Ansi, EntryPoint = "UCBioAPI_Process")]
        private static extern ApiReturn Process(UnionSafeHandle hHandle, InputFir piCapturedFIR, out IntPtr phProcessedFir);

        [DllImport("UCBioBSP.dll", CharSet = CharSet.Ansi, EntryPoint = "UCBioAPI_SetInitInfo")]
        private static extern ApiReturn SetInitInfo(UnionSafeHandle hHandle, [MarshalAs(UnmanagedType.U1)] byte nStructureType, ref InitInfo0 initInfo);

        [DllImport("UCBioBSP.dll", CharSet = CharSet.Ansi, EntryPoint = "UCBioAPI_GetInitInfo")]
        private static extern ApiReturn GetInitInfo(UnionSafeHandle hHandle, [MarshalAs(UnmanagedType.U1)] byte nStructureType, out InitInfo0 initInfo);

        [DllImport("UCBioBSP.dll", CharSet = CharSet.Ansi, EntryPoint = "UCBioAPI_SetTemplateFormat")]
        private static extern ApiReturn SetTemplateFormat(UnionSafeHandle handele, TemplateFormat format);

        public static ExportData ConvertToExportData(byte[] fingerData1, byte[] fingerData2, TemplateFormat templateFormat, FingerID fingerId, UnmanegedMemoryScope scope)
        {
            var flag = fingerData1.Length != fingerData2.Length;
            if (flag)
            {
                throw new InvalidOperationException(string.Format("Fingerprints must be the same size ({0},{1})", fingerData1.Length, fingerData2.Length));
            }
            FingerPrintProfile.CheckMinutia(fingerData1, templateFormat);
            FingerPrintProfile.CheckMinutia(fingerData2, templateFormat);
            var result = default(ExportData);
            result.TemplateType = FingerprintData.ToTemplateSize(templateFormat);
            result.SamplesPerFinger = 2;
            var array = new FingerBlock[1];
            array[0].FingerID = fingerId;
            var array2 = new TemplateBlock[2];
            array2[0].SetData(fingerData1, scope);
            array2[1].SetData(fingerData2, scope);
            array[0].SetTemplateInfoes(array2, scope);
            result.SetFingerInfo(array, scope);
            return result;
        }

        public static ExportData ConvertToExportData(byte[] fingerData, TemplateFormat templateFormat, FingerID fingerId, UnmanegedMemoryScope scope)
        {
            FingerPrintProfile.CheckMinutia(fingerData, templateFormat);
            var result = default(ExportData);
            result.TemplateType = FingerprintData.ToTemplateSize(templateFormat);
            result.SamplesPerFinger = 1;
            var array = new FingerBlock[1];
            array[0].FingerID = fingerId;
            var array2 = new TemplateBlock[1];
            array2[0].SetData(fingerData, scope);
            array[0].SetTemplateInfoes(array2, scope);
            result.SetFingerInfo(array, scope);
            return result;
        }

        public static byte[] ConvertToArray(ExportData exportData, uint userID)
        {
            var fingerPrintProfile = new FingerPrintProfile(exportData.TemplateType);
            fingerPrintProfile.SamplesPerFinger = exportData.SamplesPerFinger;
            fingerPrintProfile.UserID = userID;
            var fingerInfo = exportData.GetFingerInfo();
            var array = fingerInfo;
            for (var i = 0; i < array.Length; i++)
            {
                var fingerBlock = array[i];
                var fingerprint = fingerPrintProfile.CreateFingerprint();
                fingerprint.FingerID = fingerBlock.FingerID;
                var templateInfoes = fingerBlock.GetTemplateInfoes(exportData.SamplesPerFinger);
                for (var j = 0; j < (int)fingerPrintProfile.SamplesPerFinger; j++)
                {
                    fingerprint.Samples[j] = templateInfoes[j].GetData();
                }
                fingerPrintProfile.AddFingerprint(fingerprint);
            }
            return fingerPrintProfile.GetBytes();
        }

        public static ExportData ConvertToExportData(byte[] fingerBlockData, UnmanegedMemoryScope scope)
        {
            var flag = fingerBlockData != null && fingerBlockData.Length != 0;
            if (flag)
            {
                var fingerPrintProfile = new FingerPrintProfile(fingerBlockData);
                var result = default(ExportData);
                result.DefaultFingerID = FingerID.Unknown;
                result.TemplateType = fingerPrintProfile.TemplateSize;
                result.SamplesPerFinger = fingerPrintProfile.SamplesPerFinger;
                var array = new FingerBlock[fingerPrintProfile.FingerprintCount];
                var num = 0;
                foreach (var current in (IEnumerable<Fingerprint>)fingerPrintProfile)
                {
                    array[num].FingerID = current.FingerID;
                    var array2 = new TemplateBlock[(int)fingerPrintProfile.SamplesPerFinger];
                    for (var i = 0; i < (int)fingerPrintProfile.SamplesPerFinger; i++)
                    {
                        array2[i].SetData(current.Samples[i], scope);
                    }
                    array[num].SetTemplateInfoes(array2, scope);
                    num++;
                }
                result.SetFingerInfo(array, scope);
                return result;
            }
            throw new ArgumentNullException("Parameter data");
        }

        public static ExportData Combine(ExportData ex1, ExportData ex2, UnmanegedMemoryScope scope)
        {
            var flag = ex1.TemplateType != ex2.TemplateType;
            if (flag)
            {
                throw new InvalidOperationException(string.Format("Fingerprints must be the same size ({0},{1})", ex1.TemplateType, ex2.TemplateType));
            }
            var flag2 = ex1.SamplesPerFinger == 1 && ex2.SamplesPerFinger == 1;
            if (flag2)
            {
                var result = default(ExportData);
                result.SamplesPerFinger = 2;
                result.TemplateType = ex1.TemplateType;
                var array = new TemplateBlock[2];
                var array2 = new FingerBlock[1];
                array[0].SetData(ex1.GetFingerInfo()[0].GetTemplateInfoes(ex1.SamplesPerFinger)[0].GetData(), scope);
                array[1].SetData(ex2.GetFingerInfo()[0].GetTemplateInfoes(ex1.SamplesPerFinger)[0].GetData(), scope);
                array2[0].SetTemplateInfoes(array, scope);
                array2[0].FingerID = ex1.GetFingerInfo()[0].FingerID;
                result.SetFingerInfo(array2, scope);
                return result;
            }
            throw new InvalidOperationException("One of fingerprints has more than one finger samples");
        }

        //public static ExportData Merge(List<ExportData> fingerprints, UnmanegedMemoryScope scope)
        //{
        //	var flag = fingerprints.Count == 0;
        //	if (flag)
        //	{
        //		throw new InvalidOperationException("List must have an Item");
        //	}
        //	var flag2 = fingerprints.Count == 1;
        //	ExportData result;
        //	if (flag2)
        //	{
        //		result = fingerprints[0];
        //	}
        //	else
        //	{
        //		Func<ExportData, TemplateSize> arg_56_1;
        //		if ((arg_56_1 = UCBioApi.<>c.<>9__22_0) == null)
        //		{
        //			arg_56_1 = (UCBioApi.<>c.<>9__22_0 = new Func<ExportData, TemplateSize>(UCBioApi.<>c.<>9.<Merge>b__22_0));
        //		}
        //		bool flag3 = !fingerprints.AllTheSame(arg_56_1);
        //		if (flag3)
        //		{
        //			throw new InvalidOperationException("All fingerprints must have the same TemplateType");
        //		}
        //		var exportData = default(ExportData);
        //		var list = new List<FingerBlock>();
        //		foreach (var current in fingerprints)
        //		{
        //			var fingerInfo = current.GetFingerInfo();
        //			for (var i = 0; i < fingerInfo.Length; i++)
        //			{
        //				var item = default(FingerBlock);
        //				var templateInfoes = fingerInfo[i].GetTemplateInfoes(2);
        //				var array = new TemplateBlock[2];
        //				array[0].SetData(templateInfoes[0].GetData(), scope);
        //				array[1].SetData(templateInfoes[1].GetData(), scope);
        //				item.FingerID = fingerInfo[i].FingerID;
        //				item.SetTemplateInfoes(array, scope);
        //				list.Add(item);
        //			}
        //		}
        //		exportData.DefaultFingerID = fingerprints[0].DefaultFingerID;
        //		exportData.SamplesPerFinger = 2;
        //		exportData.TemplateType = fingerprints[0].TemplateType;
        //		exportData.SetFingerInfo(list.ToArray(), scope);
        //		result = exportData;
        //	}
        //	return result;
        //}

        public static List<ExportData> Extract(ExportData ex, UnmanegedMemoryScope scope)
        {
            var list = new List<ExportData>();
            var fingerInfo = ex.GetFingerInfo();
            var array = fingerInfo;
            for (var i = 0; i < array.Length; i++)
            {
                var fingerBlock = array[i];
                var item = default(ExportData);
                item.DefaultFingerID = ex.DefaultFingerID;
                item.SamplesPerFinger = ex.SamplesPerFinger;
                item.TemplateType = ex.TemplateType;
                var array2 = new FingerBlock[1];
                array2[0].FingerID = fingerBlock.FingerID;
                var array3 = new TemplateBlock[(int)ex.SamplesPerFinger];
                var templateInfoes = fingerBlock.GetTemplateInfoes(ex.SamplesPerFinger);
                for (var j = 0; j < (int)ex.SamplesPerFinger; j++)
                {
                    array3[j].SetData(templateInfoes[j].GetData(), scope);
                }
                array2[0].SetTemplateInfoes(array3, scope);
                item.SetFingerInfo(array2, scope);
                list.Add(item);
            }
            return list;
        }

        //public static byte GetQuality(ExportData ex)
        //{
        //	var templateType = ex.TemplateType;
        //	byte result;
        //	if (templateType != TemplateSize.Size400)
        //	{
        //		if (templateType != TemplateSize.Size500 && templateType != TemplateSize.Size600)
        //		{
        //			result = 0;
        //		}
        //		else
        //		{
        //			result = (byte)ex.GetFingerInfo().Select(delegate(FingerBlock n)
        //			{
        //				IEnumerable<TemplateBlock> arg_31_0 = n.GetTemplateInfoes(ex.SamplesPerFinger);
        //				Func<TemplateBlock, int> arg_31_1;
        //				if ((arg_31_1 = UCBioApi.<>c.<>9__24_3) == null)
        //				{
        //					arg_31_1 = (UCBioApi.<>c.<>9__24_3 = new Func<TemplateBlock, int>(UCBioApi.<>c.<>9.<GetQuality>b__24_3));
        //				}
        //				return arg_31_0.Average(arg_31_1);
        //			}).Average();
        //		}
        //	}
        //	else
        //	{
        //		result = (byte)ex.GetFingerInfo().Select(delegate(FingerBlock n)
        //		{
        //			IEnumerable<TemplateBlock> arg_31_0 = n.GetTemplateInfoes(ex.SamplesPerFinger);
        //			Func<TemplateBlock, int> arg_31_1;
        //			if ((arg_31_1 = UCBioApi.<>c.<>9__24_1) == null)
        //			{
        //				arg_31_1 = (UCBioApi.<>c.<>9__24_1 = new Func<TemplateBlock, int>(UCBioApi.<>c.<>9.<GetQuality>b__24_1));
        //			}
        //			return arg_31_0.Average(arg_31_1);
        //		}).Average();
        //	}
        //	return result;
        //}



        public FingerprintData ConvertToFingerprintData(ExportData data)
        {
            var hasValidHandle = HasValidHandle;
            if (!hasValidHandle)
            {
                throw new InvalidOperationException("UCBioBSP not initialized");
            }
            uint handle;
            var apiReturn = ImportDataToFIR(prvHandle, ref data, FirPurpose.Enroll, out handle);
            var flag = apiReturn == ApiReturn.BaseGeneral;
            if (flag)
            {
                return new FingerprintData(handle, FingerprintData.ToTemplateFormat(data.TemplateType), 0);
            }
            throw new InvalidOperationException(string.Format("Can not Convert ExportData to (Finger) - UError Code:{0} - ErrorName:{1}", (uint)apiReturn, apiReturn.ToString()));
        }

        public bool HasValidHandle
        {
            get
            {
                return !prvHandle.IsInvalid && !prvHandle.IsClosed;
            }
        }

        public bool AutoDetect
        {
            set
            {
                var apiReturn = SetAutoDetect(prvHandle, value);
                var flag = apiReturn > ApiReturn.BaseGeneral;
                if (flag)
                {
                    throw new InvalidOperationException(string.Format("Can not set auto detect - VirdiError Code:{0} - ErrorName:{1}", (uint)apiReturn, apiReturn));
                }
            }
        }

        public Version CurrentVersion
        {
            get
            {
                ApiVersion apiVersion;
                var version = GetVersion(prvHandle, out apiVersion);
                var flag = version > ApiReturn.BaseGeneral;
                if (flag)
                {
                    throw new InvalidOperationException(string.Format("Can not get current sdk version - VirdiError Code:{0} - ErrorName:{1}", (uint)version, version));
                }
                return new Version(apiVersion.Major, apiVersion.Minor);
            }
        }

        public InputDevice[] GetConnectedDevices()
        {
            var hasValidHandle = HasValidHandle;
            if (!hasValidHandle)
            {
                throw new InvalidOperationException("UCBioBSP not initialized");
            }
            int num;
            IntPtr intPtr;
            IntPtr intPtr2;
            var apiReturn = EnumerateDevice(prvHandle, out num, out intPtr, out intPtr2);
            var flag = apiReturn == ApiReturn.BaseGeneral;
            if (flag)
            {
                var array = new InputDevice[num];
                var offset = Marshal.SizeOf(typeof(DeviceInfo));
                for (var i = 0; i < num; i++)
                {
                    var deviceInfo = (DeviceInfo)Marshal.PtrToStructure(intPtr2, typeof(DeviceInfo));
                    intPtr2 += offset;
                    array[i] = new InputDevice(this, deviceInfo);
                }
                return array;
            }
            throw new InvalidOperationException(string.Format("Can Not Get Devices Information - UError Code:{0} - ErrorName:{1}", (uint)apiReturn, apiReturn.ToString()));
        }

        public SafeExportData ConvertToExportData(FingerprintData fingerprintData)
        {
            var hasValidHandle = HasValidHandle;
            if (!hasValidHandle)
            {
                throw new InvalidOperationException("UCBioBSP not initialized");
            }
            var inputFIR = fingerprintData.GetInputFir();
            ExportData exportData;
            var apiReturn = FIRToTemplate(prvHandle, ref inputFIR, out exportData, fingerprintData.TemplateSize);
            inputFIR.Release();
            var flag = apiReturn == ApiReturn.BaseGeneral;
            if (flag)
            {
                return new SafeExportData(exportData);
            }
            throw new InvalidOperationException(string.Format("Can not convert (Template finger) to finger - UError Code:{0} - ErrorName:{1}", (uint)apiReturn, apiReturn.ToString()));
        }

        public FingerprintData ConvertToFingerprintData(byte[] fingerBlockData, UnmanegedMemoryScope scope = null)
        {
            FingerprintData result = null;
            var flag = false;
            var flag2 = scope == null;
            if (flag2)
            {
                flag = true;
                scope = UnmanegedMemoryScope.Begin();
            }
            try
            {
                result = ConvertToFingerprintData(ConvertToExportData(fingerBlockData, scope));
            }
            finally
            {
                var flag3 = flag;
                if (flag3)
                {
                    scope.Dispose();
                }
            }
            return result;
        }

        public FingerprintData ConvertToFingerprintData(byte[] fingerData, TemplateFormat templateFormat, FingerID fingerId, UnmanegedMemoryScope scope = null)
        {
            FingerprintData result = null;
            var flag = false;
            var flag2 = scope == null;
            if (flag2)
            {
                flag = true;
                scope = UnmanegedMemoryScope.Begin();
            }
            try
            {
                var data = ConvertToExportData(fingerData, templateFormat, fingerId, scope);
                result = ConvertToFingerprintData(data);
            }
            finally
            {
                var flag3 = flag;
                if (flag3)
                {
                    scope.Dispose();
                }
            }
            return result;
        }

        public void SetTemplateFormat(TemplateFormat templateFormat)
        {
            var hasValidHandle = HasValidHandle;
            if (!hasValidHandle)
            {
                throw new InvalidOperationException("UCBioBSP not initialized");
            }
            var apiReturn = SetTemplateFormat(prvHandle, templateFormat);
            var flag = apiReturn > ApiReturn.BaseGeneral;
            if (flag)
            {
                throw new InvalidOperationException(string.Format("Can not Set Template Formate to UCBioBSP - UError Code:{0} - ErrorName:{1}", (uint)apiReturn, apiReturn.ToString()));
            }
        }

        public byte[] ConvertToArray(FingerprintData fingerprintData, uint userID)
        {
            byte[] result = null;
            using (var safeExportData = ConvertToExportData(fingerprintData))
            {
                result = ConvertToArray(safeExportData.ExportData, userID);
            }
            return result;
        }

        public FingerprintData Combine(FingerprintData f1, FingerprintData f2, UnmanegedMemoryScope scope = null)
        {
            var flag = f1.TemplateSize != f2.TemplateSize;
            if (flag)
            {
                throw new InvalidOperationException(string.Format("Fingerprints must be the same size ({0},{1})", f1.TemplateSize, f2.TemplateSize));
            }
            FingerprintData result = null;
            using (var safeExportData = ConvertToExportData(f1))
            {
                using (var safeExportData2 = ConvertToExportData(f2))
                {
                    var exportData = safeExportData.ExportData;
                    var exportData2 = safeExportData2.ExportData;
                    var flag2 = false;
                    var flag3 = scope == null;
                    if (flag3)
                    {
                        flag2 = true;
                        scope = UnmanegedMemoryScope.Begin();
                    }
                    try
                    {
                        result = ConvertToFingerprintData(Combine(exportData, exportData2, scope));
                    }
                    finally
                    {
                        var flag4 = flag2;
                        if (flag4)
                        {
                            scope.Dispose();
                        }
                    }
                }
            }
            return result;
        }

        //public FingerprintData Merge(List<FingerprintData> fingerprints, UnmanegedMemoryScope scope = null)
        //{
        //	var flag = fingerprints.Count == 0;
        //	if (flag)
        //	{
        //		throw new InvalidOperationException("List must have an Item");
        //	}
        //	var flag2 = fingerprints.Count == 1;
        //	FingerprintData result;
        //	if (flag2)
        //	{
        //		result = fingerprints[0];
        //	}
        //	else
        //	{
        //		Func<FingerprintData, TemplateFormat> arg_56_1;
        //		if ((arg_56_1 = UCBioApi.<>c.<>9__42_0) == null)
        //		{
        //			arg_56_1 = (UCBioApi.<>c.<>9__42_0 = new Func<FingerprintData, TemplateFormat>(UCBioApi.<>c.<>9.<Merge>b__42_0));
        //		}
        //		bool flag3 = !fingerprints.AllTheSame(arg_56_1);
        //		if (flag3)
        //		{
        //			throw new InvalidOperationException("All fingerprints must have the same TemplateType");
        //		}
        //		FingerprintData fingerprintData = null;
        //		var flag4 = false;
        //		var flag5 = scope == null;
        //		if (flag5)
        //		{
        //			flag4 = true;
        //			scope = UnmanegedMemoryScope.Begin();
        //		}
        //		try
        //		{
        //			var list = (from n in fingerprints
        //			select ConvertToExportData(n)).ToList<SafeExportData>();
        //			IEnumerable<SafeExportData> arg_C6_0 = list;
        //			Func<SafeExportData, ExportData> arg_C6_1;
        //			if ((arg_C6_1 = UCBioApi.<>c.<>9__42_2) == null)
        //			{
        //				arg_C6_1 = (UCBioApi.<>c.<>9__42_2 = new Func<SafeExportData, ExportData>(UCBioApi.<>c.<>9.<Merge>b__42_2));
        //			}
        //			fingerprintData = ConvertToFingerprintData(Merge(arg_C6_0.Select(arg_C6_1).ToList<ExportData>(), scope));
        //			var arg_FD_0 = list;
        //			Action<SafeExportData> arg_FD_1;
        //			if ((arg_FD_1 = UCBioApi.<>c.<>9__42_3) == null)
        //			{
        //				arg_FD_1 = (UCBioApi.<>c.<>9__42_3 = new Action<SafeExportData>(UCBioApi.<>c.<>9.<Merge>b__42_3));
        //			}
        //			arg_FD_0.ForEach(arg_FD_1);
        //		}
        //		finally
        //		{
        //			var flag6 = flag4;
        //			if (flag6)
        //			{
        //				scope.Dispose();
        //			}
        //		}
        //		result = fingerprintData;
        //	}
        //	return result;
        //}

        public List<FingerprintData> Extract(FingerprintData fingerprintData, UnmanegedMemoryScope scope = null)
        {
            var result = new List<FingerprintData>();
            var flag = false;
            var flag2 = scope == null;
            if (flag2)
            {
                flag = true;
                scope = UnmanegedMemoryScope.Begin();
            }
            try
            {
                using (var safeExportData = ConvertToExportData(fingerprintData))
                {
                    result = (from n in Extract(safeExportData.ExportData, scope)
                              select ConvertToFingerprintData(n)).ToList<FingerprintData>();
                }
            }
            finally
            {
                var flag3 = flag;
                if (flag3)
                {
                    scope.Dispose();
                }
            }
            return result;
        }

        public bool Equals(FingerprintData f1, FingerprintData f2)
        {
            var hasValidHandle = HasValidHandle;
            if (!hasValidHandle)
            {
                throw new InvalidOperationException("UCBioBSP not initialized");
            }
            var flag = f1 == null;
            if (flag)
            {
                throw new ArgumentNullException("f1");
            }
            var flag2 = f2 == null;
            if (flag2)
            {
                throw new ArgumentNullException("f2");
            }
            var apiReturn = ApiReturn.BaseGeneral;
            InputFir? inputFir = null;
            InputFir? inputFir2 = null;
            bool result;
            try
            {
                inputFir = new InputFir?(f1.GetInputFir());
                inputFir2 = new InputFir?(f2.GetInputFir());
                var value = inputFir.Value;
                var value2 = inputFir2.Value;
                apiReturn = VerifyMatch(prvHandle, ref value, ref value2, out result, IntPtr.Zero);
            }
            finally
            {
                var hasValue = inputFir.HasValue;
                if (hasValue)
                {
                    inputFir.Value.Release();
                }
                var hasValue2 = inputFir2.HasValue;
                if (hasValue2)
                {
                    inputFir2.Value.Release();
                }
            }
            var flag3 = apiReturn > ApiReturn.BaseGeneral;
            if (flag3)
            {
                throw new InvalidOperationException(string.Format("Can not convert (Template finger) to finger - UError Code:{0} - ErrorName:{1}", (uint)apiReturn, apiReturn));
            }
            return result;
        }

        public void Dispose()
        {
            var flag = prvFastSearchEngine != null;
            if (flag)
            {
                ((IDisposable)prvFastSearchEngine).Dispose();
            }
            prvHandle.Close();
        }

        public void SetInitialOptions(InitialOptions options)
        {
            var hasValidHandle = HasValidHandle;
            if (!hasValidHandle)
            {
                throw new InvalidOperationException("UCBioBSP not initialized");
            }
            var initInfo = options.InitInfo;
            var apiReturn = SetInitInfo(prvHandle, 0, ref initInfo);
            var flag = apiReturn > ApiReturn.BaseGeneral;
            if (flag)
            {
                throw new InvalidOperationException(string.Format("Can not convert (Template finger) to finger - UError Code:{0} - ErrorName:{1}", (uint)apiReturn, apiReturn));
            }
        }

        public InitialOptions GetInitialOptions()
        {
            var hasValidHandle = HasValidHandle;
            if (!hasValidHandle)
            {
                throw new InvalidOperationException("UCBioBSP not initialized");
            }
            InitInfo0 initInfo2;
            var initInfo = GetInitInfo(prvHandle, 0, out initInfo2);
            var flag = initInfo > ApiReturn.BaseGeneral;
            if (flag)
            {
                throw new InvalidOperationException(string.Format("Can not convert (Template finger) to finger - UError Code:{0} - ErrorName:{1}", (uint)initInfo, initInfo));
            }
            return new InitialOptions
            {
                InitInfo = initInfo2
            };
        }

        public FastSearch FastSearchEngine
        {
            get
            {
                var hasValidHandle = HasValidHandle;
                if (hasValidHandle)
                {
                    var flag = prvFastSearchEngine == null;
                    if (flag)
                    {
                        prvFastSearchEngine = new FastSearch(this);
                    }
                    return prvFastSearchEngine;
                }
                throw new InvalidOperationException("UCBioBSP not initialized");
            }
        }

        public byte GetQuality(FingerprintData data)
        {
            var safeExportData = ConvertToExportData(data);
            var fingerInfo = safeExportData.ExportData.GetFingerInfo();
            var templateInfoes = fingerInfo[0].GetTemplateInfoes(safeExportData.ExportData.SamplesPerFinger);
            var data2 = templateInfoes[0].GetData();
            var templateType = safeExportData.ExportData.TemplateType;
            byte result;
            if (templateType != TemplateSize.Size400)
            {
                if (templateType != TemplateSize.Size500 && templateType != TemplateSize.Size600)
                {
                    result = 0;
                }
                else
                {
                    result = data2[26];
                }
            }
            else
            {
                result = data2[13];
            }
            return result;
        }

        public UnionSafeHandle prvHandle;

        private FastSearch prvFastSearchEngine;

        public class InputDevice
        {
            [DllImport("UCBioBSP.dll", CharSet = CharSet.Ansi, EntryPoint = "UCBioAPI_CheckFinger")]
            private static extern ApiReturn CheckFinger(UnionSafeHandle hHandle, out bool pbFingerExist);

            [DllImport("UCBioBSP.dll", CharSet = CharSet.Ansi, EntryPoint = "UCBioAPI_GetDeviceInfo")]
            private static extern ApiReturn GetDeviceInfo(UnionSafeHandle hHandle, ushort nDeviceID, byte nStructureType, DeviceSettings pDeviceInfo);

            [DllImport("UCBioBSP.dll", CharSet = CharSet.Ansi, EntryPoint = "UCBioAPI_SetDeviceInfo")]
            private static extern ApiReturn SetDeviceInfo(UnionSafeHandle hHandle, ushort nDeviceID, byte nStructureType, DeviceSettings pDeviceInfo);

            [DllImport("UCBioBSP.dll", CharSet = CharSet.Ansi, EntryPoint = "UCBioAPI_OpenDevice")]
            private static extern ApiReturn OpenDevice(UnionSafeHandle handle, ushort deviceID);

            [DllImport("UCBioBSP.dll", CharSet = CharSet.Ansi, EntryPoint = "UCBioAPI_CloseDevice")]
            private static extern ApiReturn CloseDevice(UnionSafeHandle handle, ushort deviceID);

            [DllImport("UCBioBSP.dll", CharSet = CharSet.Ansi, EntryPoint = "UCBioAPI_Capture")]
            private static extern ApiReturn Capture(UnionSafeHandle handle, FirPurpose purpose, out uint hFinger, int timeout, out uint phAuditData, WindowOption option);

            //[DllImport("UCBioBSP.dll", CharSet = CharSet.Ansi, EntryPoint = "UCBioAPI_AuditFIRToImage")]
            //private static extern ApiReturn AuditFIRToImage(UnionSafeHandle hHandle, ref InputFir piCapturedFIR, out ExportAuditData exportData);

            //[DllImport("UCBioBSP.dll", CharSet = CharSet.Ansi, EntryPoint = "UCBioAPI_FreeExportAuditData")]
            //private static extern ApiReturn FreeExportAuditData(ref ExportAuditData exportData);

            [DllImport("UCBioBSP.dll", CharSet = CharSet.Ansi, EntryPoint = "UCBioAPI_FreeFIRHandle")]
            private static extern ApiReturn FreeFIRHandle(uint hHandle);

            [method: CompilerGenerated]
            [CompilerGenerated]
            public event EventHandler<BeforeCaptureEventArgs> BeforeCapture;

            [method: CompilerGenerated]
            [CompilerGenerated]
            public event EventHandler<AfterCaptureEventArgs> AfterCapture;

            internal InputDevice(UCBioApi uCBioApi, DeviceInfo deviceInfo)
            {
                prvDeviceID = deviceInfo.DeviceID;
                prvUCBioApi = uCBioApi;
                Name = deviceInfo.Name;
                Description = deviceInfo.Description;
                OpenDevice();
                prvSettings = new DeviceSettings();
                GetDeviceInfo(prvUCBioApi.prvHandle, prvDeviceID, 0, Settings);
                CloseDevice();
                ImageHeight = prvSettings.ImageHeight;
                ImageWidth = prvSettings.ImageWidth;
                prvWindowOption = new WindowOption();
                //prvWindowOption.CaptureCallBack = new CaptureCallBack(OnCapture);
            }

            public bool CheckFinger()
            {
                var isOpen = IsOpen;
                var flag = !isOpen;
                if (flag)
                {
                    OpenDevice();
                }
                bool result;
                var apiReturn = CheckFinger(prvUCBioApi.prvHandle, out result);
                var flag2 = !isOpen;
                if (flag2)
                {
                    CloseDevice();
                }
                var flag3 = apiReturn > ApiReturn.BaseGeneral;
                if (flag3)
                {
                    throw new InvalidOperationException(string.Format("Can not check for finger - VirdiError Code:{0} - ErrorName:{1}", (uint)apiReturn, apiReturn));
                }
                return result;
            }

            //public ApiReturn Capture(ushort timeout, bool autoDetect, out FingerprintData fingerprint)
            //{
            //    return Capture(timeout, autoDetect, FingerID.Unknown, out fingerprint);
            //}

            //private ApiReturn OnCapture(IntPtr param, IntPtr userState)
            //{
            //    var beforeCapture = BeforeCapture;
            //    var flag = beforeCapture != null;
            //    ApiReturn result;
            //    if (flag)
            //    {
            //        var windowCallbackParamOne = (WindowCallbackParamOne)Marshal.PtrToStructure(param, typeof(WindowCallbackParamOne));
            //        var beforeCaptureEventArgs = new BeforeCaptureEventArgs((byte)windowCallbackParamOne.Quality, windowCallbackParamOne.GetImage((int)ImageWidth, (int)ImageHeight), windowCallbackParamOne.DeviceError);
            //        beforeCapture(this, beforeCaptureEventArgs);
            //        var cancel = beforeCaptureEventArgs.Cancel;
            //        if (cancel)
            //        {
            //            result = ApiReturn.DataProcessFail;
            //        }
            //        else
            //        {
            //            result = ApiReturn.BaseGeneral;
            //        }
            //    }
            //    else
            //    {
            //        result = ApiReturn.BaseGeneral;
            //    }
            //    return result;
            //}

            //public ApiReturn Capture(ushort timeout, bool autoDetect, FingerID fingerID, out FingerprintData fingerprint)
            //{
            //    fingerprint = null;
            //    var hasValidHandle = prvUCBioApi.HasValidHandle;
            //    if (hasValidHandle)
            //    {
            //        var isOpen = IsOpen;
            //        var flag = !isOpen;
            //        if (flag)
            //        {
            //            OpenDevice();
            //        }
            //        SetAutoDetect(prvUCBioApi.prvHandle, autoDetect);
            //        uint num;
            //        uint num2;
            //        var apiReturn = Capture(prvUCBioApi.prvHandle, FirPurpose.Verify, out num, (int)timeout, out num2, prvWindowOption);
            //        var flag2 = !isOpen;
            //        if (flag2)
            //        {
            //            CloseDevice();
            //        }
            //        var flag3 = apiReturn == ApiReturn.BaseGeneral && num != (uint)(int)IntPtr.Zero;
            //        if (flag3)
            //        {
            //            byte[][] images = null;
            //            var inputFir = default(InputFir);
            //            inputFir.FirHandle = num2;
            //            ExportAuditData exportAuditData;
            //            var apiReturn2 = AuditFIRToImage(prvUCBioApi.prvHandle, ref inputFir, out exportAuditData);
            //            var flag4 = apiReturn2 == ApiReturn.BaseGeneral;
            //            if (flag4)
            //            {
            //                var auditData = exportAuditData.AuditData;
            //                images = auditData[0].GetImageData((int)exportAuditData.SamplesPerFinger, (int)exportAuditData.ImageWidth, (int)exportAuditData.ImageHeight);
            //                FreeExportAuditData(ref exportAuditData);
            //            }
            //            FreeFIRHandle(num2);
            //            fingerprint = new FingerprintData(num, prvLastTemplateFormat, 0);
            //            using (var safeExportData = prvUCBioApi.ConvertToExportData(fingerprint))
            //            {
            //                var fingerInfo = safeExportData.ExportData.GetFingerInfo();
            //                fingerInfo[0].FingerID = fingerID;
            //                safeExportData.ExportData.DangerousSetFingerInfo(fingerInfo);
            //                fingerprint = prvUCBioApi.ConvertToFingerprintData(safeExportData.ExportData);
            //            }
            //        }
            //        else
            //        {
            //            var flag5 = apiReturn == ApiReturn.BaseGeneral && num == (uint)(int)IntPtr.Zero;
            //            if (flag5)
            //            {
            //                apiReturn = ApiReturn.InvalidPointer;
            //            }
            //        }
            //        var afterCapture = AfterCapture;
            //        var flag6 = afterCapture != null;
            //        if (flag6)
            //        {
            //            var afterCaptureEventArgs = new AfterCaptureEventArgs(fingerprint, apiReturn);
            //        }
            //        return apiReturn;
            //    }
            //    throw new InvalidOperationException("UCBioBSP not initializedd");
            //}

            public void OpenDevice()
            {
                var hasValidHandle = prvUCBioApi.HasValidHandle;
                if (!hasValidHandle)
                {
                    throw new InvalidOperationException("UCBioBSP is not initialized");
                }
                var isOpen = IsOpen;
                if (isOpen)
                {
                    throw new InvalidOperationException("Device is already open");
                }
                var apiReturn = OpenDevice(prvUCBioApi.prvHandle, prvDeviceID);
                var flag = apiReturn > ApiReturn.BaseGeneral;
                if (flag)
                {
                    throw new InvalidOperationException(string.Format("Can not open device - VirdiError Code:{0} - ErrorName:{1}", (uint)apiReturn, apiReturn));
                }
                var initialOptions = prvUCBioApi.GetInitialOptions();
                prvLastTemplateFormat = initialOptions.TemlateFormat;
                IsOpen = true;
            }

            public void CloseDevice()
            {
                var hasValidHandle = prvUCBioApi.HasValidHandle;
                if (!hasValidHandle)
                {
                    throw new InvalidOperationException("UCBioBSP is not initialized");
                }
                var flag = !IsOpen;
                if (flag)
                {
                    throw new InvalidOperationException("Device is already closed");
                }
                var apiReturn = CloseDevice(prvUCBioApi.prvHandle, prvDeviceID);
                var flag2 = apiReturn > ApiReturn.BaseGeneral;
                if (flag2)
                {
                    throw new InvalidOperationException(string.Format("Can not close device - VirdiError Code:{0} - ErrorName:{1}", (uint)apiReturn, apiReturn));
                }
                IsOpen = false;
            }

            public bool IsOpen
            {
                get;
                private set;
            }

            public string Name
            {
                get;
                private set;
            }

            public uint ImageWidth
            {
                get;
                private set;
            }

            public uint ImageHeight
            {
                get;
                private set;
            }

            public string Description
            {
                get;
                private set;
            }

            public DeviceSettings Settings
            {
                get
                {
                    var isOpen = IsOpen;
                    var flag = !isOpen;
                    if (flag)
                    {
                        OpenDevice();
                    }
                    var deviceInfo = GetDeviceInfo(prvUCBioApi.prvHandle, prvDeviceID, 0, prvSettings);
                    var flag2 = !isOpen;
                    if (flag2)
                    {
                        CloseDevice();
                    }
                    var flag3 = deviceInfo > ApiReturn.BaseGeneral;
                    if (flag3)
                    {
                        throw new InvalidOperationException(string.Format("Can not get settings from device - VirdiError Code:{0} - ErrorName:{1}", (uint)deviceInfo, deviceInfo));
                    }
                    return prvSettings;
                }
                set
                {
                    var flag = value == null;
                    if (flag)
                    {
                        throw new ArgumentNullException("value");
                    }
                    var isOpen = IsOpen;
                    var flag2 = !isOpen;
                    if (flag2)
                    {
                        OpenDevice();
                    }
                    var apiReturn = SetDeviceInfo(prvUCBioApi.prvHandle, prvDeviceID, 0, prvSettings);
                    var flag3 = !isOpen;
                    if (flag3)
                    {
                        CloseDevice();
                    }
                    var flag4 = apiReturn > ApiReturn.BaseGeneral;
                    if (flag4)
                    {
                        throw new InvalidOperationException(string.Format("Can not set settings to device - VirdiError Code:{0} - ErrorName:{1}", (uint)apiReturn, apiReturn));
                    }
                    prvSettings = value;
                }
            }

            private UCBioApi prvUCBioApi;

            private ushort prvDeviceID;

            private DeviceSettings prvSettings;

            private WindowOption prvWindowOption;

            private TemplateFormat prvLastTemplateFormat;

            [StructLayout(LayoutKind.Sequential)]
            public class DeviceSettings
            {
                public uint Brightness
                {
                    get
                    {
                        return prvBrightness;
                    }
                    set
                    {
                        prvBrightness = value;
                    }
                }

                public uint Contrast
                {
                    get
                    {
                        return prvContrast;
                    }
                    set
                    {
                        prvContrast = value;
                    }
                }

                public uint Gain
                {
                    get
                    {
                        return prvGain;
                    }
                    set
                    {
                        prvGain = value;
                    }
                }

                private uint prvStructureType;

                internal uint ImageWidth;

                internal uint ImageHeight;

                private uint prvBrightness;

                private uint prvContrast;

                private uint prvGain;
            }
        }

        public class FastSearch : IDisposable
        {
            [DllImport("UCBioBSP.dll", CharSet = CharSet.Ansi, EntryPoint = "UCBioAPI_InitFastSearchEngine")]
            private static extern ApiReturn InitFastSearchEngine(UnionSafeHandle handle);

            [DllImport("UCBioBSP.dll", CharSet = CharSet.Ansi, EntryPoint = "UCBioAPI_TerminateFastSearchEngine")]
            private static extern ApiReturn TerminateFastSearchEngine(UnionSafeHandle handle);

            [DllImport("UCBioBSP.dll", CharSet = CharSet.Ansi, EntryPoint = "UCBioAPI_AddFIRToFastSearchDB")]
            private static extern ApiReturn AddFIRToFastSearchDB(UnionSafeHandle handle, ref InputFir inputFir, uint userID, IntPtr sampleInfo);

            [DllImport("UCBioBSP.dll", CharSet = CharSet.Ansi, EntryPoint = "UCBioAPI_IdentifyFIRFromFastSearchDB")]
            private static extern ApiReturn IdentifyFIRFromFastSearchDB(UnionSafeHandle handle, ref InputFir inputfir, [MarshalAs(UnmanagedType.U1)] SecurityLevel SecurityLevel, out FpInfo fastsearchFpInfo, IntPtr callbackInfo0);

            [DllImport("UCBioBSP.dll", CharSet = CharSet.Ansi, EntryPoint = "UCBioAPI_ClearFastSearchDB")]
            private static extern ApiReturn ClearFastSearchDB(UnionSafeHandle handle);

            [DllImport("UCBioBSP.dll", CharSet = CharSet.Ansi, EntryPoint = "UCBioAPI_RemoveUserFromFastSearchDB")]
            private static extern ApiReturn RemoveUserFromFastSearchDB(UnionSafeHandle hHandle, uint nUserID);

            [DllImport("UCBioBSP.dll", CharSet = CharSet.Ansi, EntryPoint = "UCBioAPI_GetFpCountFromFastSearchDB")]
            private static extern ApiReturn GetFpCountFromFastSearchDB(UnionSafeHandle hHandle, out uint pDataCount);

            [DllImport("UCBioBSP.dll", CharSet = CharSet.Ansi, EntryPoint = "UCBioAPI_GetFastSearchInitInfo")]
            private static extern ApiReturn GetFastSearchInitInfo(UnionSafeHandle hHandle, [MarshalAs(UnmanagedType.U1)] byte nStructureType, out InitInfo0 initInfo);

            [DllImport("UCBioBSP.dll", CharSet = CharSet.Ansi, EntryPoint = "UCBioAPI_SetFastSearchInitInfo")]
            private static extern ApiReturn SetFastSearchInitInfo(UnionSafeHandle hHandle, [MarshalAs(UnmanagedType.U1)] byte nStructureType, ref InitInfo0 initInfo);

            [DllImport("UCBioBSP.dll", CharSet = CharSet.Ansi, EntryPoint = "UCBioAPI_SaveFastSearchDBToFile")]
            private static extern ApiReturn SaveFastSearchDBToFile(UnionSafeHandle hHandle, [MarshalAs(UnmanagedType.LPStr)] string szFilepath);

            [DllImport("UCBioBSP.dll", CharSet = CharSet.Ansi, EntryPoint = "UCBioAPI_LoadFastSearchDBToFile")]
            private static extern ApiReturn LoadFastSearchDBToFile(UnionSafeHandle hHandle, [MarshalAs(UnmanagedType.LPStr)] string szFilepath);

            internal FastSearch(UCBioApi ucbioapi)
            {
                prvUCBioApi = ucbioapi;
                prvFingerprintBuffer = new Dictionary<uint, ulong>();
                prvIndex = 0;
                var hasValidHandle = prvUCBioApi.HasValidHandle;
                if (!hasValidHandle)
                {
                    throw new InvalidOperationException("UCBioBSP not initializedd");
                }
                var apiReturn = InitFastSearchEngine(prvUCBioApi.prvHandle);
                var flag = apiReturn > ApiReturn.BaseGeneral;
                if (flag)
                {
                    throw new InvalidOperationException(string.Format("Can not initialize fast search engine - VirdiError Code:{0} - ErrorName:{1}", (uint)apiReturn, apiReturn));
                }
            }

            public void ClearItems()
            {
                var hasValidHandle = prvUCBioApi.HasValidHandle;
                if (hasValidHandle)
                {
                    var obj = prvFingerprintBuffer;
                    lock (obj)
                    {
                        var apiReturn = ClearFastSearchDB(prvUCBioApi.prvHandle);
                        var flag2 = apiReturn > ApiReturn.BaseGeneral;
                        if (flag2)
                        {
                            throw new InvalidOperationException(string.Format("Can not clear Fast Search Engine - UError Code:{0} - ErrorName:{1}", (uint)apiReturn, apiReturn.ToString()));
                        }
                        prvFingerprintBuffer.Clear();
                        prvIndex = 0;
                    }
                    return;
                }
                throw new InvalidOperationException("UCBioBSP not initializedd");
            }

            public void RemoveItem(FingerprintData item)
            {
                var hasValidHandle = prvUCBioApi.HasValidHandle;
                if (hasValidHandle)
                {
                    var flag = prvFingerprintBuffer.ContainsValue(item.UniqueID);
                    if (flag)
                    {
                        var keyValuePair = prvFingerprintBuffer.FirstOrDefault((KeyValuePair<uint, ulong> n) => n.Value == item.UniqueID);
                        var apiReturn = RemoveUserFromFastSearchDB(prvUCBioApi.prvHandle, keyValuePair.Key);
                        var flag2 = apiReturn > ApiReturn.BaseGeneral;
                        if (flag2)
                        {
                            throw new InvalidOperationException(string.Format("Can not Add Finger to Fast Search database - UError Code:{0} - ErrorName:{1}", (uint)apiReturn, apiReturn.ToString()));
                        }
                        prvFingerprintBuffer.Remove(keyValuePair.Key);
                    }
                    return;
                }
                throw new InvalidOperationException("UCBioBSP not initializedd");
            }

            public void RemoveItem(ulong uniqueID)
            {
                var hasValidHandle = prvUCBioApi.HasValidHandle;
                if (hasValidHandle)
                {
                    var flag = prvFingerprintBuffer.ContainsValue(uniqueID);
                    if (flag)
                    {
                        var keyValuePair = prvFingerprintBuffer.First((KeyValuePair<uint, ulong> n) => n.Value == uniqueID);
                        var apiReturn = RemoveUserFromFastSearchDB(prvUCBioApi.prvHandle, keyValuePair.Key);
                        var flag2 = apiReturn > ApiReturn.BaseGeneral;
                        if (flag2)
                        {
                            throw new InvalidOperationException(string.Format("Can not Add Finger to Fast Search database - UError Code:{0} - ErrorName:{1}", (uint)apiReturn, apiReturn.ToString()));
                        }
                        prvFingerprintBuffer.Remove(keyValuePair.Key);
                    }
                    return;
                }
                throw new InvalidOperationException("UCBioBSP not initializedd");
            }

            public void AddItem(FingerprintData item)
            {
                var hasValidHandle = prvUCBioApi.HasValidHandle;
                if (!hasValidHandle)
                {
                    throw new InvalidOperationException("UCBioBSP not initializedd");
                }
                var apiReturn = ApiReturn.BaseGeneral;
                var inputFIR = item.GetInputFir();
                var num = Interlocked.Increment(ref prvIndex);
                try
                {
                    prvFingerprintBuffer.Add((uint)num, item.UniqueID);
                    apiReturn = AddFIRToFastSearchDB(prvUCBioApi.prvHandle, ref inputFIR, (uint)num, IntPtr.Zero);
                }
                finally
                {
                    inputFIR.Release();
                }
                var flag = apiReturn > ApiReturn.BaseGeneral;
                if (flag)
                {
                    throw new InvalidOperationException(string.Format("Can not Add Finger to Fast Search database - UError Code:{0} - ErrorName:{1}", (uint)apiReturn, apiReturn.ToString()));
                }
            }

            public ulong? First(FingerprintData item, SecurityLevel level = SecurityLevel.BelowNormal)
            {
                var hasValidHandle = prvUCBioApi.HasValidHandle;
                if (hasValidHandle)
                {
                    var num = 0u;
                    GetFpCountFromFastSearchDB(prvUCBioApi.prvHandle, out num);
                    var inputFIR = item.GetInputFir();
                    var apiReturn = ApiReturn.BaseGeneral;
                    FpInfo fPInfo;
                    try
                    {
                        apiReturn = IdentifyFIRFromFastSearchDB(prvUCBioApi.prvHandle, ref inputFIR, level, out fPInfo, IntPtr.Zero);
                    }
                    finally
                    {
                    }
                    var flag = apiReturn == ApiReturn.BaseGeneral;
                    ulong? result;
                    if (flag)
                    {
                        result = new ulong?(prvFingerprintBuffer[fPInfo.ID]);
                    }
                    else
                    {
                        var flag2 = apiReturn == ApiReturn.FastsearchIdentifyFail;
                        if (!flag2)
                        {
                            throw new InvalidOperationException(string.Format("Can not Identify Finger In Fast Search database - UError Code:{0} - ErrorName:{1}", (uint)apiReturn, apiReturn.ToString()));
                        }
                        result = null;
                    }
                    return result;
                }
                throw new InvalidOperationException("UCBioBSP not initializedd");
            }

            public void Load(string filePath)
            {
                var flag = File.Exists(filePath);
                if (!flag)
                {
                    throw new ArgumentException("File not Exist");
                }
                var hasValidHandle = prvUCBioApi.HasValidHandle;
                if (!hasValidHandle)
                {
                    throw new InvalidOperationException("UCBioBSP not initializedd");
                }
                var apiReturn = LoadFastSearchDBToFile(prvUCBioApi.prvHandle, filePath);
                var flag2 = apiReturn > ApiReturn.BaseGeneral;
                if (flag2)
                {
                    throw new InvalidOperationException(string.Format("Can not Loading Fast Search database - UError Code:{0} - ErrorName:{1}", (uint)apiReturn, apiReturn.ToString()));
                }
            }

            public void Save(string filePath)
            {
                var hasValidHandle = prvUCBioApi.HasValidHandle;
                if (!hasValidHandle)
                {
                    throw new InvalidOperationException("UCBioBSP not initializedd");
                }
                var apiReturn = SaveFastSearchDBToFile(prvUCBioApi.prvHandle, filePath);
                var flag = apiReturn > ApiReturn.BaseGeneral;
                if (flag)
                {
                    throw new InvalidOperationException(string.Format("Can not Save Fast Search database - UError Code:{0} - ErrorName:{1}", (uint)apiReturn, apiReturn.ToString()));
                }
            }

            public int Count
            {
                get
                {
                    var hasValidHandle = prvUCBioApi.HasValidHandle;
                    if (!hasValidHandle)
                    {
                        throw new InvalidOperationException("UCBioBSP not initializedd");
                    }
                    uint result;
                    var fpCountFromFastSearchDB = GetFpCountFromFastSearchDB(prvUCBioApi.prvHandle, out result);
                    var flag = fpCountFromFastSearchDB == ApiReturn.BaseGeneral;
                    if (flag)
                    {
                        return (int)result;
                    }
                    throw new InvalidOperationException(string.Format("Can not get Fingerprint count from Fast Search database - UError Code:{0} - ErrorName:{1}", (uint)fpCountFromFastSearchDB, fpCountFromFastSearchDB.ToString()));
                }
            }

            void IDisposable.Dispose()
            {
                var hasValidHandle = prvUCBioApi.HasValidHandle;
                if (hasValidHandle)
                {
                    var apiReturn = TerminateFastSearchEngine(prvUCBioApi.prvHandle);
                }
            }

            private UCBioApi prvUCBioApi;

            private Dictionary<uint, ulong> prvFingerprintBuffer;

            private int prvIndex;
        }
    }
}
