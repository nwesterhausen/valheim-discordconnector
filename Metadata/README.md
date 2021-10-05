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

### Version 0.6.0

Enabled for both the client and server versions of Valheim.

Key differences if running a server from the client:

- No Launch/Startup message is sent. This is because when the server launches it
immediately begins loading the world, but for the client it is loading into the
main menu. This may be fixable in the future to be a hook that goes before the
world begins getting loaded to keep the functionality on the server and to enable
similar functionality on the client.
