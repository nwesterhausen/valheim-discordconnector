# Discord Connector

[![Build and test](https://github.com/Yibbzz/valheim-discordconnector_fix/actions/workflows/build-on-push-pr.yml/badge.svg)](https://github.com/Yibbzz/valheim-discordconnector_fix/actions/workflows/build-on-push-pr.yml)
[![CodeQL](https://github.com/Yibbzz/valheim-discordconnector_fix/actions/workflows/codeql-analysis.yml/badge.svg)](https://github.com/Yibbzz/valheim-discordconnector_fix/actions/workflows/codeql-analysis.yml)
![Server Version (Main)](https://img.shields.io/badge/dynamic/json?url=https%3A%2F%2Fgithub.com%2FYibbzz%2Fvalheim-discordconnector_fix%2Fraw%2Frefs%2Fheads%2Fmain%2FDiscordConnector%2FMetadata%2Fmanifest.json&query=%24.version_number&prefix=v&style=flat&label=Server%20Version%20(Main)&color=darkviolet&labelColor=%2332393F)
![Client Version (Main)](https://img.shields.io/badge/dynamic/json?url=https%3A%2F%2Fgithub.com%2FYibbzz%2Fvalheim-discordconnector_fix%2Fraw%2Frefs%2Fheads%2Fmain%2FDiscordConnector.Client%2FMetadata%2Fmanifest.json&query=%24.version_number&prefix=v&style=flat&label=Client%20Version%20(Main)&color=darkviolet&labelColor=%2332393F)
![Built and tested against Valheim 1.0.20](https://img.shields.io/badge/Built_and_tested_against_Valheim-1.0.20-purple?style=flat&labelColor=%2332393F)
[![Thunderstore Version](https://img.shields.io/badge/dynamic/json?url=https%3A%2F%2Fthunderstore.io%2Fapi%2Fexperimental%2Fpackage%2Fnwesterhausen%2FDiscordConnector%2F&query=%24.latest.version_number&prefix=v&label=Thunderstore&labelColor=%2332393F&color=%2323FFB0)](https://thunderstore.io/c/valheim/p/nwesterhausen/DiscordConnector/)
[![Thunderstore Version Client](https://img.shields.io/badge/dynamic/json?url=https%3A%2F%2Fthunderstore.io%2Fapi%2Fexperimental%2Fpackage%2Fnwesterhausen%2FDiscordConnector_Client%2F&query=%24.latest.version_number&prefix=v&label=Thunderstore%20(Client)&labelColor=%2332393F&color=%2323FFB0)](https://thunderstore.io/c/valheim/p/nwesterhausen/DiscordConnector_Client/)
[![NexusMods](https://img.shields.io/badge/NexusMods-2.1.14-%23D98F40?style=flat&labelColor=%2332393F)](https://www.nexusmods.com/valheim/mods/1551/)

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
   DiscordConnector.sln
```

Post build, the compiled library and its dependencies get copied into `bin/DiscordConnector` which enables you to simply copy that folder into `$(GamePath)/BePinEx/plugins` for testing or use.

The compiled plugin will be in a zip ready for upload at `bin/DiscordConnector.zip`.

You don't need the game client to build: the free Valheim dedicated server has the same game files, in
`valheim_server_Data` instead of `valheim_Data`. Link one to the other (`mklink /J valheim_Data valheim_server_Data`
in the server folder) and set `GamePath` to the server folder. This is what the GitHub workflows do
(see `.github/actions/setup-valheim`).

## Testing

The mod is tested inside a real Valheim dedicated server with a fake Discord webhook. See [tests/README.md](tests/README.md).
Every push and pull request to `main` builds the mod and runs the full test suite on GitHub Actions.

### Dependencies

For JSON serialization, using Newtonsoft.Json

For data storage/retrieval using [LiteDB](https://www.litedb.org/)
(If you want to read the database file generated, you can use [LiteDB Studio](https://github.com/mbdavid/LiteDB.Studio/releases/latest))

## Release Steps

Before release, to bump the version the following needs changed:

1. Update the version of the plugin in these files
   - `{src}/PluginInfo.cs`
   - `{src}/Metadata/DiscordConnector-Nexus.readme`
   - `{src}/Metadata/manifest.json`
   - `{src}/Metadata/thunderstore.toml`
2. Finalize the changelog entry in `docs/changelog.md`
3. Copy changelog notes into `{src}/Metadata/README.md`
