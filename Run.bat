@echo off
rem move to the script directory
cd /d "%~dp0"
rem build the mod
dotnet build
rem copy the mod to the mods folder
robocopy ./bin/Debug/net7.0/ "%LOCALAPPDATA%/Tiny Life/Mods" /e /is
rem run the game
set /p dir=<"%LOCALAPPDATA%/Tiny Life/GameDir"
cd /d %dir%
set HARMONY_DEBUG=1 && "Tiny Life.exe" -v
