@pushd .
set CurDir=%CD%
set SimpleTime=%Time::=_%
set SimpleTime=%SimpleTime: =_%
set CurDir=%CD%_%SimpleTime%
mkdir "%CurDir%"
echo Profiling results will be copied to directory %CurDir%

cd bin\Release\net471
cmd /C RunTests.cmd
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

cd ..\netcoreapp2
cmd /C RunTests_Core.cmd
move Startup_NoNGen_Core.csv "%CurDir%"
move SerializationPerf_Core.csv "%CurDir%"

if "%1" EQU "-profile" (
	cmd /C RunTests_Core.cmd -profile
	move c:\temp\NETCore_SerializationTests_Profiler.csv "%CurDir%"
	move c:\temp\NETCore_SerializeTests.etl "%CurDir%"
	robocopy /S C:\temp\NETCore_SerializeTests.etl.NGENPDB  "%CurDir%\NETCore_SerializeTests.etl.NGENPDB"
)

cd "%CurDir%"
copy SerializationPerf.csv SerializationPerf_Combined.csv
type SerializationPerf_Core.csv >> SerializationPerf_Combined.csv
@popd


