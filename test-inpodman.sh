#!/bin/bash
set -e

dotnet build -c Release
podman compose stop
rm server-data/bepinex/plugins/DiscordConnector.dll
cp DiscordConnector/bin/Release/net481/DiscordConnector.dll server-data/bepinex/plugins/DiscordConnector.dll
podman compose up -d
podman compose logs -f