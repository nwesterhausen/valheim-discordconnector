# Discord Connector

Connect your Valheim server to Discord. ([See website for installation or configuration instructions](https://discordconnector.valheim.nwest.games/)). This plugin is largely based on [valheim-discord-notifier](https://github.com/aequasi/valheim-discord-notifier), but this plugin supports randomized messages, muting players, and Discord message embeds.

Plugin available on [Thunderstore.io](https://valheim.thunderstore.io/package/nwesterhausen/DiscordConnector/), [NexusMods](https://www.nexusmods.com/valheim/mods/1551/), and [Mod Vault](https://modvault.xyz/viewmod/132).

### Plugin Details

See [the README](Metadata/README.md) for the plugin.

### Changelog

See [the changelog](Metadata/CHANGELOG.md).

## Development

To contribute or modify for a PR etc, simply clone this repository. A good set of
steps to follow to prepare your machine for development are listed in the
[BepInEx docs](https://docs.bepinex.dev/master/articles/dev_guide/plugin_tutorial/1_setup.html)
(essentially, make sure you have the .NET SDK setup, an IDE of your choice like
vscode, and that you are prepared to test the compiled plugin after you build it).

### Building

Use dotnet to restore and build the project. Post build, the compiled library and its
dependencies get copied into `bin/DiscordConnector` which enables you to simply copy
that folder into `$(GameDir)/BepinEx/plugins` for testing.

The compiled plugin will be in a zip ready for upload at `bin/DiscordConnector.zip`.

### Dependencies

For JSON serialization, I chose to use the System.Text.Json library which is part of
the most recent .NET but can be used with .NET 4.8 which is used in this project.

### Contributors

Thanks for the helpful contributions!

- @Digitalroot
- @nwesterhausen
- @thedefside

Thanks for an excellent original plugin @aequasi!