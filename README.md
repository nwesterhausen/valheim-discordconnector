# Discord Connector

Connect your Valheim server to Discord. ([See website for installation or configuration instructions](https://discordconnector.valheim.nwest.games/)). This plugin has its core idea from [valheim-discord-notifier](https://github.com/aequasi/valheim-discord-notifier), but this plugin supports randomized messages, muting players, Discord message embeds, and has Discord Bot Integration support.

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

#### Building without the Discord Bot Webhook

When compiling, you can set the property 'NoBotSupport' to 1 to compile a version without any
listening webhook code (and therefore won't support the Discord Bot).

`dotnet build /p:NoBotSupport=1`

This completely skips compiling the webhook listener code, removes any webhook listener properties
in the plugin, and changes the assembly details. Instead of having a GUID of `games.nwest.valheim.discordconnector`
it will use `games.nwest.valheim.discordconnector-nobotsupport` and the name of the assembly will
include '(No Discord Bot Support)'. It also will log a message when loaded by BepInEx that says this
version doesn't include the bot support.

When you build with this property set, the destination is `bin/DiscordConnector-NoBotSupport` and
a zipped version at `bin/DiscordConnector-NoBotSupport.zip`.

### Dependencies

For JSON serialization, I chose to use Newtonsoft.Json, the library files are included in this repository in `lib/Newtonsoft.Json`.

### Contributors

Thanks for the helpful contributions!

- @Digitalroot
- @nwesterhausen
- @thedefside

Thanks for an excellent original plugin @aequasi!