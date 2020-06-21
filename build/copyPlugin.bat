REM @ECHO OFF
setlocal
SET pluginSource=%1
SET pluginSource=%pluginSource:"=%
SET ConfigurationName=%2
SET ConfigurationName=%ConfigurationName:"=%
SET scriptDir=%~dp0
IF [%pluginSource%] == [] exit 1234
IF [%ConfigurationName%] == [] exit 1235

SET pluginsRoot=%scriptDir%%ConfigurationName%\Plugins\
xcopy /s /y /q /k "%pluginSource%*.*" "%pluginsRoot%"
endlocal