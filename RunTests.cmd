@echo off 
setlocal enabledelayedexpansion

REM uninstall any previous ngenned dlls
cmd /C NGen.cmd 
SerializerTests.exe -test firstcall -nongenwarn > Startup_NoNGen.csv
cmd /C Ngen.cmd -install
SerializerTests.exe -test firstcall -nongenwarn > Startup_NGen.csv
SerializerTests.exe -test combined -Runs 5 > SerializationPerf.csv