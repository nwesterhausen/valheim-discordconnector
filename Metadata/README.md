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

### Supported Message Notificaitons

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

- Fancier Discord messages
- Discord bot integration
- Multiple webhook support
- More statistics able to be sent

## Changelog

### Version 2.1.0

A full leaderboard overhaul is in the version. The previous settings for the statistic leaderboards are depreciated in favor of configuration defined statistic leaderboard settings. Look in the `discordconnector-leaderboards.cfg` file and configure any number of the 4 leaderboards to present the kind of data you want. In addition to multiple leaderboards, there are now time-based filters for the leaderboards; restrict them to today or this week or leave them set to all-time. By default, all leaderboards are disabled. If you were using a leaderboard before, you will have to set up a leaderboard to accomplish what you were sending before and enable it. Sorry for the inconvenience but this was the safest tradeoff.

Also relating to statistic leaderboards, there is a new statistic available for the leaderboards, 'Time Online' which uses the saved 'join' and 'leave' records to estimate a player's time on the server and present that as a value. This obviously doesn't work if you had disabled one or the other pieces of tracking (either disabled recording 'join' or 'leave' stats in the toggles config file). This values are calculated when the leaderboard is created but that should be OK since it is in a non-blocking task call.

Additionally, the configuration files are nested in a subdirectory now. This is from a request on the plugin repository. When loading 2.1.0 (or future versions), the Discord Connector config files that are in the `BepInEx/config` directory will be automatically moved to the subdirectory and loaded from there. The subdirectory is `BepInEx/config/games.nwest.valheim.discordconnector`, and the config files themselves have shortened filenames. The records database is also moved to this subdirectory and renamed `records.db`.

Features:

- Adds new tracked stat for time on server
- Adds dyanamically configured leaderboards

Changes:

- Configuration files are now nested in a subdirectory
- Database file moved into the subdirectory
- `config-debug.json` file is dumped to subdirectory after config load to be useful for debugging issues with the plugin

### Version 2.0.2

If a shout is performed by a player that isn't a real player (like a mod), it would break the shout call from working. This is because Discord Connector was trying to lookup the player's details and encountering null. The plugin now checks for that and returns early if null is found.

Fixes:

- Detect if a shout is by a non-player and gracefully exit.

### Version 2.0.1

With this update, we bring back Steam_ID variable inclusion and leaderboard message sending (respecting your config settings). I recommend you replace your `discordconnector.valheim.nwest.games-records.db` database, since the records will not line up and will be essentially soft-reset because the column name changed with the different type of data. Steam IDs are prefaced with 'Steam_' now, so you could migrate your stat database with a bit of effort. I believe this could all be done with queries inside the LiteDB Query Tool.

Fixes:

- Periodic leaderboard messages sending will now respect your config value intead of never sending
- The STEAMID variable works again. An alias is the PLAYERID variable, which does the same thing -- they both provide the full player id, so `Steam_<SteamID>` or `XBox_<XBoxID>`

Breaking changes:

- Player IDs are tracked in the stat database using a new column name, which resets any stat tracking because the player ID is used to resolve to a single player by combining with the character name.

### Version 2.0.0

Previous version broke with the new updates to Valheim using the PlayFab server stuff. Previously, the steam ID was grabbed directly from the socket but that doesn't work anymore. To get something workable (the other messages work), I have removed the code which tried to get the SteamID and disabled leaderboard sending.

Breaking changes:

- Removed steamid variable (internally) and tracking stats by steamid. This broke with the PlayFab changes to Valheim. It will be a bit involved to figure out how to deliver the same thing again, so if you have an idea or seen it done in another mod, please reach out with a Github Issue or ping on Discord.
- Leaderboard records will reset and a new database with suffix '-records2.db' will be saved anew. This is because what is being tracked is changed (used to be steamid, now it is using the character id).
- Perodic leaderboard messages will not send, ignoring the setting in the config (for now). This is until a more reliable method of determining players apart.

### Version 1.8.0

Features:

- Web requests to Discord API are async instead of blocking the main thread

Fixes:

- Handles the edge case when a toggle was enabled but the text in 'messages' for that toggle was blank, the plugin would crash. (e.g. if 'send shout' toggle was `true` but the 'shout message' was blank, in prior versions this would crash the plugin)

Full changelog history available on the
[Github repository](https://github.com/nwesterhausen/valheim-discordconnector/blob/main/Metadata/CHANGELOG.md)
or [discordconnector.valheim.nwest.games](https://discordconnector.valheim.nwest.games/changelog).
