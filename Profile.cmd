@echo off

REM By default use the latest one from the Windows SDK
set WPRLocation=C:\Program Files (x86)\Windows Kits\10\Windows Performance Toolkit\wpr.exe

if "%1" EQU "-start" goto :StartProfiling
if "%1" EQU "-stop"  goto StopProfiling

echo Profile.cmd [-start] [-stop xxx.etl]
echo It will only profile when the environment variable ProfilingEnabled=1 is set!

goto :EOF

:StartProfiling
if "%ProfilingEnabled%" EQU "1" (
	"!WPRLocation!" -cancel 2> NUL
	"!WPRLocation!" -start MultiProfile.wprp^^!CSwitch -start MultiProfile.wprp^^!File
)
goto :EOF

:StopProfiling
if "%ProfilingEnabled%" EQU "1" (
	"!WPRLocation!" -stop %2 -skipPdbGen %3 %4 %5 %6 %7
)