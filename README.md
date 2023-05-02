# Discord Connector

[![CodeQL](https://github.com/nwesterhausen/valheim-discordconnector/actions/workflows/codeql-analysis.yml/badge.svg)](https://github.com/nwesterhausen/valheim-discordconnector/actions/workflows/codeql-analysis.yml)
[![Build](https://github.com/nwesterhausen/valheim-discordconnector/actions/workflows/dotnet.yml/badge.svg)](https://github.com/nwesterhausen/valheim-discordconnector/actions/workflows/dotnet.yml)
[![Publish](https://github.com/nwesterhausen/valheim-discordconnector/actions/workflows/publish.yml/badge.svg)](https://github.com/nwesterhausen/valheim-discordconnector/actions/workflows/publish.yml)
[![GitHub release (latest by date)](https://img.shields.io/github/v/release/nwesterhausen/valheim-discordconnector?label=Github%20Release&style=flat&labelColor=%2332393F)](https://github.com/nwesterhausen/valheim-discordconnector/releases/latest)
[![Thunderstore.io](https://img.shields.io/badge/Thunderstore.io-2.1.13-%23375a7f?style=flat&labelColor=%2332393F)](https://valheim.thunderstore.io/package/nwesterhausen/DiscordConnector/)
[![NexusMods](https://img.shields.io/badge/NexusMods-2.1.13-%23D98F40?style=flat&labelColor=%2332393F)](https://www.nexusmods.com/valheim/mods/1551/)

Connect your Valheim server to Discord. ([See website for installation or configuration instructions](https://discordconnector.valheim.nwest.games/)). This plugin is largely based on [valheim-discord-notifier](https://github.com/aequasi/valheim-discord-notifier), but this plugin supports randomized messages, muting players, and Discord message embeds.

## Plugin Details

See [the README](Metadata/README.md) for the plugin.

## Changelog

See [the changelog](docs/changelog.md).

### Building

Before building, you must update the `GameDir` property in the `csproj` file.

Use dotnet to restore and build the project. Post build, the compiled library and its
dependencies get copied into `bin/DiscordConnector` which enables you to simply copy
that folder into `$(GameDir)/BePinEx/plugins` for testing.

The compiled plugin will be in a zip ready for upload at `bin/DiscordConnector.zip`.

### Dependencies

For JSON serialization, using Newtonsoft.Json

For data storage/retrieval using [LiteDB](https://www.litedb.org/)
(If you want to read the database file generated, you can use [LiteDB Studio](https://github.com/mbdavid/LiteDB.Studio/releases/latest))

### Contributors

Thanks for the helpful contributions!

- @Digitalroot
- @nwesterhausen
- @thedefside

Thanks for an excellent original plugin @aequasi!
