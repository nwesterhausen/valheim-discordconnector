# Discord Connector

Connect your Valheim server (dedicated or served from the game itself) to a Discord Webhook.
(Find mod documentation on [the official website](https://discordconnector.valheim.nwest.games/).)

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

### Version 2.2.2

Features

- add `%JOIN_CODE%` variable which will show the join code if crossplay is enabled
- add `%NUM_PLAYERS%` variable which will show the number of online players

Fixes

- updated the documentation to reflect how `%WORLD_NAME%`, `%PUBLIC_IP%`, and `%DAY_NUMBER%` can be
in any messages and be replaced.

Note: At server startup, some variables may not be available. They all should be available when server
is launched, but the join code may take a bit longer to display -- more testing is needed to know exactly
how much extra time it needs on average. If it is consistently unavailable, please file an issue and we
can come up with either a delayed startup message or another event that fires when the code becomes not
empty or changes.
