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

### Version 2.3.2

Built against the latest version of Valheim (0.219.14, the Bog Witch update).

Fixes

- Logging improvements for the Discord webhook requests
- Logging improvements for all our logs (a copy is saved to `BepInEx/config/games.nwest.valheim.discordconnector/vdc.log`)

These changes for logging are to help with troubleshooting plugin issues without requiring the full server log to do so (although
sometimes it would be necessary to see the full server log).

### Version 2.3.1

Features

- Added a few new variables related to timestamps and time
  - `%TIMESTAMP%` - replaced with `<t:UNIX_TIMESTAMP>` which Discord will convert to the user's local time
  - `%TIMESINCE%` - replaced with `<t:UNIX_TIMESTAMP:R>` which Discord will convert to a relative time (e.g. 2 hours ago)
  - `%UNIX_TIMESTAMP%` - replaced with the UNIX timestamp of the event (e.g. 1634567890). This can be used to create a custom timestamp format in the message.
  - Added a configuration option for the Active Players announcement which will allow you to disable the announcement when no one is online.
