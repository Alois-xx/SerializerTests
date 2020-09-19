@echo off 
setlocal enabledelayedexpansion

if "%1" EQU "-profile" (
	"%perftools%\xxprofile" -on 
	SerializerTests.exe -test combined -Runs 1 -nongenwarn > c:\temp\NETCore_SerializationTests_Profiler.csv
	"%perftools%\xxprofile" -stop c:\temp\NETCore_SerializeTests.etl
) ELSE (
	set Runs=%1
	if "!Runs!" EQU "" set Runs=3
	SerializerTests.exe -test firstcall -nongenwarn > Startup_NoNGen_Core.csv
	echo Running Test !Runs! Times
	SerializerTests.exe -test combined -Runs !Runs! -nongenwarn > SerializationPerf_Core.csv
)

