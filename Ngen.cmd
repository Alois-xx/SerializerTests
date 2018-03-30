@echo off 
setlocal enabledelayedexpansion

set Action=uninstall
if "%1" EQU "-install" (
set Action=install
)

%windir%\Microsoft.NET\Framework64\v4.0.30319\ngen.exe %Action% SerializerTests.exe
for %%i in (*.dll) do  %windir%\Microsoft.NET\Framework64\v4.0.30319\ngen.exe %Action% %%i
