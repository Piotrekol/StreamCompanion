@echo off
REM initial cleanup
rd .\build\Release /s /q
rd .\build\Output /s /q
mkdir .\build\Release
mkdir .\build\Output

build\nuget.exe restore
REM build solution
build\msbuild.bat . /p:Configuration=Release /p:Platform=x86

pause