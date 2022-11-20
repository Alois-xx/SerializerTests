@echo off 
setlocal enabledelayedexpansion


if "%1" EQU "-profile" (
	set ProfilingEnabled=1
	SHIFT /1
)

REM uninstall any previous ngenned dlls
set PayloadDataSize=%2
if "!PayloadDataSize!" EQU "" set PayloadDataSize=0

cmd /C NGen.cmd 
SerializerTests.exe -test firstcall -nongenwarn -bookdatasize !PayloadDataSize! -scenario NoNgen > SerializationPerf.csv
cmd /C Ngen.cmd -install

call Profile.cmd -start

SerializerTests.exe -test firstcall -nongenwarn -bookdatasize !PayloadDataSize! -scenario Ngen >> SerializationPerf.csv

call Profile.cmd -stop c:\temp\SerializeTests_Startup.etl


set Runs=%1
if "!Runs!" EQU "" set Runs=3

echo Running Test !Runs! times with PayloadSize=!PayloadDataSize!, Additional Arguments: !AdditionalArgs!

call Profile.cmd -start

SerializerTests.exe -test combined -Runs !Runs! -bookdatasize !PayloadDataSize! -scenario Combined !AdditionalArgs! >> SerializationPerf.csv

call Profile -stop c:\temp\SerializeTests.etl

