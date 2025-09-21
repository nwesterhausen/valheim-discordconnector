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

## Version 4.0.0

Fixes

- Should be logging all shouts correctly now.

Changes

- No longer requires the `DiscordConnector-Client` plugin to be installed on clients.
