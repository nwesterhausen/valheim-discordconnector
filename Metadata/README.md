# Discord Connector

Connect your Valheim server to a Discord Webhook. ([How to get a webhook](https://github.com/nwesterhausen/valheim-discordconnector/blob/main/Metadata/HowtoGuide.md#how-to-get-a-discord-webhook) or [Short FAQ Guide for this mode](https://github.com/nwesterhausen/valheim-discordconnector/blob/main/Metadata/HowtoGuide.md))

## Features

- Set your own webhook, lets you configure icon, title, and a target channel
- Enable or Disable any messages
- Set what text gets sent for most messages
- Set more than one message for each type and have one randomly chosen!
- Record number of logins/deaths/pings and flavor the Discord messages

### Supported Message Notificaitons

- Server startup (server starting, loading the world)
- Server started (world loaded, ready to join)
- Server shutting down (server stopping)
- Player join
- Player leave
- Player shouting
- Player pinging

### Roadmap

- Message when events start/end
- Player death
- Fancier Discord messages

## Changelog

Full changelog history available on the
[Github repository](https://github.com/nwesterhausen/valheim-discordconnector/blob/main/Metadata/CHANGELOG.md).

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
