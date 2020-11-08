@pushd .
setlocal enabledelayedexpansion
set CurDir=%CD%
set SimpleTime=%Time::=_%
set SimpleTime=%SimpleTime: =_%
set CurDir=%CD%_%SimpleTime%
set PayloadByteSize=0
set Runs=1
mkdir "%CurDir%"
echo Profiling results will be copied to directory %CurDir%

if "%1" NEQ "" set PayloadByteSize=%1

cd bin\Release\net48
cmd /C RunTests.cmd !Runs! !PayloadByteSize!
move Startup_NGen.csv "%CurDir%"
move Startup_NoNGen.csv "%CurDir%"
move SerializationPerf.csv "%CurDir%"

if "%1" EQU "-profile" (
	cmd /C RunTests.cmd -profile
	move c:\temp\Net_SerializationTests.etl "%CurDir%"
	move c:\temp\Net_SerializationTests.etl.NGENPDB "%CurDir%"
	robocopy /S c:\temp\Net_SerializationTests.etl.NGENPDB  "%CurDir%\Net_SerializationTests.etl.NGENPDB"
	move C:\temp\NET_SerializationTests_Profiler.csv "%CurDir%"
)

cd ..\net50
cmd /C RunTests_Core.cmd !Runs! !PayloadByteSize!
move Startup_NoNGen_Core.csv "%CurDir%"
move SerializationPerf_Core.csv "%CurDir%"

cd ..\netcoreapp3.1
cmd /C RunTests_Core.cmd !Runs! !PayloadByteSize!
move Startup_NoNGen_Core.csv  Startup_NoNGen_Core3.1.csv
move SerializationPerf_Core.csv SerializationPerf_Core3.1.csv
move Startup_NoNGen_Core3.1.csv "%CurDir%"
move SerializationPerf_Core3.1.csv "%CurDir%"

if "%1" EQU "-profile" (
	cmd /C RunTests_Core.cmd -profile
	move c:\temp\NETCore_SerializationTests_Profiler.csv "%CurDir%"
	move c:\temp\NETCore_SerializeTests.etl "%CurDir%"
	robocopy /S C:\temp\NETCore_SerializeTests.etl.NGENPDB  "%CurDir%\NETCore_SerializeTests.etl.NGENPDB"
)

cd "%CurDir%"
set OutputFileName=SerializationPerf_Combined_!PayloadByteSize!.csv
copy SerializationPerf.csv !OutputFileName!
type SerializationPerf_Core.csv  | findstr /v Objects >> !OutputFileName!
type SerializationPerf_Core3.1.csv  | findstr /v Objects >> !OutputFileName!
echo Test Results are located at %CurDir%\!OutputFileName!
@popd
