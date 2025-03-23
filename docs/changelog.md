# Changelog

A full changelog of changes, dating all the way back to the first release.

## Version 3.1.0

Changes

- Updated to the latest version of Valheim (0.222.4)
- Improved discord embedding customization (thanks @engels74)
- Use an RPC to have the clients tell the server when they say things

Breaking Changes

- The `DiscordConnector-Client` plugin is required on clients for the new chat functionality to work. While you can still
use just the server `DiscordConnector` plugin, you will not get the chat information (shouts, pings, etc.) from the clients.

The client plugin is on [Thunderstore](https://thunderstore.io/c/valheim/p/nwesterhausen/DiscordConnector-Client/) or can
be downloaded from the [GitHub releases](https://github.com/nwesterhausen/valheim-discordconnector/releases).

## Version 3.0.0

Changes

- No longer tries to re-load the config if a change is detected. This never worked quite right.
- Build process changes, including using ILRepack to merge the dependencies into the main assembly.
This means that there is now just a single `DiscordConnector.dll` file which includes this plugin & its dependencies.

Fixes

- Fixed a bug where the plugin would crash on the new version of Valheim due to the user detail changes.
Thanks @ilyas-elbani for contributing this fix!

## Version 2.3.3

Fixes

- Can't start plugin when the old log exists in some situations

## Version 2.3.2

Built against the latest version of Valheim (0.219.14, the Bog Witch update).

Fixes

- Logging improvements for the Discord webhook requests
- Logging improvements for all our logs (a copy is saved to `BepInEx/config/games.nwest.valheim.discordconnector/vdc.log`)

These changes for logging are to help with troubleshooting plugin issues without requiring the full server log to do so (although
sometimes it would be necessary to see the full server log).

## Version 2.3.1

Features

- Added a few new variables related to timestamps and time
  - `%TIMESTAMP%` - replaced with `<t:UNIX_TIMESTAMP>` which Discord will convert to the user's local time
  - `%TIMESINCE%` - replaced with `<t:UNIX_TIMESTAMP:R>` which Discord will convert to a relative time (e.g. 2 hours ago)
  - `%UNIX_TIMESTAMP%` - replaced with the UNIX timestamp of the event (e.g. 1634567890). This can be used to create a custom timestamp format in the message.
  - Added a configuration option for the Active Players announcement which will allow you to disable the announcement when no one is online.

## Version 2.3.0

Features

- support up to 16 additional webhooks (adds a new config file, `discordconnector-extraWebhooks.cfg`)
- support restricting/allowing Discord mentions in messages (`@here` and `@everyone` are disabled by default -- possibly a breaking change)
- custom variables are now evaluated again for any embedded variables in the message
- added new configuration values to allow specifying a custom username and avatar for each webhook (to override the Discord webhook settings)
- added a configuration value that sets a default username for all webhooks (if not overridden)

Fixes

- no longer relies on the `ZNet.GetPublicIP()` method and instead gets the public IP on its own at server start.
- `%NUM_PLAYERS%` proactively subtracts 1 if the event is a player leaving

## Version 2.2.2

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

## Version 2.2.1

Features

- Server New Day event and message (@jterrace)
- `%DAY_NUMBER%` variable replacement (@jterrace)

## Version 2.2.0

Features

- Working time online leaderboard (@jterrace)

Fixes

- Build against the Ashlands update
- Properly calculate time online (@jterrace)
- Ping show the ping POS instead of player's POS (@jimw383)

## Version 2.1.17

Fixes:

- Build against latest Valheim version

## Version 2.1.16

Fixes:

- include additional JSON dependency

## Version 2.1.15

Fixes:

- update to latest Valheim version

## Version 2.1.14

Fixes:

- error from ZDOID property changes preventing player actions being sent
- bumps BepInEx dependency to latest on Thunderstore

## Version 2.1.13

Features:

- Added send shouts in all caps toggle.
- Added the "Log Debug Messages" setting for improved troubleshooting.

Fixes:

- Updated build against Valheim 0.215.2

## Version 2.1.12

Update to Valheim 0.214.300

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
[documentation](https://discord-connector.valheim.games.nwest.one/config/main.html#webhook-events).

Features:

- Adds a second webhook entry
- Adds webhook events configuration entries (webhooks can be configured to only send certain messages)

Fixes:

- Empty leaderboards now send an empty leaderboard (instead of not sending anything)
- Configuration for how to differentiate players may have swapped the Name and NameAndPlayerId definitions

## Version 2.1.8

Fixes:

- Leader boards not being sent (but active players were)

## Version 2.1.7

Fixes:

- Death messages weren't respecting the POS enabled setting

## Version 2.1.6

Changes:

- Regress web request async changes until more reliable method is determined.

## Version 2.1.5

Fixes:

- Players leaving were being recorded as joining in the database

Changes:

- Web Request methods changed to used Async methods and `ContinueWith` instead of dispatching a task
- `%PUBLIC_IP%` variable now gets its information by asking the server instead of finding out itself

## Version 2.1.4

Fixes:

- Death messages were referencing "LeavePosEnabled" instead of "DeathPosEnabled"
- Shout messages were referencing "PingPosEnabled" instead of "ShoutPosEnabled"

## Version 2.1.3

Fixes:

- Extra JSON data was getting sent to Discord when some LiteDB lookups were happening

## Version 2.1.2

Mistlands update.

## Version 2.1.1

Fixes:

- Missing dependency in final bundle (error in csharp project file)

## Version 2.1.0

A full leaderboard overhaul is in the version. The previous settings for the statistic leaderboards are depreciated in favor of configuration defined statistic leaderboard settings. Look in the `discordconnector-leaderboards.cfg` file and configure any number of the 4 leaderboards to present the kind of data you want. In addition to multiple leaderboards, there are now time-based filters for the leaderboards; restrict them to today or this week or leave them set to all-time. By default, all leaderboards are disabled. If you were using a leaderboard before, you will have to set up a leaderboard to accomplish what you were sending before and enable it. Sorry for the inconvenience but this was the safest tradeoff.

Also relating to statistic leaderboards, there is a new statistic available for the leaderboards, 'Time Online' which uses the saved 'join' and 'leave' records to estimate a player's time on the server and present that as a value. This obviously doesn't work if you had disabled one or the other pieces of tracking (either disabled recording 'join' or 'leave' stats in the toggles config file). This values are calculated when the leaderboard is created but that should be OK since it is in a non-blocking task call.

The new Active Player's Announcement can be configured to announce server activity at a pre-defined interval. Configurable stats for it include players currently online, unique players online today, unique players online this week and unique players all time. It will use the same method set in the main config file (`discordconnector.cfg`) for how to determine individual players to count unique players for these time spans.

Additionally, the configuration files are nested in a subdirectory now. This is from a request on the plugin repository. When loading 2.1.0 (or future versions), the Discord Connector config files that are in the `BepInEx/config` directory will be automatically moved to the subdirectory and loaded from there. The subdirectory is `BepInEx/config/games.nwest.valheim.discordconnector`, and the config files themselves have shortened filenames. The records database is also moved to this subdirectory and renamed `records.db`.

Features:

- Adds new tracked stat for time on server (only works if you have enabled join and leave stats)
- Adds dynamically configured leaderboards (disabled by default)
- Adds an Active Players Announcement (disabled by default)

Changes:

- Configuration files are now nested in a subdirectory (first run will migrate them automatically)
- Database file moved into the subdirectory (first run will migrate it automatically)
- `config-debug.json` file is dumped to subdirectory after config load to be useful for debugging issues with the plugin (sensitive info is redacted, i.e. the webhook url)
- Multiple-choice config options use Enums on the backend now instead of Strings (may affect `discordconnector.cfg`: How to discern players in Record Retrieval)
- Building the plugin with the optimization flag present; in my tests, startup time of a Valheim server with just DiscordConnector installed was quicker
- Public IP is only queried if it is used (by including the %PUBLICIP% variable in a message)

## Version 2.0.8

Changes:

- `%WORLD_NAME%` will now only replace with world name once server has started up to avoid an issue with Key Manager

## Version 2.0.7

Changes:

- Further guards against null-reference exceptions

## Version 2.0.6

Fixes:

- Fixes plugin crash that could occur if the game was initiated more than once.
- Removed extraneous discord message on server load

## Version 2.0.5

Features:

- Adds a config option to format how position data is formatted
- Adds a config option to format how the automatically-appended position data is formatted
- Adds a new variable which can be used in any messages: `%WORLD_NAME%` turns into the name of the world.

Changes:

- `%POS%` now renders without the enclosing parentheses.

## Version 2.0.4

Features:

- Adds a config option to enable sending non-player shouts to Discord. This is in the main config file and disabled by default.

## Version 2.0.3

Other Changes:

- Set BepInEx dependency to exactly 5.4.19 instead of 5.* (this stops a warning from showing up)

## Version 2.0.2

If a shout is performed by a player that isn't a real player (like a mod), it would break the shout call from working. This is because Discord Connector was trying to lookup the player's details and encountering null. The plugin now checks for that and returns early if null is found.

Fixes:

- Detect if a shout is by a non-player and gracefully exit.

## Version 2.0.1

With this update, we bring back Steam_ID variable inclusion and leader board message sending (respecting your config settings). I recommend you replace your `discordconnector.valheim.nwest.games-records.db` database, since the records will not line up and will be essentially soft-reset because the column name changed with the different type of data. Steam IDs are prefaced with 'Steam_' now, so you could migrate your stat database with a bit of effort. I believe this could all be done with queries inside the LiteDB Query Tool.

Fixes:

- Periodic leader board messages sending will now respect your config value instead of never sending
- The `%STEAMID%` variable works again. An alias is the `%PLAYERID%` variable, which does the same thing -- they both provide the full player id, so `Steam_<SteamID>` or `XBox_<XBoxID>`

Breaking changes:

- Player IDs are tracked in the stat database using a new column name, which resets any stat tracking because the player ID is used to resolve to a single player by combining with the character name.

## Version 2.0.0

Previous version broke with the new updates to Valheim using the PlayFab server stuff. Previously, the steam ID was grabbed directly from the socket but that doesn't work anymore. To get something workable (the other messages work), I have removed the code which tried to get the SteamID and disabled leader board sending.

Breaking changes:

- Removed steamid variable (internally) and tracking stats by steamid. This broke with the PlayFab changes to Valheim. It will be a bit involved to figure out how to deliver the same thing again, so if you have an idea or seen it done in another mod, please reach out with a Github Issue or ping on Discord.
- Leader board records will reset and a new database with suffix '-records2.db' will be saved anew. This is because what is being tracked is changed (used to be steamid, now it is using the character id).
- Periodic leader board messages will not send, ignoring the setting in the config (for now). This is until a more reliable method of determining players apart.

## Version 1.8.0

Features:

- Web requests to Discord API are async instead of blocking the main thread

Fixes:

- Handles the edge case when a toggle was enabled but the text in 'messages' for that toggle was blank, the plugin would crash. (e.g. if 'send shout' toggle was `true` but the 'shout message' was blank, in prior versions this would crash the plugin)

## Version 1.7.1

Fixes:

- Ignore player regex was matching everything if not set. Now if it is not set, it will match nothing.
- Player shout messages were not including enough information for formatting. Now they properly include steamId and shout text.

## Version 1.7.0

Features:

- New variable available for messages: `%PLAYER_STEAMID%` which gets replaced with the player's Steam ID

Other Changes:

- Switched from ipify.org to [ipconfig.me](https://ifconfig.me) for grabbing public IP address

## Version 1.6.1

Fixes:

- Errors when accessing the ignored players regex

There was a typo that was affecting the way the config file was read. I didn't run into this in my testing on Windows but was able to duplicate this on Linux after it was reported. Thank you to those who reported this.

## Version 1.6.0

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

## Version 1.5.3

Fixes:

- Leader board interval was half of what was configured (now is properly minutes)

## Version 1.5.2

Fixes:

- Highest and Lowest leader boards were not checking the correct tables
- Configurable retrieval strategy for all records (either SteamID, PLayer Name, or both) -- always returns player names

Due to how records.json recorded stats and the LiteDB, you will not be able to use the old records with strategies
involving the SteamID because prior to 1.5.0 we were not recording the SteamID with the record.

## Version 1.5.1

Fixes:

- Toggles for the bottom n players leader boards (inverse ranked leader boards)

## Version 1.5.0

Features:

- Using LiteDB for record storage.

Because of how unreliable storing the records in a "roll-your-own"
database with a JSON file was, and because of the increased flexibility
in what could be stored, I've changed the storage system for the
recorded player stats to use LiteDB. Currently this means records for
join/leave/death/shout/ping will be timestamped, include the position of
the event, have the player name, and the player's steamid. Hopefully
adding this additional information will allow for more customization
options for the users of this mod.

It is set up to do a migration on first load of the updated plugin, the
steps it follows for that is:

1. check if records.json (or configured name) exists
2. read all records from the file
3. parse the records
4. loop through all the records and add them to the database (Records added this way will have position of zero and a steamid of 1.)
5. move the records.json file to records.json.migrated

If you don't want to have it auto-migrate the records, rename your
records.json or delete it. If the name does not match exactly it will
not migrate the data.

For the migration steps, it will be outputting log information (at INFO
level) with how many records were migrated and which steps completed.

- Ranked Lowest Player Leader board

Added an inverse of the Top Player leader board.

- Custom leader board heading messages

Added configuration for the messages sent at the top of the leader board
messages.

- The variable `%PUBLICIP%` can be used in _any_ message configuration
  now.

## Version 1.4.4

Fixes:

- Position being sent with event messages even if event position was disabled in config

## Version 1.4.3

Fixes:

- Event messages were sending the wrong message (start instead of end and vice-versa)
- Event Stop messages were sending zero coordinates
- If you had enabled first death message and death message (this is default settings), you would
  get two messages. This has been changed to merge the messages into one if both settings are on
  and it's a player's first death.

Features:

- Added toggles to enable/disable some event debug messages (all disabled by default)
- Added a toggle to enable/disable a debug message with responses from the webhook (disabled by default)

## Version 1.4.2

Fixes:

- Least deaths leader board wasn't respecting the correct config entry. (THanks @thedefside)

## Version 1.4.1

Fixes:

- Removed the two debug logging calls for events -- sorry for the log spam!

## Version 1.4.0

Features:

- 10 user defined variables that can be used an any messages (%VAR1% thru %VAR10%). These are set in their own configuration file,
  `games.nwest.valheim.discordconnector-variables.cfg` which will get generated first time 1.4.0 is run.
- The position of where the player/ping/event coordinates are inserted into messages is configurable using the `%POS%` variable in
  the messages config. It won't be replaced if the "send coordinates" toggle is off for that message. If you don't include a `%POS%`
  variable, it will append the coordinates as happens with previous versions.

Fixes:

- Fixed an off-by-one error in the Top Players leader board (the default leader board) (Thanks @thedefside)
- Fixed configuration not referencing proper settings (Thanks @thedefside)
- Fixed event messages (now properly functioning on dedicated servers)

Breaking Changes:

- If you used `%PLAYERS%` in any of the event messages, you need to remove it. With the changes required for the event messages
  functionality, it is not supportable at this time.

## Version 1.3.0

Features:

- Additional leader board options. The existing leader board option will now default to sending top 3 players for what is enabled.
  You can enable a highest and lowest leader board for each tracked stat now. All leader boards get sent on the same interval.

## Version 1.2.2

Fixes:

- No shutdown message when some other mods are loaded (Like World of Valheim suite)

Also this update modifies when the startup, shutting down, and shut down messages are sent. There now will likely be a bit
of a pause because the startup message gets sent when the game is initialized instead of when the loading of the map starts
for the server.

## Version 1.2.1

Fixes:

- The leader board toggles were not working properly, behind the scenes they were all following the death leader board toggle

A breaking change was found with the records.json in 1.2.0. The records.json file needs to have all `PlayerName` changed to `Key`.
If you are seeing an error message in your logs from Discord Connector, this is the likely culprit (should see something about
JsonException I believe). For example:

records.json pre 1.2.0:

```json
[{"Category":"death","Values":[{"PlayerName":"Xithyr","Value":13} ...
```

records.json 1.2.0+ (PlayerName changed to Key)

```json
[{"Category":"death","Values":[{"Key":"Xithyr","Value":13} ...
```

## Version 1.2.0

Features:

- `%PUBLICIP%` message variable available in the server messages

  There is no variable for what port the game is running on since I figured that had to be set manually in the first place (if not default),
  and you should be able to modify the message to be something like `Join the server on %PUBLICIP%:2457` or similar if you want to.

- Messages for events start/pause/stopping

  A feature that I wanted finally added. This was difficult to test in a server environment and I did the testing running on the client and then
  the client running as a server. In those instances I verified that the messages were fired when events started, ended, paused or resumed. The
  resume message is the same as the start message by default because I couldn't think of a way to word it that made sense.

## Version 1.1.1

Fix:

- Stop and Loaded config values were using the same value as launched on the backend and not respecting the actual config.

## Version 1.1.0

Features:

- Send a message when the server saves the world

Fixes:

- Configuration file comments should be clearer/easier to understand

## Version 1.0.0

**Release 1.0.0 is a breaking release** since the structure of the configuration files completely changes. When you update you will need to modify the config
to save your webhook again and to update any message customization you have done!

Features:

- Send an extra message the first time a player does something (by default only for Join and Death on server)
- Configuration is "simpler" with other configuration files to consult for full customization
- Server shutdown message

Fixes:

- Global toggles weren't being applied

Other Changes:

- Mention Mod Vault in readme

This version included a source code restructuring which should make it easier to maintain in the future.

## Version 0.10.1

Hotfix: Message toggles don't act independently.

This is fixed and you can have join messages disabled and death messages enabled and get death messages sent.

## Version 0.10.0

Features:

- %PLAYER_NAME% is replaced in messages with the player name, allowing you to change
  where in the message the player is mentioned (Thanks @Digitalroot)
- Configurable Ping and Shout messages

Fixes:

- More robust dedicated server detection (Thanks @Digitalroot)

## Version 0.9.1

Fixes

- Time interval for leader board in **minutes** not seconds.
- Don't display a leave message for disconnects due to version mismatch

## Version 0.9.0

Default config options are updated to be true for all notification and coordinates.

Features:

- Periodic stats leader board functionality (opt-in)

Fixes:

- Corrected duplicate "join" message when player dies
- Correctly looks at leave config option before sending leave message
- Correctly looks at join/death config option before sending messages

Improvements:

- Loaded config is now debug logged to make debugging easier

## Version 0.8.0

Added a Death detection and config options to enable/disable the messages as well as
set either a single message or list of messages to be chosen from when sending a message.

## Version 0.7.2, 0.7.3

Hotfix for mis-packed plugin

## Version 0.7.1

Added config option to ignore players when sending shout messages to Discord.

## Version 0.7.0

Fixes:

- properly check for dedicated vs non-dedicated servers

Features:

- when sending position (POS or coordinates) with the message, will use an embed
  to improve visibility (if enabled)
- added config options to enable/disable sending position with join and leave
- added config option to enable/disable using the embed with discord when sending
  position data (disabled by default, I find it very busy when enabled atm)
- added config option to enable/disable sending position with pings

## Version 0.6.0

Enabled for both the client and server versions of Valheim.

Key differences if running a server from the client:

- No Launch/Startup message is sent. This is because when the server launches it
  immediately begins loading the world, but for the client it is loading into the
  main menu. This may be fixable in the future to be a hook that goes before the
  world begins getting loaded to keep the functionality on the server and to enable
  similar functionality on the client.

## Version 0.5.1

Fixes:

- removed '$' before the position for ping messages

New this version:

- stats recording

Added a stat recording mechanism. This will record the player name and the trigger
(join, leave, shout, or ping) when a notification is generated for Discord. These
are stored in a 'records.json' file in the game server directory. The plan is to
(optionally) incorporate these stats into the messages sent to Discord. For example,
when you log in, it adds a little context: "John joined the game for the 1st time!" or
"Stuart arrives. Previous logins: 15". The context additions are not yet created but
record-keeping is ready and makes sense to get it started as soon as possible.

If you want to disable the record keeping in its entirety, set the Collect Player Stats
config value to false. This will prevent any records from being saved or written to disk.

## Version 0.5.0

Allows for randomized messages to get sent. If you want only one message to be sent
(the existing functionality 0.4.0 and earlier), you don't need to change anything,
and default configuration will only have one message for each notification. If you
would like to have a random message chosen each time, add multiple messages for each
config value and separate them with a semicolon ';'. Then, when Discord notifications
are sent, a random message will be sent from what you have provided.

New Features:

- Randomized messages amongst configured messages (separated with semicolon)

Breaking Changes:

- If you used a semicolon in your message, it will be seen as multiple messages

## Version 0.4.0

Features:

- Player leave messages

Thanks to a contribution from Digitalroot, player join and leave messages are now
implemented. You can modify what is announced when players join and leave or toggle
them on or off.

This removes the PlayerArrival settings.

## Version 0.3.0

Bug fixes:

When the server loaded it was sending the same message from the launch.

New Features:

Added 3 messages from hooking into the chat on the server. This includes:

- Players joining the server
- Shouting
- Pinging

All 3 are togglable and can have the position toggled separately.

To include when players leave, more work has to be done because those events
are not broadcast and instead it is only network messages.

## Version 0.2.0

- Use config values to set what messages get sent for what actions
- More granularity with Enable/Disable for existing messages

## Version 0.1.2

Added link to a how-to guide for creating a discord webhook.

## Version 0.1.1

Initial release. Configuration and sends messages on server startup and shutdown.
Essentially a minimally viable product.

- Configuration file with webhook and enable disable for each notification
- Ability to send messages to a Discord webhook
- Detection and message sent for:
  - server starting
  - server started
  - server stopping
