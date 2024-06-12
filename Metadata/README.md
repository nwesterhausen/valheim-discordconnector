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

### Roadmap

See the [current roadmap](https://github.com/nwesterhausen/valheim-discordconnector/projects/1) as a Github project.

- More statistics trackable/able to be sent
- New day messages

## Abridged Changelog

### Version 2.3.0

Features

- support up to 16 additional webhooks (adds a new config file, `discordconnector-extraWebhooks.cfg`)
- support restricting/allowing Discord mentions in messages (`@here` and `@everyone` are disabled by default -- possibly a breaking change)
- custom variables are now evaluated again for any embedded variables in the message
- added new configuration values to allow specifying a custom username and avatar for each webhook (to override the Discord webhook settings)
- added a configuration value that sets a default username for all webhooks (if not overridden)

Fixes

- no longer relies on the `ZNet.GetPublicIP()` method and instead gets the public IP on its own at server start.
- `%NUM_PLAYERS%` proactively subtracts 1 if the event is a player leaving
