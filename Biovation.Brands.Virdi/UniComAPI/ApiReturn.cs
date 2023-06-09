﻿namespace Biovation.Brands.Virdi.UniComAPI
{
	public enum ApiReturn : uint
	{
		BaseGeneral,
		BaseInit = 256u,
		BaseDevice = 512u,
		BaseUi = 768u,
		BaseFastsearch = 1024u,
		BaseOptional = 1280u,
		BaseSmartcard = 1536u,
		None = 0u,
		InvalidHandle,
		InvalidPointer,
		InvalidType,
		FunctionFail,
		StructtypeNotMatched,
		AlreadyProcessed,
		ExtractionOpenFail,
		VerificationOpenFail,
		DataProcessFail,
		MustBeProcessedData,
		InternalChecksumFail,
		EncryptedDataError,
		UnknownFormat,
		UnknownVersion,
		ValidityFail,
		InvalidTemplatesize,
		InvalidTemplate,
		ExpiredVersion,
		InvalidSamplesperfinger,
		UnknownInputformat,
		InvalidParameter,
		FunctionNotSupported,
		InitMaxfingersforenroll = 257u,
		InitNecessaryenrollnum,
		InitSamplesperfinger,
		InitSeculevelforenroll,
		InitSeculevelforverify,
		InitSeculevelforidentify,
		InitReserved1,
		InitReserved2,
		DeviceOpenFail = 513u,
		InvalidDeviceID,
		WrongDeviceID,
		DeviceAlreadyOpened,
		DeviceNotOpened,
		DeviceBrightness,
		DeviceContrast,
		DeviceGain,
		UserCancel = 769u,
		UserBack,
		CaptureTimeout,
		CaptureFakeSuspicious,
		EnrollEventPlace,
		EnrollEventHold,
		EnrollEventRemove,
		EnrollEventPlaceAgain,
		EnrollEventProcess,
		EnrollEventMatchFailed,
		FastsearchInitFail = 1025u,
		FastsearchSaveDb,
		FastsearchLoadDb,
		FastsearchUnknownVer,
		FastsearchIdentifyFail,
		FastsearchDuplicatedID,
		FastsearchIdentifyStop,
		FastsearchNouserExist,
		OptionalUuidFail = 1281u,
		OptionalPin1Fail,
		OptionalPin2Fail,
		OptionalSiteidFail,
		OptionalExpireDateFail,
		ScFunctionFailed = 1537u,
		ScNotSupportedDevice,
		ScNotSupportedFirmware
	}
}
