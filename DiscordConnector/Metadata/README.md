# Discord Connector

Connect your Valheim server (dedicated or served from the game itself) to a Discord Webhook.
(Find mod documentation on [the official website](https://discord-connector.valheim.games.nwest.one/).)

## Features

- Set your own webhook, lets you configure icon, title, and a target channel
- Enable or Disable any messages
- Set what text gets sent for most messages
- Set more than one message for each type and have one randomly chosen!
- Record number of logins/deaths/pings and flavor the Discord messages
- Works with non-dedicated server (games opened to lan from the client)
- Configure custom leader boards to be sent periodically, listing rankings for any of the tracked stats

### Supported Message Notifications

- Server startup (server starting, loading the world)
- Server started (world loaded, ready to join)
- Server shutting down (server stopping)
- Player join
- Player leave
- Player shouting
- Player pinging
- Player death
- Random events start/pause/resume/end

## Abridged Changelog

## Version 3.1.0

Known Issues

- no leave notification
- a died notification is sent when a player logs in

Changes

- Updated to the latest version of Valheim (0.222.4)
- Improved discord embedding customization (thanks @engels74)
- Use an RPC to have the clients tell the server when they say things

Breaking Changes

- The `DiscordConnector-Client` plugin is required on clients for the new chat functionality to work. While you can still
  use just the server `DiscordConnector` plugin, you will not get the chat information (shouts, pings, etc.) from the clients.

The client plugin is on [Thunderstore](https://thunderstore.io/c/valheim/p/nwesterhausen/DiscordConnector_Client/) or can
be downloaded from the [GitHub releases](https://github.com/nwesterhausen/valheim-discordconnector/releases).