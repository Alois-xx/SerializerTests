@echo off 
setlocal enabledelayedexpansion

REM By default use the latest one from the Windows SDK
set WPRLocation=C:\Program Files (x86)\Windows Kits\10\Windows Performance Toolkit\wpr.exe

if "%1" EQU "-profile" (
	set ProfilingEnabled=1
	SHIFT /1
)
echo current directory: %CD%
set Runs=%1
if "!Runs!" EQU "" set Runs=3

set PayloadDataSize=%2
if "!PayloadDataSize!" EQU "" set PayloadDataSize=0

SerializerTests.exe -test firstcall -bookdatasize !PayloadDataSize! -nongenwarn > Startup_NoNGen_Core.csv

echo Running Test !Runs! times with PayloadSize=!PayloadDataSize!

if "%ProfilingEnabled%" EQU "1" (
	"!WPRLocation!" -start MultiProfile.wprp^^!CSwitch -start MultiProfile.wprp^^!File
)

SerializerTests.exe -test combined -Runs !Runs! -bookdatasize !PayloadDataSize! -nongenwarn > SerializationPerf_Core.csv

if "%ProfilingEnabled%" EQU "1" (
	"!WPRLocation!" -stop c:\temp\SerializeTests.etl -skipPdbGen
)

