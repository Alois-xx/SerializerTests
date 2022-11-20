@pushd .
setlocal enabledelayedexpansion
set CurDir=%CD%\SerializerTestResults\
mkdir %CurDir% 2> NUL
set SimpleTime=%Time::=_%
set SimpleTime=%SimpleTime: =_%
set SimpleTime=%SimpleTime:~0,-3%
set CurDir=%CurDir%%SimpleTime%

set PayloadByteSize=0
set Runs=5
REM set AdditionalArgs=-N 1
mkdir "%CurDir%"
echo Profiling results will be copied to directory %CurDir%

if "%1" EQU "-profile" (
	set ProfilingEnabled=-profile
	set Runs=1
	SHIFT /1
)

if "%1" NEQ "" set PayloadByteSize=%1

echo Publish ReadyToRun binaries for .NET Core 3.1
dotnet publish -c Release -p:PublishProfile=NetCore31ReadyToRun -f netcoreapp3.1

echo Publish ReadyToRun binaries for .NET 5
dotnet publish -c Release -p:PublishProfile=Net50ReadyToRun -f net50 

echo Publish ReadyToRun binaries for .NET 6
dotnet publish -c Release -p:PublishProfile=Net60ReadyToRun -f net60 

echo Publish ReadyToRun binaries for .NET 7
dotnet publish -c Release -p:PublishProfile=Net70ReadyToRun -f net70 


cd bin\Release\net60
cmd /C RunTests_Core.cmd !ProfilingEnabled! !Runs! !PayloadByteSize! 
move SerializationPerf_Core.csv SerializationPerf_6.0.csv
move SerializationPerf_6.0.csv "%CurDir%"


if "!ProfilingEnabled!" EQU "-profile" (
	move c:\temp\SerializeTests.etl  "%CurDir%\SerializeTestsNet60.etl"
	move c:\temp\SerializeTests_Startup.etl  "%CurDir%\StartupNet60.etl"
)


cd ..\net70
cmd /C RunTests_Core.cmd !ProfilingEnabled! !Runs! !PayloadByteSize! 
move SerializationPerf_Core.csv SerializationPerf_7.0.csv
move SerializationPerf_7.0.csv "%CurDir%"


if "!ProfilingEnabled!" EQU "-profile" (
	move c:\temp\SerializeTests.etl  "%CurDir%\SerializeTestsNet70.etl"
	move c:\temp\SerializeTests_Startup.etl  "%CurDir%\StartupNet70.etl"
)

cd ..\net48
cmd /C RunTests.cmd !ProfilingEnabled! !Runs! !PayloadByteSize! 
move SerializationPerf.csv "%CurDir%"

if "!ProfilingEnabled!" EQU "-profile" (
	move c:\temp\SerializeTests.etl  "%CurDir%\SerializeTestsNet48.etl"
	move c:\temp\SerializeTests_Startup.etl  "%CurDir%\StartupNet48.etl"
)

cd ..\net50
cmd /C RunTests_Core.cmd !ProfilingEnabled! !Runs! !PayloadByteSize! 
move SerializationPerf_Core.csv SerializationPerf_5.0.csv
move SerializationPerf_5.0.csv "%CurDir%"

if "!ProfilingEnabled!" EQU "-profile" (
	move c:\temp\SerializeTests.etl  "%CurDir%\SerializeTestsNet50.etl"
	move c:\temp\SerializeTests_Startup.etl  "%CurDir%\StartupNet50.etl"
)

cd ..\netcoreapp3.1
cmd /C RunTests_Core.cmd !ProfilingEnabled! !Runs! !PayloadByteSize! 
move SerializationPerf_Core.csv SerializationPerf_3.1.csv
move SerializationPerf_3.1.csv "%CurDir%"

if "!ProfilingEnabled!" EQU "-profile" (
	move c:\temp\SerializeTests.etl  "%CurDir%\SerializeTestsNet31.etl"
	move c:\temp\SerializeTests_Startup.etl  "%CurDir%\StartupNet31.etl"
)

cd "%CurDir%"
set OutputFileName=SerializationPerf_Combined_!PayloadByteSize!.csv
echo Serializer	Objects	"Time to serialize in s"	"Time to deserialize in s"	"Size in bytes"	FileVersion	Framework	ProjectHome	DataFormat	FormatDetails	Supports Versioning	Scenario > !OutputFileName!
type SerializationPerf.csv      | findstr /v Objects >> !OutputFileName!
type SerializationPerf_7.0.csv  | findstr /v Objects >> !OutputFileName!
type SerializationPerf_6.0.csv  | findstr /v Objects >> !OutputFileName!
type SerializationPerf_5.0.csv  | findstr /v Objects >> !OutputFileName!
type SerializationPerf_3.1.csv  | findstr /v Objects >> !OutputFileName!
echo Test Results are located at %CurDir%\!OutputFileName!
@popd
