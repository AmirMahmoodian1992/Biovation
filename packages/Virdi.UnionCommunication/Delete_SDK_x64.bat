cd /d %~dp0
if /i "%PROCESSOR_IDENTIFIER:~0,3%"=="X86" (
	echo system is x86, new sdk is x64 
	del %windir%\System32\FPLib.dll
	del %windir%\System32\Interop.UCBioBSPCOMLib.dll
	del %windir%\System32\Interop.UCSAPICOMLib.dll
	del %windir%\System32\UCBioBSP.dll
	del %windir%\System32\UCBioBSPCOM.dll
	del %windir%\System32\UCSAPI40.dll
	del %windir%\System32\UCSAPICOM.dll
	del %windir%\System32\UNIONCOMM.SDK.UCBioBSP.dll
	del %windir%\System32\VHMLib.dll
	del %windir%\System32\WSEngine.dll
	regsvr32 %windir%\System32\UCBioBSPCOM.dll -u
	regsvr32 %windir%\System32\UCSAPICOM.dll -u
	) else (
		echo system is x64, new sdk is x64, delete all
		del %windir%\System32\FPLib.dll
		del %windir%\System32\Interop.UCBioBSPCOMLib.dll
		del %windir%\System32\Interop.UCSAPICOMLib.dll
		del %windir%\System32\UCBioBSP.dll
		del %windir%\System32\UCBioBSPCOM.dll
		del %windir%\System32\UCSAPI40.dll
		del %windir%\System32\UCSAPICOM.dll
		del %windir%\System32\UNIONCOMM.SDK.UCBioBSP.dll
		del %windir%\System32\VHMLib.dll
		del %windir%\System32\WSEngine.dll
		regsvr32 %windir%\System32\UCBioBSPCOM.dll -u
		regsvr32 %windir%\System32\UCSAPICOM.dll -u
		del %windir%\SysWOW64\FPLib.dll
		del %windir%\SysWOW64\Interop.UCBioBSPCOMLib.dll
		del %windir%\SysWOW64\Interop.UCSAPICOMLib.dll
		del %windir%\SysWOW64\UCBioBSP.dll
		del %windir%\SysWOW64\UCBioBSPCOM.dll
		del %windir%\SysWOW64\UCSAPI40.dll
		del %windir%\SysWOW64\UCSAPICOM.dll
		del %windir%\SysWOW64\UNIONCOMM.SDK.UCBioBSP.dll
		del %windir%\SysWOW64\VHMLib.dll
		del %windir%\SysWOW64\WSEngine.dll

		regsvr32 %windir%\SysWOW64\UCBioBSPCOM.dll -u
		regsvr32 %windir%\SysWOW64\UCSAPICOM.dll -u
	)
pause