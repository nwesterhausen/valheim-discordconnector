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

### Version 2.0.4

Features:

- Adds a config option to enable sending non-player shouts to Discord. This is in the main config file and disabled by default.

### Version 2.0.3

Other Changes:

- Set BepInEx depencency to eactly 5.4.19 instead of 5.* (this stops a warning from showing up)

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
