# Discord Connector

Connect your Valheim server (dedicated or served from the game itself) to a Discord Webhook. 
([How to get a webhook](https://github.com/nwesterhausen/valheim-discordconnector/blob/main/Metadata/HowtoGuide.md#how-to-get-a-discord-webhook) or [Short FAQ Guide for this mod](https://github.com/nwesterhausen/valheim-discordconnector/blob/main/Metadata/HowtoGuide.md))

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

### Version 0.7.1

Added config option to ignore players when sending shout messages to Discord.

### Version 0.7.0

Fixes:
- properly check for dedicated vs non-dedicated servers

Features:
- when sending position (POS or coordinates) with the message, will use an embed
to improve visibility (if enabled)
- added config options to enable/disable sending position with join and leave
- added config option to enable/disable using the embed with discord when sending
position data (disabled by default, I find it very busy when enabled atm)
- added config option to enable/disable sending position with pings
