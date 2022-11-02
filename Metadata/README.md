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

### Version 1.7.1

Fixes:

- Ignore player regex was matching everything if not set. Now if it is not set, it will match nothing.
- Player shout messages were not including enough information for formatting. Now they properly include steamId and shout text.

### Version 1.7.0

Features:

- New variable available for messages: `%PLAYER_STEAMID%` which gets replaced with the player's Steam ID

### Version 1.6.1

Fixes:

- Errors when accessing the ignored players regex

There was a typo that was affecting the way the config file was read. I didn't run into this in my testing on Windows but was able to duplicate this on Linux after it was reported. Thank you to those who reported this.

### Version 1.6.0

Finally a new release! This on is mainly some small features and bugfixes from the github issues backlog.

Features:

- New configuration setting, "Ignored Players (Regex)" lets you specify a regular expression to ignore players.
- Configuration is reloaded when a change is detected.
- The records database is saved in the BepInEx config directory.

Fixes:

- Handle exceptions that occur when checking the public IP from ipify.org
- Fully quality the records database to avoid any possible conflicts

Breaking Changes:

- Removed conversion code which would convert `records.json` into `records.db`.

  If you need to make use of that automatic conversion, load the 1.5.3 version of the plugin once before upgrading.

Full changelog history available on the
[Github repository](https://github.com/nwesterhausen/valheim-discordconnector/blob/main/Metadata/CHANGELOG.md)
or [discordconnector.valheim.nwest.games](https://discordconnector.valheim.nwest.games/changelog).
