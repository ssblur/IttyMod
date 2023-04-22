#!/bin/bash
# move to the script directory
cd "$(dirname "$0")"
# build the mod
dotnet build
# copy the mod to the mods folder
cp ./bin/Debug/net7.0/* "$HOME/.local/share/Tiny Life/Mods" -r
# run the game
dir=$(<"$HOME/.local/share/Tiny Life/GameDir")
cd $dir
"./Tiny Life" -v

# TODO: Replace this system with the cake build system. Shouldn't be difficult, but prioritizing working.