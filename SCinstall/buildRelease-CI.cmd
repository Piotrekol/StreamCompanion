@echo off
REM initial cleanup
REM rm ./build/Release/* -rf
rm ./build/Output/* -rf

REM build solution
REM MSBuild osu!StreamCompanion.sln /p:Configuration=Release /p:Platform=x86

cd ./build

mkdir Output

REM copy files to new folder
cp -r ./Release/* ./Output/
REM copy web overlay
robocopy ../webOverlay ./Output/Files/Web /E

REM remove debug symbols
rm ./Output/*.pdb
rm ./Output/Plugins/*.pdb

REM remove misc files
rm ./Output/*.xml
rm ./Output/StreamCompanion Updater.exe.config
rm ./Output/Plugins/StreamCompanionTypes.dll
rm ./Output/Plugins/CollectionManager.dll
rm ./Output/Plugins/System.*

cd ..
REM clean installer folder
rm ./innoSetup/Output/*
REM create installer (Inno Setup 6)
"C:\Program Files (x86)\Inno Setup 6\ISCC.exe" ".\innoSetup\setupScript.iss"
7z a .\build\ingameOverlay.zip .\build\Release_unsafe\*
"C:\Program Files (x86)\Inno Setup 6\ISCC.exe" ".\innoSetup\osuOverlayScript.iss"
"C:\Program Files (x86)\Inno Setup 6\ISCC.exe" ".\innoSetup\browserOverlayScript.iss"

type nul > .\build\Output\.portableMode
7z a .\build\StreamCompanion-portable.zip .\build\Output\*
7z a .\build\StreamCompanion-portable-browserOverlay.zip .\build\Release_browserOverlay\*
7z a .\build\StreamCompanion-portable-textOverlay.zip .\build\Release_unsafe\*
