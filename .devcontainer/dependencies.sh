#!/bin/bash
#
# Setup Valheim dependencies in devcontainer
sudo dpkg --add-architecture i386
sudo sed -i 's/^Components: main$/& contrib non-free non-free-firmware/' /etc/apt/sources.list.d/debian.sources
echo steam steam/license note '' | sudo debconf-set-selections
echo steam steam/question select "I AGREE" | sudo debconf-set-selections
sudo apt update
DEBIAN_FRONTEND=noninteractive sudo apt -y install steamcmd

wget -O bepinex.zip "https://thunderstore.io/package/download/denikson/BepInExPack_Valheim/5.4.2202/"
unzip bepinex.zip -d ~/BepInExRaw

/usr/games/steamcmd +force_install_dir ~/VHINSTALL +login anonymous +app_update 896660 validate +exit

mv ~/VHINSTALL/valheim_server_Data/ ~/VHINSTALL/valheim_Data/
mv ~/BepInExRaw/BepInExPack_Valheim/* ~/VHINSTALL/

# PreBuild
dotnet restore
pnpm install

# Build
dotnet build DiscordConnector.sln /p:VALHEIM_INSTALL=/home/vscode/VHINSTALL
pnpm docs:build