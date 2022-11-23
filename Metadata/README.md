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

- Multiple webhook support
- More statistics trackable/able to be sent
- New day messages

## Changelog

### Version 2.1.0

A full leaderboard overhaul is in the version. The previous settings for the statistic leaderboards are depreciated in favor of configuration defined statistic leaderboard settings. Look in the `discordconnector-leaderboards.cfg` file and configure any number of the 4 leaderboards to present the kind of data you want. In addition to multiple leaderboards, there are now time-based filters for the leaderboards; restrict them to today or this week or leave them set to all-time. By default, all leaderboards are disabled. If you were using a leaderboard before, you will have to set up a leaderboard to accomplish what you were sending before and enable it. Sorry for the inconvenience but this was the safest tradeoff.

Also relating to statistic leaderboards, there is a new statistic available for the leaderboards, 'Time Online' which uses the saved 'join' and 'leave' records to estimate a player's time on the server and present that as a value. This obviously doesn't work if you had disabled one or the other pieces of tracking (either disabled recording 'join' or 'leave' stats in the toggles config file). This values are calculated when the leaderboard is created but that should be OK since it is in a non-blocking task call.

The new Active Player's Announcement can be configured to announce server activity at a pre-defined interval. Configurable stats for it include players currently online, unique players online today, unique players online this week and unique players all time. It will use the same method set in the main config file (`discordconnector.cfg`) for how to determine individual players to count unique players for these time spans.

Additionally, the configuration files are nested in a subdirectory now. This is from a request on the plugin repository. When loading 2.1.0 (or future versions), the Discord Connector config files that are in the `BepInEx/config` directory will be automatically moved to the subdirectory and loaded from there. The subdirectory is `BepInEx/config/games.nwest.valheim.discordconnector`, and the config files themselves have shortened filenames. The records database is also moved to this subdirectory and renamed `records.db`.

Features:

- Adds new tracked stat for time on server
- Adds dynamically configured leaderboards
- Adds an Active Players Announcement (disabled by default)

Changes:

- Configuration files are now nested in a subdirectory (first run will migrate them automatically)
- Database file moved into the subdirectory (first run will migrate it automatically)
- `config-debug.json` file is dumped to subdirectory after config load to be useful for debugging issues with the plugin (sensitive info is redacted, i.e. the webhook url)
- Multiple-choice config options use Enums on the backend now instead of Strings (may affect `discordconnector.cfg`: How to discern players in Record Retrieval)
- Building the plugin with the optimization flag present; in my tests, startup time of a Valheim server with just DiscordConnector installed was quicker
