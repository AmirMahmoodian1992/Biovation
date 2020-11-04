@ECHO OFF
SET basePath=%~1
SET installationMode=%~2
IF "%basePath%"=="" SET basePath=%~dp0
IF "%installationMode%"=="" SET installationMode="Release"
ECHO %basePath%

:: Section 1: Biovation gateway service
ECHO ====================
ECHO Installing Biovation Gateway Service
ECHO ====================

SET serviceName=Biovation.Gateway
SET serviceDisplayName=Biovation Gateway

SC QUERY "%serviceName%" > NUL
IF ERRORLEVEL 1060 GOTO BIOVATION GATEWAY MISSING
ECHO BIOVATION GATEWAY EXISTS
GOTO BIOVATION GATEWAY EXISTS

:BIOVATION GATEWAY EXISTS

for /F "tokens=3 delims=: " %%H in ('sc query "%serviceName%" ^| findstr "        STATE"') do (
  if /I "%%H" NEQ "STOPPED" (
   ECHO Stopping 
   SC stop "%serviceName%"
  )
)

SC delete %serviceName%

:BIOVATION GATEWAY MISSING

IF "%installationMode%"=="Debug" (SET binaryPath=%basePath%\%serviceName%\bin\Debug\netcoreapp3.1\%serviceName%.exe) ELSE SET binaryPath=%basePath%\%serviceName%\%serviceName%.exe

SC create "%serviceName%" binPath= "%basePath%\%serviceName%\bin\Debug\netcoreapp3.1\%serviceName%.exe" DisplayName= "%serviceDisplayName%" start=auto
SC description "%serviceName%" "The API gateway of Biovation system, handles the requests. (C) Kasra Co."
SC failure "%serviceName%" reset=10800 actions=restart/15/restart/45//



:: Section 2: Biovation queries service
ECHO ====================
ECHO Installing Biovation Queries Service
ECHO ====================

SET serviceName=Biovation.Data.Queries
SET serviceDisplayName=Biovation Queries

SC QUERY "%serviceName%" > NUL
IF ERRORLEVEL 1060 GOTO BIOVATION QUERIES MISSING
ECHO BIOVATION QUERIES EXISTS
GOTO BIOVATION QUERIES EXISTS

:BIOVATION QUERIES EXISTS

for /F "tokens=3 delims=: " %%H in ('sc query "%serviceName%" ^| findstr "        STATE"') do (
  if /I "%%H" NEQ "STOPPED" (
   ECHO Stopping 
   SC stop "%serviceName%"
  )
)

SC delete %serviceName%

:BIOVATION QUERIES MISSING

IF "%installationMode%"=="Debug" (SET binaryPath=%basePath%\%serviceName%\bin\Debug\netcoreapp3.1\%serviceName%.exe) ELSE SET binaryPath=%basePath%\%serviceName%\%serviceName%.exe

SC create "%serviceName%" binPath= "%basePath%\%serviceName%\bin\Debug\netcoreapp3.1\%serviceName%.exe" DisplayName= "%serviceDisplayName%" start=auto
SC description "%serviceName%" "The data manipulator controller of Biovation system, handles the data flow. (C) Kasra Co."
SC failure "%serviceName%" reset=10800 actions=restart/15/restart/45//



:: Section 3: Biovation commands service
ECHO ====================
ECHO Installing Biovation Commands Service
ECHO ====================

SET serviceName=Biovation.Data.Commands
SET serviceDisplayName=Biovation Commands

SC QUERY "%serviceName%" > NUL
IF ERRORLEVEL 1060 GOTO BIOVATION COMMANDS MISSING
ECHO BIOVATION COMMANDS EXISTS
GOTO BIOVATION COMMANDS EXISTS

:BIOVATION COMMANDS EXISTS

for /F "tokens=3 delims=: " %%H in ('sc query "%serviceName%" ^| findstr "        STATE"') do (
  if /I "%%H" NEQ "STOPPED" (
   ECHO Stopping 
   SC stop "%serviceName%"
  )
)

SC delete %serviceName%

:BIOVATION COMMANDS MISSING

IF "%installationMode%"=="Debug" (SET binaryPath=%basePath%\%serviceName%\bin\Debug\netcoreapp3.1\%serviceName%.exe) ELSE SET binaryPath=%basePath%\%serviceName%\%serviceName%.exe

SC create "%serviceName%" binPath= "%basePath%\%serviceName%\bin\Debug\netcoreapp3.1\%serviceName%.exe" DisplayName= "%serviceDisplayName%" start=auto
SC description "%serviceName%" "The fetch data controller of Biovation system, handles the data flow. (C) Kasra Co."
SC failure "%serviceName%" reset=10800 actions=restart/15/restart/45//



:: Section 4: Biovation server service
ECHO ====================
ECHO Installing Biovation Server Service
ECHO ====================

SET serviceName=Biovation.Server
SET serviceDisplayName=Biovation Server

SC QUERY "%serviceName%" > NUL
IF ERRORLEVEL 1060 GOTO BIOVATION SERVER MISSING
ECHO BIOVATION SERVER EXISTS
GOTO BIOVATION SERVER EXISTS

:BIOVATION SERVER EXISTS

for /F "tokens=3 delims=: " %%H in ('sc query "%serviceName%" ^| findstr "        STATE"') do (
  if /I "%%H" NEQ "STOPPED" (
   ECHO Stopping 
   SC stop "%serviceName%"
  )
)

SC delete %serviceName%

:BIOVATION SERVER MISSING

IF "%installationMode%"=="Debug" (SET binaryPath=%basePath%\%serviceName%\bin\Debug\netcoreapp3.1\%serviceName%.exe) ELSE SET binaryPath=%basePath%\%serviceName%\%serviceName%.exe

SC create "%serviceName%" binPath= "%basePath%\%serviceName%\bin\Debug\netcoreapp3.1\%serviceName%.exe" DisplayName= "%serviceDisplayName%" start=auto
SC description "%serviceName%" "The main and the manager service of Biovation system, handles the requests. (C) Kasra Co."
SC failure "%serviceName%" reset=10800 actions=restart/15/restart/45//



:: Section 5: Biovation EOS service
ECHO ====================
ECHO Installing Biovation EOS Service
ECHO ====================

SET serviceName=Biovation.Brands.EOS
SET serviceDisplayName=Biovation EOS Manager

SC QUERY "%serviceName%" > NUL
IF ERRORLEVEL 1060 GOTO BIOVATION EOS MISSING
ECHO BIOVATION EOS EXISTS
GOTO BIOVATION EOS EXISTS

:BIOVATION EOS EXISTS

for /F "tokens=3 delims=: " %%H in ('sc query "%serviceName%" ^| findstr "        STATE"') do (
  if /I "%%H" NEQ "STOPPED" (
   ECHO Stopping 
   SC stop "%serviceName%"
  )
)

SC delete %serviceName%

:BIOVATION EOS MISSING

IF "%installationMode%"=="Debug" (SET binaryPath=%basePath%\%serviceName%\bin\Debug\netcoreapp3.1\%serviceName%.exe) ELSE SET binaryPath=%basePath%\%serviceName%\%serviceName%.exe

SC create "%serviceName%" binPath= "%basePath%\%serviceName%\bin\Debug\netcoreapp3.1\%serviceName%.exe" DisplayName= "%serviceDisplayName%" start=auto
SC description "%serviceName%" "The Elm-o-Sanat device manager service of Biovation system, handles the devices' requests. (C) Kasra Co."
SC failure "%serviceName%" reset=10800 actions=restart/15/restart/45//



:: Section 6: Biovation PW service
ECHO ====================
ECHO Installing Biovation PW Service
ECHO ====================

SET serviceName=Biovation.Brands.PW
SET serviceDisplayName=Biovation PW Manager

SC QUERY "%serviceName%" > NUL
IF ERRORLEVEL 1060 GOTO BIOVATION PW MISSING
ECHO BIOVATION PW EXISTS
GOTO BIOVATION PW EXISTS

:BIOVATION PW EXISTS

for /F "tokens=3 delims=: " %%H in ('sc query "%serviceName%" ^| findstr "        STATE"') do (
  if /I "%%H" NEQ "STOPPED" (
   ECHO Stopping 
   SC stop "%serviceName%"
  )
)

SC delete %serviceName%

:BIOVATION PW MISSING

IF "%installationMode%"=="Debug" (SET binaryPath=%basePath%\%serviceName%\bin\Debug\netcoreapp3.1\%serviceName%.exe) ELSE SET binaryPath=%basePath%\%serviceName%\%serviceName%.exe

SC create "%serviceName%" binPath= "%basePath%\%serviceName%\bin\Debug\netcoreapp3.1\%serviceName%.exe" DisplayName= "%serviceDisplayName%" start=auto
SC description "%serviceName%" "The Processing World device manager service of Biovation system, handles the devices' requests. (C) Kasra Co."
SC failure "%serviceName%" reset=10800 actions=restart/15/restart/45//



:: Section 7: Biovation SHAHAB service
ECHO ====================
ECHO Installing Biovation Shahab Service
ECHO ====================

SET serviceName=Biovation.Brands.Shahab
SET serviceDisplayName=Biovation Shahab Manager

SC QUERY "%serviceName%" > NUL
IF ERRORLEVEL 1060 GOTO BIOVATION SHAHAB MISSING
ECHO BIOVATION SHAHAB EXISTS
GOTO BIOVATION SHAHAB EXISTS

:BIOVATION SHAHAB EXISTS

for /F "tokens=3 delims=: " %%H in ('sc query "%serviceName%" ^| findstr "        STATE"') do (
  if /I "%%H" NEQ "STOPPED" (
   ECHO Stopping 
   SC stop "%serviceName%"
  )
)

SC delete %serviceName%

:BIOVATION SHAHAB MISSING

IF "%installationMode%"=="Debug" (SET binaryPath=%basePath%\%serviceName%\bin\Debug\netcoreapp3.1\%serviceName%.exe) ELSE SET binaryPath=%basePath%\%serviceName%\%serviceName%.exe

SC create "%serviceName%" binPath= "%basePath%\%serviceName%\bin\Debug\netcoreapp3.1\%serviceName%.exe" DisplayName= "%serviceDisplayName%" start=auto
SC description "%serviceName%" "The Shahab plate detector manager service of Biovation system, handles the plate detector core. (C) Kasra Co."
SC failure "%serviceName%" reset=10800 actions=restart/15/restart/45//



:: Section 8: Biovation SUPREMA service
ECHO ====================
ECHO Installing Biovation Suprema Service
ECHO ====================

SET serviceName=Biovation.Brands.Suprema
SET serviceDisplayName=Biovation Suprema Manager

SC QUERY "%serviceName%" > NUL
IF ERRORLEVEL 1060 GOTO BIOVATION SUPREMA MISSING
ECHO BIOVATION SUPREMA EXISTS
GOTO BIOVATION SUPREMA EXISTS

:BIOVATION SUPREMA EXISTS

for /F "tokens=3 delims=: " %%H in ('sc query "%serviceName%" ^| findstr "        STATE"') do (
  if /I "%%H" NEQ "STOPPED" (
   ECHO Stopping 
   SC stop "%serviceName%"
  )
)

SC delete %serviceName%

:BIOVATION SUPREMA MISSING

IF "%installationMode%"=="Debug" (SET binaryPath=%basePath%\%serviceName%\bin\Debug\netcoreapp3.1\%serviceName%.exe) ELSE SET binaryPath=%basePath%\%serviceName%\%serviceName%.exe

SC create "%serviceName%" binPath= "%basePath%\%serviceName%\bin\Debug\netcoreapp3.1\%serviceName%.exe" DisplayName= "%serviceDisplayName%" start=auto
SC description "%serviceName%" "The Suprema V1 device manager service of Biovation system, handles the devices' requests. (C) Kasra Co."
SC failure "%serviceName%" reset=10800 actions=restart/15/restart/45//



:: Section 9: Biovation VIRDI service
ECHO ====================
ECHO Installing Biovation Virdi Service
ECHO ====================

SET serviceName=Biovation.Brands.Virdi
SET serviceDisplayName=Biovation Virdi Manager

SC QUERY "%serviceName%" > NUL
IF ERRORLEVEL 1060 GOTO BIOVATION VIRDI MISSING
ECHO BIOVATION VIRDI EXISTS
GOTO BIOVATION VIRDI EXISTS

:BIOVATION VIRDI EXISTS

for /F "tokens=3 delims=: " %%H in ('sc query "%serviceName%" ^| findstr "        STATE"') do (
  if /I "%%H" NEQ "STOPPED" (
   ECHO Stopping 
   SC stop "%serviceName%"
  )
)

SC delete %serviceName%

:BIOVATION VIRDI MISSING

IF "%installationMode%"=="Debug" (SET binaryPath=%basePath%\%serviceName%\bin\Debug\netcoreapp3.1\%serviceName%.exe) ELSE SET binaryPath=%basePath%\%serviceName%\%serviceName%.exe

SC create "%serviceName%" binPath= "%basePath%\%serviceName%\bin\Debug\netcoreapp3.1\%serviceName%.exe" DisplayName= "%serviceDisplayName%" start=auto
SC description "%serviceName%" "The Virdi device manager service of Biovation system, handles the devices' requests. (C) Kasra Co."
SC failure "%serviceName%" reset=10800 actions=restart/15/restart/45//



:: Section 10: Biovation ZK service
ECHO ====================
ECHO Installing Biovation ZK Service
ECHO ====================

SET serviceName=Biovation.Brands.ZK
SET serviceDisplayName=Biovation ZK Manager

SC QUERY "%serviceName%" > NUL
IF ERRORLEVEL 1060 GOTO BIOVATION ZK MISSING
ECHO BIOVATION ZK EXISTS
GOTO BIOVATION ZK EXISTS

:BIOVATION ZK EXISTS

for /F "tokens=3 delims=: " %%H in ('sc query "%serviceName%" ^| findstr "        STATE"') do (
  if /I "%%H" NEQ "STOPPED" (
   ECHO Stopping 
   SC stop "%serviceName%"
  )
)

SC delete %serviceName%

:BIOVATION ZK MISSING

IF "%installationMode%"=="Debug" (SET binaryPath=%basePath%\%serviceName%\bin\Debug\netcoreapp3.1\%serviceName%.exe) ELSE SET binaryPath=%basePath%\%serviceName%\%serviceName%.exe

SC create "%serviceName%" binPath= "%basePath%\%serviceName%\bin\Debug\netcoreapp3.1\%serviceName%.exe" DisplayName= "%serviceDisplayName%" start=auto
SC description "%serviceName%" "The ZK device manager service of Biovation system, handles the devices' requests. (C) Kasra Co."
SC failure "%serviceName%" reset=10800 actions=restart/15/restart/45//


:: Section 10: Biovation ServiceManager service
ECHO ====================
ECHO Installing Biovation ServiceManager Service
ECHO ====================

SET serviceName=Biovation.ServiceManager
SET serviceDisplayName=Biovation Service Manager

SC QUERY "%serviceName%" > NUL
IF ERRORLEVEL 1060 GOTO BIOVATION SERVICE MANAGER MISSING
ECHO BIOVATION SERVICE MANAGER EXISTS
GOTO BIOVATION SERVICE MANAGER EXISTS

:BIOVATION SERVICE MANAGER EXISTS

for /F "tokens=3 delims=: " %%H in ('sc query "%serviceName%" ^| findstr "        STATE"') do (
  if /I "%%H" NEQ "STOPPED" (
   ECHO Stopping 
   SC stop "%serviceName%"
  )
)

SC delete %serviceName%

:BIOVATION SERVICE MANAGER MISSING

IF "%installationMode%"=="Debug" (SET binaryPath=%basePath%\%serviceName%\bin\Debug\netcoreapp3.1\%serviceName%.exe) ELSE SET binaryPath=%basePath%\%serviceName%\%serviceName%.exe

SC create "%serviceName%" binPath= "%basePath%\%serviceName%\bin\Debug\netcoreapp3.1\%serviceName%.exe" DisplayName= "%serviceDisplayName%" start=auto
SC description "%serviceName%" "The services manager of Biovation system, controls the services availability. (C) Kasra Co."
SC failure "%serviceName%" reset=10800 actions=restart/15/restart/45//


pause