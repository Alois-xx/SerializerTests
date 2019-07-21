@echo off 
setlocal enabledelayedexpansion

if "%1" EQU "-profile" (
	REM uninstall any previous ngenned dlls
	cmd /C NGen.cmd 
	"%perftools%\xxprofile" -on 
	SerializerTests.exe -test combined -Runs 1  -nongenwarn > C:\temp\NET_SerializationTests_Profiler.csv
	"%perftools%\xxprofile" -stop c:\temp\Net_SerializationTests.etl
) ELSE (
	REM uninstall any previous ngenned dlls
	cmd /C NGen.cmd 
	SerializerTests.exe -test firstcall -nongenwarn > Startup_NoNGen.csv
	cmd /C Ngen.cmd -install
	SerializerTests.exe -test firstcall -nongenwarn > Startup_NGen.csv
	set Runs=%1
	if "!Runs!" EQU "" set Runs=3
	echo Running Test !Runs! Times
	SerializerTests.exe -test combined -Runs !Runs! > SerializationPerf.csv
)
