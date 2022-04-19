@ECHO OFF
SETLOCAL ENABLEEXTENSIONS ENABLEDELAYEDEXPANSION

SET SOLUTION_DIR=%~dp0
REM Find MSBuild on PATH
FOR /F %%a IN ("msbuild.exe") DO SET MSBUILD=%%~$PATH:a
IF NOT DEFINED MSBUILD CALL :FIND_MSBUILD

IF NOT DEFINED MSBUILD (
	ECHO "Cannot find MSBuild.exe"
	GOTO :EOF
)

ECHO Building solution...
"%MSBUILD%" "%SOLUTION_DIR%\ChannelsProto.sln" 1>NUL
IF ERRORLEVEL 1 (
	ECHO Build failed
	GOTO :EOF
)

ECHO Done building, running demo
ECHO ChannelsCrawler.exe %*

"%SOLUTION_DIR%\ChannelsCrawler\bin\Debug\net5.0\ChannelsCrawler.exe" %*

GOTO :EOF

:FIND_MSBUILD
SET VS_VERSIONS=2019 2017 2022

:FIND_MSBUILD_NEXT_VS_VERSION
FOR /F "tokens=1,*" %%a IN ("%VS_VERSIONS%") DO (
	SET VS_VERSION_CURRENT=%%a
	SET VS_VERSIONS=%%b
)

SET VS_EDITIONS=Enterprise Professional Community BuildTools

:FIND_MSBUILD_NEXT_VS_EDITION
FOR /F "tokens=1,*" %%a IN ("%VS_EDITIONS%") DO (
	SET VS_EDITION_CURRENT=%%a
	SET VS_EDITIONS=%%b
)

IF NOT EXIST "C:\Program Files (x86)\Microsoft Visual Studio\%VS_VERSION_CURRENT%\%VS_EDITION_CURRENT%\MSBuild\Current\Bin\MSBuild.exe" (
	GOTO FIND_MSBUILD_CONTINUE
)

SET PATH=C:\Program Files (x86)\Microsoft Visual Studio\%VS_VERSION_CURRENT%\%VS_EDITION_CURRENT%\MSBuild\Current\Bin;%PATH%
FOR /F %%a IN ("msbuild.exe") DO SET MSBUILD=%%~$PATH:a
GOTO FIND_MSBUILD_END

:FIND_MSBUILD_CONTINUE
IF NOT "%VS_EDITIONS%" == "" GOTO FIND_MSBUILD_NEXT_VS_EDITION
IF NOT "%VS_VERSIONS%" == "" GOTO FIND_MSBUILD_NEXT_VS_VERSION

:FIND_MSBUILD_END
SET VS_EDITIONS=
SET VS_EDITION_CURRENT=
SET VS_VERSIONS=
SET VS_VERSION_CURRENT=
GOTO :EOF