@echo off 
setlocal enabledelayedexpansion

REM uninstall any previous ngenned dlls
rem cmd /C NGen.cmd 
dotnet SerializerTests.dll -test firstcall -nongenwarn > Startup_NoNGen_Core.csv
rem cmd /C Ngen.cmd -install
rem SerializerTests.exe -test firstcall -nongenwarn > Startup_NGen.csv
dotnet SerializerTests.dll -test combined -Runs 5 -nongenwarn > SerializationPerf_Core.csv