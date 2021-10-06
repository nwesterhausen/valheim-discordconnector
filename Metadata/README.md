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

### Version 0.9.0

Default config options are updated to be true for all notification and coordinates.

Features:

- Periodic stats leaderboard functionality (opt-in)

Fixes:

- Corrected duplicate "join" message when player dies
- Correctly looks at leave config option before sending leave message
- Correctly looks at join/death config option before sending messages

Improvements:

- Loaded config is now debug logged to make debugging easier
