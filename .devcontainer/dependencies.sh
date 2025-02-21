#!/bin/bash
#
# Setup Valheim dependencies in devcontainer
sudo apt -y install software-properties-common
sudo dpkg --add-architecture i386
sudo add-apt-repository -y -n -U http://deb.debian.org/debian -c non-free -c non-free-firmware
sudo add-apt-repository -y -n -U http://deb.debian.org/debian -c non-free -c non-free-firmware
sudo apt update
sudo apt -y install steamcmd

wget -O bepinex.zip "https://thunderstore.io/package/download/denikson/BepInExPack_Valheim/5.4.2202/"
unzip bepinex.zip -d ~/BepInExRaw

/usr/games/steamcmd +force_install_dir ~/VHINSTALL +login anonymous +app_update 896660 validate +exit

mv ~/VHINSTALL/valheim_server_Data/ ~/VHINSTALL/valheim_Data/
mv ~/BepInExRaw/BepInExPack_Valheim/* ~/VHINSTALL/

# Build
dotnet build DiscordConnector.sln