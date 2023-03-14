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

## Changelog

## Version 2.1.11

This is for build 0.214.2 onward of Valheim. The signature of the chat messages was changed which was causing an argument out of bounds exception with Discord Connector.

Fixes:

- Argument out of bounds exception which was occurring.

## Version 2.1.10

Thanks a lot to everyone who reported and helped resolve this error. Missed it in my initial testing because I was eager to test both webhooks and didn't end up testing with only one set.

Fixes:

- Null pointer error which would spam when only 1 webhook was set

## Version 2.1.9

Adds a requested feature for a second webhook. Both webhooks can be configured to accept messages of any type that Discord
Connector sends, and by default (to be non-breaking) they will send all messages (which is the behavior of 2.1.8 and previous).
Some plugins which may make use of Discord Connector's webhook to send messages can use the same method for sending and will
be tagged as 'other' webhook events. For a full list of what webhook events can be configured, see the
[documentation](https://discordconnector.valheim.nwest.games/config/main.html#webhook-events).

Features:

- Adds a second webhook entry
- Adds webhook events configuration entries (webhooks can be configured to only send certain messages)

Fixes:

- Empty leaderboards now send an empty leaderboard (instead of not sending anything)
- Configuration for how to differentiate players may have swapped the Name and NameAndPlayerId definitions

### Version 2.1.8

Fixes:

- Leader boards not being sent (but active players were)

### Version 2.1.7

Fixes:

- Death messages weren't respecting the POS enabled setting

### Version 2.1.6

Changes:

- Regress web request async changes until more reliable method is determined.

### Version 2.1.5

Fixes:

- Players leaving were being recorded as joining in the database

Changes:

- Web Request methods changed to used Async methods and `ContinueWith` instead of dispatching a task
- `%PUBLIC_IP%` variable now gets its information by asking the server instead of finding out itself

### Version 2.1.4

Fixes:

- Death messages were referencing "LeavePosEnabled" instead of "DeathPosEnabled"
- Shout messages were referencing "PingPosEnabled" instead of "ShoutPosEnabled"

### Version 2.1.3

Fixes:

- Extra JSON data was getting sent to Discord when some LiteDB lookups were happening

### Version 2.1.2

Mistlands update.

### Version 2.1.1

Fixes:

- Missing dependency in final bundle (error in csharp project file)

### Version 2.1.0

A full leaderboard overhaul is in the version. The previous settings for the statistic leaderboards are depreciated in favor of configuration defined statistic leaderboard settings. Look in the `discordconnector-leaderboards.cfg` file and configure any number of the 4 leaderboards to present the kind of data you want. In addition to multiple leaderboards, there are now time-based filters for the leaderboards; restrict them to today or this week or leave them set to all-time. By default, all leaderboards are disabled. If you were using a leaderboard before, you will have to set up a leaderboard to accomplish what you were sending before and enable it. Sorry for the inconvenience but this was the safest tradeoff.

Also relating to statistic leaderboards, there is a new statistic available for the leaderboards, 'Time Online' which uses the saved 'join' and 'leave' records to estimate a player's time on the server and present that as a value. This obviously doesn't work if you had disabled one or the other pieces of tracking (either disabled recording 'join' or 'leave' stats in the toggles config file). This values are calculated when the leaderboard is created but that should be OK since it is in a non-blocking task call.

The new Active Player's Announcement can be configured to announce server activity at a pre-defined interval. Configurable stats for it include players currently online, unique players online today, unique players online this week and unique players all time. It will use the same method set in the main config file (`discordconnector.cfg`) for how to determine individual players to count unique players for these time spans.

Additionally, the configuration files are nested in a subdirectory now. This is from a request on the plugin repository. When loading 2.1.0 (or future versions), the Discord Connector config files that are in the `BepInEx/config` directory will be automatically moved to the subdirectory and loaded from there. The subdirectory is `BepInEx/config/games.nwest.valheim.discordconnector`, and the config files themselves have shortened filenames. The records database is also moved to this subdirectory and renamed `records.db`.

Features:

- Adds new tracked stat for time on server (only works if you have enabled join and leave stats) *The duration provided will probably be inaccurate*
- Adds dynamically configured leaderboards (disabled by default)
- Adds an Active Players Announcement (disabled by default)

Changes:

- Configuration files are now nested in a subdirectory (first run will migrate them automatically)
- Database file moved into the subdirectory (first run will migrate it automatically)
- `config-debug.json` file is dumped to subdirectory after config load to be useful for debugging issues with the plugin (sensitive info is redacted, i.e. the webhook url)
- Multiple-choice config options use Enums on the backend now instead of Strings (may affect `discordconnector.cfg`: How to discern players in Record Retrieval)
- Building the plugin with the optimization flag present; in my tests, startup time of a Valheim server with just DiscordConnector installed was quicker
- Public IP is only queried if it is used (by including the %PUBLICIP% variable in a message)
