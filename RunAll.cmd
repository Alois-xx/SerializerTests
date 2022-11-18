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
mkdir "%CurDir%"
echo Profiling results will be copied to directory %CurDir%

if "%1" EQU "-profile" (
	set ProfilingEnabled=-profile
	SHIFT /1
)

if "%1" NEQ "" set PayloadByteSize=%1

cd bin\Release\net60
cmd /C RunTests_Core.cmd !ProfilingEnabled! !Runs! !PayloadByteSize!
move Startup_NoNGen_Core.csv  Startup_NoNGen_6.0.csv
move SerializationPerf_Core.csv SerializationPerf_6.0.csv
move SerializationPerf_6.0.csv "%CurDir%"
move Startup_NoNGen_6.0.csv "%CurDir%"

if "!ProfilingEnabled!" EQU "-profile" (
	move c:\temp\SerializeTests.etl  "%CurDir%\SerializeTestsNet60.etl"
)


cd ..\net70
cmd /C RunTests_Core.cmd !ProfilingEnabled! !Runs! !PayloadByteSize!
move Startup_NoNGen_Core.csv  Startup_NoNGen_7.0.csv
move SerializationPerf_Core.csv SerializationPerf_7.0.csv
move SerializationPerf_7.0.csv "%CurDir%"
move Startup_NoNGen_7.0.csv "%CurDir%"

if "!ProfilingEnabled!" EQU "-profile" (
	move c:\temp\SerializeTests.etl  "%CurDir%\SerializeTestsNet70.etl"
)

cd ..\net48
cmd /C RunTests.cmd !ProfilingEnabled! !Runs! !PayloadByteSize!
move Startup_NGen.csv "%CurDir%"
move Startup_NoNGen.csv "%CurDir%"
move SerializationPerf.csv "%CurDir%"

if "!ProfilingEnabled!" EQU "-profile" (
	move C:\temp\SerializeTests.etl.NGENPDB  "%CurDir%\NETCore_SerializeTestsNet48.etl.NGENPDB"
	move c:\temp\SerializeTests.etl  "%CurDir%\SerializeTestsNet48.etl"
)

cd ..\net50
cmd /C RunTests_Core.cmd !ProfilingEnabled! !Runs! !PayloadByteSize!
move Startup_NoNGen_Core.csv  Startup_NoNGen_5.0.csv
move SerializationPerf_Core.csv SerializationPerf_5.0.csv
move Startup_NoNGen_5.0.csv "%CurDir%"
move SerializationPerf_5.0.csv "%CurDir%"

if "!ProfilingEnabled!" EQU "-profile" (
	move c:\temp\SerializeTests.etl  "%CurDir%\SerializeTestsNet50.etl"
)

cd ..\netcoreapp3.1
cmd /C RunTests_Core.cmd !ProfilingEnabled! !Runs! !PayloadByteSize!
move Startup_NoNGen_Core.csv  Startup_NoNGen_3.1.csv
move SerializationPerf_Core.csv SerializationPerf_3.1.csv
move Startup_NoNGen_3.1.csv "%CurDir%"
move SerializationPerf_3.1.csv "%CurDir%"

if "!ProfilingEnabled!" EQU "-profile" (
	move c:\temp\SerializeTests.etl  "%CurDir%\SerializeTestsNet31.etl"
)

cd "%CurDir%"
set OutputFileName=SerializationPerf_Combined_!PayloadByteSize!.csv
copy SerializationPerf.csv !OutputFileName!
type SerializationPerf_7.0.csv  | findstr /v Objects >> !OutputFileName!
type SerializationPerf_6.0.csv  | findstr /v Objects >> !OutputFileName!
type SerializationPerf_5.0.csv  | findstr /v Objects >> !OutputFileName!
type SerializationPerf_3.1.csv  | findstr /v Objects >> !OutputFileName!
echo Test Results are located at %CurDir%\!OutputFileName!
@popd
