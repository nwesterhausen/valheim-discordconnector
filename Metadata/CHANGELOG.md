# Changelog

A full changelog for reference.

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
