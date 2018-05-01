@echo off 
setlocal enabledelayedexpansion

if "%1" EQU "-profile" (
	"%perftools%\xxprofile" -on 
	dotnet SerializerTests.dll -test combined -Runs 1 -nongenwarn > c:\temp\NETCore_SerializationTests_Profiler.csv
	"%perftools%\xxprofile" -stop c:\temp\NETCore_SerializeTests.etl
) ELSE (
	dotnet SerializerTests.dll -test firstcall -nongenwarn > Startup_NoNGen_Core.csv
	dotnet SerializerTests.dll -test combined -Runs 5 -nongenwarn > SerializationPerf_Core.csv
)

