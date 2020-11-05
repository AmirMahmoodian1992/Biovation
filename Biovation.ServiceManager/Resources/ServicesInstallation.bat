@ECHO OFF
SET basePath=%~1
SET installationMode=%~2
SET selectedService=%~3
SET instanceName=%~4
IF "%basePath%"=="" SET basePath=%~dp0
IF "%installationMode%"=="" SET installationMode="Release"
REM ECHO %basePath%

IF "%selectedService%" NEQ "" GOTO %selectedService%
IF "%instanceName%" NEQ "" (
	ECHO A service name should be provided when you want to install an instance, please set the third property for the service name
	GOTO EOF
)

:: Section 1: Biovation gateway service
:GATEWAY
ECHO ====================
ECHO Installing Biovation Gateway Service
ECHO ====================

SET serviceFileName=Biovation.Gateway
IF "%installationMode%"=="Debug" (SET binaryPath=%basePath%%serviceFileName%\bin\Debug\netcoreapp3.1\%serviceFileName%.exe) ELSE SET binaryPath=%basePath%%serviceFileName%\%serviceFileName%.exe

IF "%instanceName%" NEQ "" (SET serviceName=Biovation.Gateway$%instanceName%)			ELSE (SET serviceName=Biovation.Gateway)
IF "%instanceName%" NEQ "" (SET serviceDisplayName=Biovation Gateway (%instanceName%^)) ELSE (SET serviceDisplayName=Biovation Gateway)

SC QUERY "%serviceName%" > NUL
IF ERRORLEVEL 1060 GOTO BIOVATION GATEWAY MISSING
ECHO BIOVATION GATEWAY EXISTS
GOTO BIOVATION GATEWAY EXISTS

:BIOVATION GATEWAY EXISTS

IF EXIST %binaryPath% (
	for /F "tokens=3 delims=: " %%H in ('sc query "%serviceName%" ^| findstr "        STATE"') do (
	  if /I "%%H" NEQ "STOPPED" (
	   ECHO Stopping 
	   SC stop "%serviceName%"
	  )
	)

	SC delete %serviceName%)

:BIOVATION GATEWAY MISSING


IF EXIST %binaryPath% (
	ECHO Installing %serviceName% service
	SC create "%serviceName%" binPath= "%binaryPath%" DisplayName= "%serviceDisplayName%" start=auto
	SC description "%serviceName%" "The API gateway of Biovation system, handles the requests. (C) Kasra Co."
	SC failure "%serviceName%" reset=10800 actions=restart/15/restart/45//
) ELSE (
	ECHO [Error]: The binary file of %serviceName% service is not found in path:
	ECHO 	%binaryPath%
)

IF "%selectedService%" NEQ "" GOTO EOF


:: Section 2: Biovation queries service
:QUERIES
ECHO ====================
ECHO Installing Biovation Queries Service
ECHO ====================

SET serviceFileName=Biovation.Data.Queries
IF "%installationMode%"=="Debug" (SET binaryPath=%basePath%%serviceFileName%\bin\Debug\netcoreapp3.1\%serviceFileName%.exe) ELSE SET binaryPath=%basePath%%serviceFileName%\%serviceFileName%.exe

IF "%instanceName%" NEQ "" (SET serviceName=Biovation.Data.Queries$%instanceName%)		ELSE (SET serviceName=Biovation.Data.Queries)	 
IF "%instanceName%" NEQ "" (SET serviceDisplayName=Biovation Queries (%instanceName%^)) ELSE (SET serviceDisplayName=Biovation Queries)

SC QUERY "%serviceName%" > NUL
IF ERRORLEVEL 1060 GOTO BIOVATION QUERIES MISSING
ECHO BIOVATION QUERIES EXISTS
GOTO BIOVATION QUERIES EXISTS

:BIOVATION QUERIES EXISTS

IF EXIST %binaryPath% (
	for /F "tokens=3 delims=: " %%H in ('sc query "%serviceName%" ^| findstr "        STATE"') do (
	  if /I "%%H" NEQ "STOPPED" (
	   ECHO Stopping 
	   SC stop "%serviceName%"
	  )
	)

	SC delete %serviceName%)

:BIOVATION QUERIES MISSING

IF EXIST %binaryPath% (
	ECHO Installing %serviceName% service
	SC create "%serviceName%" binPath= "%binaryPath%" DisplayName= "%serviceDisplayName%" start=auto
	SC description "%serviceName%" "The data manipulator controller of Biovation system, handles the data flow. (C) Kasra Co."
	SC failure "%serviceName%" reset=10800 actions=restart/15/restart/45//
) ELSE (
	ECHO [Error]: The binary file of %serviceName% service is not found in path:
	ECHO 	%binaryPath%
)

IF "%selectedService%" NEQ "" GOTO EOF


:: Section 3: Biovation commands service
:COMMANDS
ECHO ====================
ECHO Installing Biovation Commands Service
ECHO ====================

SET serviceFileName=Biovation.Data.Commands
IF "%installationMode%"=="Debug" (SET binaryPath=%basePath%%serviceFileName%\bin\Debug\netcoreapp3.1\%serviceFileName%.exe) ELSE SET binaryPath=%basePath%%serviceFileName%\%serviceFileName%.exe

IF "%instanceName%" NEQ "" (SET serviceName=Biovation.Data.Commands$%instanceName)		 ELSE (SET serviceName=Biovation.Data.Commands)  
IF "%instanceName%" NEQ "" (SET serviceDisplayName=Biovation Commands (%instanceName%^)) ELSE	(SET serviceDisplayName=Biovation Commands)

SC QUERY "%serviceName%" > NUL
IF ERRORLEVEL 1060 GOTO BIOVATION COMMANDS MISSING
ECHO BIOVATION COMMANDS EXISTS
GOTO BIOVATION COMMANDS EXISTS

:BIOVATION COMMANDS EXISTS

IF EXIST %binaryPath% (
	for /F "tokens=3 delims=: " %%H in ('sc query "%serviceName%" ^| findstr "        STATE"') do (
	  if /I "%%H" NEQ "STOPPED" (
	   ECHO Stopping 
	   SC stop "%serviceName%"
	  )
	)

	SC delete %serviceName%)

:BIOVATION COMMANDS MISSING

IF EXIST %binaryPath% (
	ECHO Installing %serviceName% service
	SC create "%serviceName%" binPath= "%binaryPath%" DisplayName= "%serviceDisplayName%" start=auto
	SC description "%serviceName%" "The fetch data controller of Biovation system, handles the data flow. (C) Kasra Co."
	SC failure "%serviceName%" reset=10800 actions=restart/15/restart/45//
) ELSE (
	ECHO [Error]: The binary file of %serviceName% service is not found in path:
	ECHO 	%binaryPath%
)

IF "%selectedService%" NEQ "" GOTO EOF


:: Section 4: Biovation server service
:SERVER
ECHO ====================
ECHO Installing Biovation Server Service
ECHO ====================

SET serviceFileName=Biovation.Server
IF "%installationMode%"=="Debug" (SET binaryPath=%basePath%%serviceFileName%\bin\Debug\netcoreapp3.1\%serviceFileName%.exe) ELSE SET binaryPath=%basePath%%serviceFileName%\%serviceFileName%.exe

IF "%instanceName%" NEQ "" (SET serviceName=Biovation.Server$%instanceName%)		   ELSE (SET serviceName=Biovation.Server)
IF "%instanceName%" NEQ "" (SET serviceDisplayName=Biovation Server (%instanceName%^)) ELSE (SET serviceDisplayName=Biovation Server)

SC QUERY "%serviceName%" > NUL
IF ERRORLEVEL 1060 GOTO BIOVATION SERVER MISSING
ECHO BIOVATION SERVER EXISTS
GOTO BIOVATION SERVER EXISTS

:BIOVATION SERVER EXISTS

IF EXIST %binaryPath% (
	for /F "tokens=3 delims=: " %%H in ('sc query "%serviceName%" ^| findstr "        STATE"') do (
	  if /I "%%H" NEQ "STOPPED" (
	   ECHO Stopping 
	   SC stop "%serviceName%"
	  )
	)

	SC delete %serviceName%)

:BIOVATION SERVER MISSING

IF EXIST %binaryPath% (
	ECHO Installing %serviceName% service
	SC create "%serviceName%" binPath= "%binaryPath%" DisplayName= "%serviceDisplayName%" start=auto
	SC description "%serviceName%" "The main and the manager service of Biovation system, handles the requests. (C) Kasra Co."
	SC failure "%serviceName%" reset=10800 actions=restart/15/restart/45//
) ELSE (
	ECHO [Error]: The binary file of %serviceName% service is not found in path:
	ECHO 	%binaryPath%
)

IF "%selectedService%" NEQ "" GOTO EOF


:: Section 5: Biovation EOS service
:EOS
ECHO ====================
ECHO Installing Biovation EOS Service
ECHO ====================

SET serviceFileName=Biovation.Brands.EOS
IF "%installationMode%"=="Debug" (SET binaryPath=%basePath%%serviceFileName%\bin\Debug\netcoreapp3.1\%serviceFileName%.exe) ELSE SET binaryPath=%basePath%%serviceFileName%\%serviceFileName%.exe

IF "%instanceName%" NEQ "" (SET serviceName=Biovation.Brands.EOS$%instanceName%)			ELSE (SET serviceName=Biovation.Brands.EOS)		 
IF "%instanceName%" NEQ "" (SET serviceDisplayName=Biovation EOS Manager (%instanceName%^)) ELSE (SET serviceDisplayName=Biovation EOS Manager)

SC QUERY "%serviceName%" > NUL
IF ERRORLEVEL 1060 GOTO BIOVATION EOS MISSING
ECHO BIOVATION EOS EXISTS
GOTO BIOVATION EOS EXISTS

:BIOVATION EOS EXISTS

IF EXIST %binaryPath% (
	for /F "tokens=3 delims=: " %%H in ('sc query "%serviceName%" ^| findstr "        STATE"') do (
	  if /I "%%H" NEQ "STOPPED" (
	   ECHO Stopping 
	   SC stop "%serviceName%"
	  )
	)

	SC delete %serviceName%)

:BIOVATION EOS MISSING

IF EXIST %binaryPath% (
	ECHO Installing %serviceName% service
	SC create "%serviceName%" binPath= "%binaryPath%" DisplayName= "%serviceDisplayName%" start=auto
	SC description "%serviceName%" "The Elm-o-Sanat device manager service of Biovation system, handles the devices' requests. (C) Kasra Co."
	SC failure "%serviceName%" reset=10800 actions=restart/15/restart/45//
) ELSE (
	ECHO [Error]: The binary file of %serviceName% service is not found in path:
	ECHO 	%binaryPath%
)

IF "%selectedService%" NEQ "" GOTO EOF


:: Section 6: Biovation PW service
:PW
ECHO ====================
ECHO Installing Biovation PW Service
ECHO ====================

SET serviceFileName=Biovation.Brands.PW
IF "%installationMode%"=="Debug" (SET binaryPath=%basePath%%serviceFileName%\bin\Debug\netcoreapp3.1\%serviceFileName%.exe) ELSE SET binaryPath=%basePath%%serviceFileName%\%serviceFileName%.exe

IF "%instanceName%" NEQ "" (SET serviceName=Biovation.Brands.PW$%instanceName%)			   ElSE (SET serviceName=Biovation.Brands.PW)		
IF "%instanceName%" NEQ "" (SET serviceDisplayName=Biovation PW Manager (%instanceName%^)) ElSE (SET serviceDisplayName=Biovation PW Manager)

SC QUERY "%serviceName%" > NUL
IF ERRORLEVEL 1060 GOTO BIOVATION PW MISSING
ECHO BIOVATION PW EXISTS
GOTO BIOVATION PW EXISTS

:BIOVATION PW EXISTS

IF EXIST %binaryPath% (
	for /F "tokens=3 delims=: " %%H in ('sc query "%serviceName%" ^| findstr "        STATE"') do (
	  if /I "%%H" NEQ "STOPPED" (
	   ECHO Stopping 
	   SC stop "%serviceName%"
	  )
	)

	SC delete %serviceName%)

:BIOVATION PW MISSING

IF EXIST %binaryPath% (
	ECHO Installing %serviceName% service
	SC create "%serviceName%" binPath= "%binaryPath%" DisplayName= "%serviceDisplayName%" start=auto
	SC description "%serviceName%" "The Processing World device manager service of Biovation system, handles the devices' requests. (C) Kasra Co."
	SC failure "%serviceName%" reset=10800 actions=restart/15/restart/45//
) ELSE (
	ECHO [Error]: The binary file of %serviceName% service is not found in path:
	ECHO 	%binaryPath%
)

IF "%selectedService%" NEQ "" GOTO EOF


:: Section 7: Biovation SHAHAB service
:SHAHAB
ECHO ====================
ECHO Installing Biovation Shahab Service
ECHO ====================

SET serviceFileName=Biovation.Brands.Shahab
IF "%installationMode%"=="Debug" (SET binaryPath=%basePath%%serviceFileName%\bin\Debug\netcoreapp3.1\%serviceFileName%.exe) ELSE SET binaryPath=%basePath%%serviceFileName%\%serviceFileName%.exe

IF "%instanceName%" NEQ "" (SET serviceName=Biovation.Brands.Shahab$%instanceName%)			   ELSE (SET serviceName=Biovation.Brands.Shahab)		
IF "%instanceName%" NEQ "" (SET serviceDisplayName=Biovation Shahab Manager (%instanceName%^)) ELSE (SET serviceDisplayName=Biovation Shahab Manager)

SC QUERY "%serviceName%" > NUL
IF ERRORLEVEL 1060 GOTO BIOVATION SHAHAB MISSING
ECHO BIOVATION SHAHAB EXISTS
GOTO BIOVATION SHAHAB EXISTS

:BIOVATION SHAHAB EXISTS

IF EXIST %binaryPath% (
	for /F "tokens=3 delims=: " %%H in ('sc query "%serviceName%" ^| findstr "        STATE"') do (
	  if /I "%%H" NEQ "STOPPED" (
	   ECHO Stopping 
	   SC stop "%serviceName%"
	  )
	)

	SC delete %serviceName%)

:BIOVATION SHAHAB MISSING

IF EXIST %binaryPath% (
	ECHO Installing %serviceName% service
	SC create "%serviceName%" binPath= "%binaryPath%" DisplayName= "%serviceDisplayName%" start=auto
	SC description "%serviceName%" "The Shahab plate detector manager service of Biovation system, handles the plate detector core. (C) Kasra Co."
	SC failure "%serviceName%" reset=10800 actions=restart/15/restart/45//
) ELSE (
	ECHO [Error]: The binary file of %serviceName% service is not found in path:
	ECHO 	%binaryPath%
)

IF "%selectedService%" NEQ "" GOTO EOF


:: Section 8: Biovation SUPREMA service
:SUPREMA
ECHO ====================
ECHO Installing Biovation Suprema Service
ECHO ====================

SET serviceFileName=Biovation.Brands.Suprema
IF "%installationMode%"=="Debug" (SET binaryPath=%basePath%%serviceFileName%\bin\Debug\netcoreapp3.1\%serviceFileName%.exe) ELSE (SET binaryPath=%basePath%%serviceFileName%\%serviceFileName%.exe)

IF "%instanceName%" NEQ "" (SET serviceName=Biovation.Brands.Suprema$%instanceName%)			ELSE (SET serviceName=Biovation.Brands.Suprema)		 
IF "%instanceName%" NEQ "" (SET serviceDisplayName=Biovation Suprema Manager (%instanceName%^)) ELSE (SET serviceDisplayName=Biovation Suprema Manager)

SC QUERY "%serviceName%" > NUL
IF ERRORLEVEL 1060 GOTO BIOVATION SUPREMA MISSING
ECHO BIOVATION SUPREMA EXISTS
GOTO BIOVATION SUPREMA EXISTS

:BIOVATION SUPREMA EXISTS

IF EXIST %binaryPath% (
	for /F "tokens=3 delims=: " %%H in ('sc query "%serviceName%" ^| findstr "        STATE"') do (
	  if /I "%%H" NEQ "STOPPED" (
	   ECHO Stopping 
	   SC stop "%serviceName%"
	  )
	)

	SC delete %serviceName%)

:BIOVATION SUPREMA MISSING

IF EXIST %binaryPath% (
	ECHO Installing %serviceName% service
	SC create "%serviceName%" binPath= "%binaryPath%" DisplayName= "%serviceDisplayName%" start=auto
	SC description "%serviceName%" "The Suprema V1 device manager service of Biovation system, handles the devices' requests. (C) Kasra Co."
	SC failure "%serviceName%" reset=10800 actions=restart/15/restart/45//
) ELSE (
	ECHO [Error]: The binary file of %serviceName% service is not found in path:
	ECHO 	%binaryPath%
)

IF "%selectedService%" NEQ "" GOTO EOF


:: Section 9: Biovation VIRDI service
:VIRDI
ECHO ====================
ECHO Installing Biovation Virdi Service
ECHO ====================

SET serviceFileName=Biovation.Brands.Virdi
IF "%installationMode%"=="Debug" (SET binaryPath=%basePath%%serviceFileName%\bin\Debug\netcoreapp3.1\%serviceFileName%.exe) ELSE SET binaryPath=%basePath%%serviceFileName%\%serviceFileName%.exe

IF "%instanceName%" NEQ "" (SET serviceName=Biovation.Brands.Virdi$%instanceName%)			  ELSE (SET serviceName=Biovation.Brands.Virdi)			
IF "%instanceName%" NEQ "" (SET serviceDisplayName=Biovation Virdi Manager (%instanceName%^)) ELSE (SET serviceDisplayName=Biovation Virdi Manager)

SC QUERY "%serviceName%" > NUL
IF ERRORLEVEL 1060 GOTO BIOVATION VIRDI MISSING
ECHO BIOVATION VIRDI EXISTS
GOTO BIOVATION VIRDI EXISTS

:BIOVATION VIRDI EXISTS

IF EXIST %binaryPath% (
	for /F "tokens=3 delims=: " %%H in ('sc query "%serviceName%" ^| findstr "        STATE"') do (
	  if /I "%%H" NEQ "STOPPED" (
	   ECHO Stopping 
	   SC stop "%serviceName%"
	  )
	)

	SC delete %serviceName%)

:BIOVATION VIRDI MISSING

IF EXIST %binaryPath% (
	ECHO Installing %serviceName% service
	SC create "%serviceName%" binPath= "%binaryPath%" DisplayName= "%serviceDisplayName%" start=auto
	SC description "%serviceName%" "The Virdi device manager service of Biovation system, handles the devices' requests. (C) Kasra Co."
	SC failure "%serviceName%" reset=10800 actions=restart/15/restart/45//
) ELSE (
	ECHO [Error]: The binary file of %serviceName% service is not found in path:
	ECHO 	%binaryPath%
)

IF "%selectedService%" NEQ "" GOTO EOF


:: Section 10: Biovation ZK service
:ZK
ECHO ====================
ECHO Installing Biovation ZK Service
ECHO ====================

SET serviceFileName=Biovation.Brands.ZK
IF "%installationMode%"=="Debug" (SET binaryPath=%basePath%%serviceFileName%\bin\Debug\netcoreapp3.1\%serviceFileName%.exe) ELSE SET binaryPath=%basePath%%serviceFileName%\%serviceFileName%.exe

IF "%instanceName%" NEQ "" (SET serviceName=Biovation.Brands.ZK$%instanceName%)			   ELSE (SET serviceName=Biovation.Brands.ZK)		
IF "%instanceName%" NEQ "" (SET serviceDisplayName=Biovation ZK Manager (%instanceName%^)) ELSE (SET serviceDisplayName=Biovation ZK Manager)

SC QUERY "%serviceName%" > NUL
IF ERRORLEVEL 1060 GOTO BIOVATION ZK MISSING
ECHO BIOVATION ZK EXISTS
GOTO BIOVATION ZK EXISTS

:BIOVATION ZK EXISTS

IF EXIST %binaryPath% (
	for /F "tokens=3 delims=: " %%H in ('sc query "%serviceName%" ^| findstr "        STATE"') do (
	  if /I "%%H" NEQ "STOPPED" (
	   ECHO Stopping 
	   SC stop "%serviceName%"
	  )
	)

	SC delete %serviceName%)

:BIOVATION ZK MISSING

IF EXIST %binaryPath% (
	ECHO Installing %serviceName% service
	SC create "%serviceName%" binPath= "%binaryPath%" DisplayName= "%serviceDisplayName%" start=auto
	SC description "%serviceName%" "The ZK device manager service of Biovation system, handles the devices' requests. (C) Kasra Co."
	SC failure "%serviceName%" reset=10800 actions=restart/15/restart/45//
) ELSE (
	ECHO [Error]: The binary file of %serviceName% service is not found in path:
	ECHO 	%binaryPath%
)

IF "%selectedService%" NEQ "" GOTO EOF


:: Section 11: Biovation ServiceManager service
:MANAGER
ECHO ====================
ECHO Installing Biovation ServiceManager Service
ECHO ====================

SET serviceFileName=Biovation.ServiceManager
IF "%installationMode%"=="Debug" (SET binaryPath=%basePath%%serviceFileName%\bin\Debug\netcoreapp3.1\%serviceFileName%.exe) ELSE SET binaryPath=%basePath%%serviceFileName%\%serviceFileName%.exe

IF "%instanceName%" NEQ "" (SET serviceName=Biovation.ServiceManager$%instanceName%)			ELSE (SET serviceName=Biovation.ServiceManager)		 
IF "%instanceName%" NEQ "" (SET serviceDisplayName=Biovation Service Manager (%instanceName%^)) ELSE (SET serviceDisplayName=Biovation Service Manager)

SC QUERY "%serviceName%" > NUL
IF ERRORLEVEL 1060 GOTO BIOVATION SERVICE MANAGER MISSING
ECHO BIOVATION SERVICE MANAGER EXISTS
GOTO BIOVATION SERVICE MANAGER EXISTS

:BIOVATION SERVICE MANAGER EXISTS

IF EXIST %binaryPath% (
	for /F "tokens=3 delims=: " %%H in ('sc query "%serviceName%" ^| findstr "        STATE"') do (
	  if /I "%%H" NEQ "STOPPED" (
	   ECHO Stopping 
	   SC stop "%serviceName%"
	  )
	)

	SC delete %serviceName%)

:BIOVATION SERVICE MANAGER MISSING

IF EXIST %binaryPath% (
	ECHO Installing %serviceName% service
	SC create "%serviceName%" binPath= "%binaryPath%" DisplayName= "%serviceDisplayName%" start=auto
	SC description "%serviceName%" "The services manager of Biovation system, controls the services availability. (C) Kasra Co."
	SC failure "%serviceName%" reset=10800 actions=restart/15/restart/45//
) ELSE (
	ECHO [Error]: The binary file of %serviceName% service is not found in path:
	ECHO 	%binaryPath%
)

:EOF
pause