echo off
echo off
set baseDir=%1

REM set operating folder
pushd %baseDir%

REM remove and make deploy directory
IF EXIST deploy (
    rd deploy /s /q
)
mkdir deploy

REM copy files
copy "osu!StreamCompanion.exe" .\deploy\
copy "System.Data.SQLite.dll" .\deploy\
copy ".\x86\SQLite.Interop.dll" .\deploy\
pushd deploy
REM this assumes that 7z is avaliable in enviroment path
7z a -tzip SC.zip *
popd
popd