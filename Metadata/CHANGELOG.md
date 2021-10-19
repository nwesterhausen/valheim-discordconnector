# Changelog

A full changelog for reference.

### Version 1.2.2

Fixes:

- No shutdown message when some other mods are loaded (Like World of Valheim suite)

Also this update modifies when the startup, shutting down, and shut down messages are sent. There now will likely be a bit
of a pause because the startup message gets sent when the game is initialized instead of when the loading of the map starts
for the server.

### Version 1.2.1

Fixes:

- The leaderboard toggles were not working properly, behind the scenes they were all following the death leaderboard toggle

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

### Version 1.2.0

Features:

- `%PUBLICIP%` message variable available in the server messages

  There is no variable for what port the game is running on since I figured that had to be set manually in the first place (if not default),
  and you should be able to modify the message to be something like `Join the server on %PUBLICIP%:2457` or similar if you want to.

- Messages for events start/pause/stopping

  A feature that I wanted finally added. This was difficult to test in a server environment and I did the testing running on the client and then
  the client running as a server. In those instances I verified that the messages were fired when events started, ended, paused or resumed. The
  resume message is the same as the start message by default because I couldn't think of a way to word it that made sense.

### Version 1.1.1

Fix:

- Stop and Loaded config values were using the same value as launched on the backend and not respecting the actual config.

### Version 1.1.0

Features:

- Send a message when the server saves the world

Fixes:

- Configuration file comments should be clearer/easier to understand

### Version 1.0.0

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

### Version 0.10.1

Hotfix: Message toggles don't act independently.

This is fixed and you can have join messages disabled and death messages enabled and get death messages sent.

### Version 0.10.0

Features:

- %PLAYER_NAME% is replaced in messages with the player name, allowing you to change
where in the message the playe is mentioned (Thanks @Digitalroot)
- Configurable Ping and Shout messages

Fixes:

- More robust dedicated server detection (Thanks @Digitalroot)

### Version 0.9.1

Fixes

- Time interval for leaderboard in **minutes** not seconds.
- Don't display a leave message for disconnects due to version mismatch

### Version 0.9.0

Default config options are updated to be true for all notification and coordinates.

Features:

- Periodic stats leaderboard functionality (opt-in)

Fixes:

- Corrected duplicate "join" message when player dies
- Correctly looks at leave config option before sending leave message
- Correctly looks at join/death config option before sending messages

Improvements:

- Loaded config is now debug logged to make debugging easier

### Version 0.8.0

Added a Death detection and config options to enable/disable the messages as well as
set either a single message or list of messages to be chosen from when sending a message.

### Version 0.7.2, 0.7.3

Hotfix for mis-packed plugin

### Version 0.7.1

Added config option to ignore players when sending shout messages to Discord.

### Version 0.7.0

Fixes:

- properly check for dedicated vs non-dedicated servers

Features:

- when sending position (POS or coordinates) with the message, will use an embed
to improve visibility (if enabled)
- added config options to enable/disable sending position with join and leave
- added config option to enable/disable using the embed with discord when sending
position data (disabled by default, I find it very busy when enabled atm)
- added config option to enable/disable sending position with pings

### Version 0.6.0

Enabled for both the client and server versions of Valheim.

Key differences if running a server from the client:

- No Launch/Startup message is sent. This is because when the server launches it
immediately begins loading the world, but for the client it is loading into the
main menu. This may be fixable in the future to be a hook that goes before the
world begins getting loaded to keep the functionality on the server and to enable
similar functionality on the client.

### Version 0.5.1

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

If you want to disable the record keeping in its entirity, set the Collect Player Stats
config value to false. This will prevent any records from being saved or written to disk.

### Version 0.5.0

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

### Version 0.4.0

Features:

- Player leave messages

Thanks to a contribution from Digitalroot, player join and leave messages are now
implemented. You can modify what is announced when players join and leave or toggle
them on or off.

This removes the PlayerArrival settings.

### Version 0.3.0

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

### Version 0.2.0

- Use config values to set what messages get sent for what actions
- More granualarity with Enable/Disable for existing messages

### Version 0.1.2

Added link to a how-to guide for creating a discord webhook.

### Version 0.1.1

Initial release. Configuration and sends messages on server startup and shutdown.
Essentially a minimally viable product.

- Configuration file with webhook and enable disable for each notification
- Ability to send messages to a Discord webhook
- Detection and message sent for:
  - server starting
  - server started
  - server stopping
