@echo off 
setlocal enabledelayedexpansion

REM By default use the latest one from the Windows SDK
set WPRLocation=C:\Program Files (x86)\Windows Kits\10\Windows Performance Toolkit\wpr.exe

if "%1" EQU "-profile" (
	set ProfilingEnabled=1
	SHIFT /1
)
set Runs=%1
if "!Runs!" EQU "" set Runs=3

set PayloadDataSize=%2
if "!PayloadDataSize!" EQU "" set PayloadDataSize=0

call Profile.cmd -start

SerializerTests.exe -test firstcall -bookdatasize !PayloadDataSize! -scenario NoCrossGen > SerializationPerf_Core.csv

.\publish\SerializerTests.exe -test firstcall -bookdatasize !PayloadDataSize! -scenario CrossGen -NoHeader >> SerializationPerf_Core.csv

call Profile.cmd -stop c:\temp\SerializeTests_Startup.etl -skipPdbGen

echo Running Test !Runs! times with PayloadSize=!PayloadDataSize! Additional Args: !AdditionalArgs!

call Profile.cmd -start

SerializerTests.exe -test combined -Runs !Runs! -bookdatasize !PayloadDataSize! -scenario Combined -NoHeader !AdditionalArgs! >> SerializationPerf_Core.csv

call Profile.cmd -stop c:\temp\SerializeTests.etl -skipPdbGen
