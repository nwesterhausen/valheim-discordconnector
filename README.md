# Discord Connector

[![CodeQL](https://github.com/nwesterhausen/valheim-discordconnector/actions/workflows/codeql-analysis.yml/badge.svg)](https://github.com/nwesterhausen/valheim-discordconnector/actions/workflows/codeql-analysis.yml)
[![Build](https://github.com/nwesterhausen/valheim-discordconnector/actions/workflows/dotnet.yml/badge.svg)](https://github.com/nwesterhausen/valheim-discordconnector/actions/workflows/dotnet.yml)
[![GitHub release (latest by date)](https://img.shields.io/github/v/release/nwesterhausen/valheim-discordconnector?label=Github%20Release&style=flat&labelColor=%2332393F)](https://github.com/nwesterhausen/valheim-discordconnector/releases/latest)
[![Thunderstore.io](https://img.shields.io/badge/Thunderstore.io-2.3.3-%23375a7f?style=flat&labelColor=%2332393F)](https://valheim.thunderstore.io/package/nwesterhausen/DiscordConnector/)
[![NexusMods](https://img.shields.io/badge/NexusMods-2.1.14-%23D98F40?style=flat&labelColor=%2332393F)](https://www.nexusmods.com/valheim/mods/1551/)
![Built against Valheim version](https://img.shields.io/badge/Built_against_Valheim-0.219.14-purple?style=flat&labelColor=%2332393F)

Connect your Valheim server to Discord. ([See website for installation or configuration instructions](https://discord-connector.valheim.games.nwest.one/)). This plugin is largely based on [valheim-discord-notifier](https://github.com/aequasi/valheim-discord-notifier), but this plugin supports randomized messages, muting players, and Discord message embeds.

## Plugin Details

See [the README](Metadata/README.md) for the plugin.

## Changelog

See [the changelog](docs/changelog.md).

## Building

To build, first get the path to your Valheim installation and also use the publicize tool to create a publicized version of the game. I'm not sure without that, if it will fail to build or not.

Then, run the following command to build the project:

```shell
dotnet build \
   -c Release \
   /p:GamePath="C:\Program Files (x86)\Steam\steamapps\common\Valheim" \
   valheim-discordconnector.sln
```

Post build, the compiled library and its dependencies get copied into `bin/DiscordConnector` which enables you to simply copy that folder into `$(GamePath)/BePinEx/plugins` for testing or use.

The compiled plugin will be in a zip ready for upload at `bin/DiscordConnector.zip`.

### Dependencies

For JSON serialization, using Newtonsoft.Json

For data storage/retrieval using [LiteDB](https://www.litedb.org/)
(If you want to read the database file generated, you can use [LiteDB Studio](https://github.com/mbdavid/LiteDB.Studio/releases/latest))

## Release Steps

Before release, to bump the version the following needs changed:

1. Update the version of the plugin in these files
   - `src/PluginInfo.cs`
   - `Metadata/DiscordConnector-Nexus.readme`
   - `Metadata/manifest.json`
   - `Metadata/thunderstore.toml`
2. Finalize the changelog entry in `docs/changelog.md`
3. Copy changelog notes into `Metadata/README.md`
