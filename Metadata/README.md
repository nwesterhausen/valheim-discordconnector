# Discord Connector

Connect your Valheim server (dedicated or served from the game itself) to a Discord Webhook. 
([How to get a webhook](https://github.com/nwesterhausen/valheim-discordconnector/blob/main/Metadata/HowtoGuide.md#how-to-get-a-discord-webhook) or [Short FAQ Guide for this mod](https://github.com/nwesterhausen/valheim-discordconnector/blob/main/Metadata/HowtoGuide.md)) There's also [configuration documentation](https://github.com/nwesterhausen/valheim-discordconnector/blob/main/Metadata/ConfigurationDetails.md).

## Features

- Set your own webhook, lets you configure icon, title, and a target channel
- Enable or Disable any messages
- Set what text gets sent for most messages
- Set more than one message for each type and have one randomly chosen!
- Record number of logins/deaths/pings and flavor the Discord messages
- Works with non-dedicated server (games opened to lan from the client)

### Supported Message Notificaitons

- Server startup (server starting, loading the world)
- Server started (world loaded, ready to join)
- Server shutting down (server stopping)
- Player join
- Player leave
- Player shouting
- Player pinging

### Roadmap

- Message when events start/end
- Player death
- Fancier Discord messages

## Changelog

Full changelog history available on the
[Github repository](https://github.com/nwesterhausen/valheim-discordconnector/blob/main/Metadata/CHANGELOG.md).

### Version 1.0.0

Release 1.0.0 is a breaking release since the structure of the configuration files completely changes. When you update you will need to modify the config
to save your webhook again and to update any message customization you have done!

Features:

- Send an extra message the first time a player does something (by default only for Join and Death on server)
- Configuration is "simpler" with other configuration files to consult for full customization
- Server shutdown message

Fixes:

- Global toggles weren't being applied

Other Changes:

- Mention Mod Vault in readme

This version included a source code restructuring which should make it easier to maintain in the future.
