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

## Version 3.0.0

Changes

- No longer tries to re-load the config if a change is detected. This never worked quite right.
- Build process changes, including using ILRepack to merge the dependencies into the main assembly.
  This means that there is now just a single `DiscordConnector.dll` file which includes this plugin & its dependencies.

Fixes

- Fixed a bug where the plugin would crash on the new version of Valheim due to the user detail changes.
