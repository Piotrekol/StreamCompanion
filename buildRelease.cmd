@echo off
REM initial cleanup
rm ./build/Release/* -rf
rm ./build/Output/* -rf

REM build solution
MSBuild osu!StreamCompanion.sln /p:Configuration=Release /p:Platform=x86

cd ./build

mkdir Output

REM copy files to new folder
cp -r ./Release/* ./Output/

REM move sqlite dll
cp ./Output/x86/* ./Output/
REM remove sqlite folders
rm -rf ./Output/x86
rm -rf ./Output/x64

REM remove debug symbols
rm ./Output/*.pdb
rm ./Output/Plugins/*.pdb

REM remove misc files
rm ./Output/*.xml
rm ./Output/*.exe.config

REM plugin specific action..
mv ./Output/Plugins/OsuMemoryDataProvider.dll ./Output/

cd ..
REM clean installer folder
rm ./innoSetup/Output/*
REM create installer (Inno Setup 5)
ISCC ".\innoSetup\setupScript.iss"