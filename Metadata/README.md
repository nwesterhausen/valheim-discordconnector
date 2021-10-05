# Discord Connector

Connect your Valheim server to a Discord Webhook. ([How to get a webhook](https://github.com/nwesterhausen/valheim-discordconnector/blob/main/Metadata/HowtoGuide.md#how-to-get-a-discord-webhook) or [Short FAQ Guide for this mode](https://github.com/nwesterhausen/valheim-discordconnector/blob/main/Metadata/HowtoGuide.md))

## Features

- Set your own webhook, lets you configure icon, title, and a target channel
- Enable or Disable any messages
- Set what text gets sent for most messages
- Set more than one message for each type and have one randomly chosen!

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

## Changelog

Full changelog history available on the
[Github repository](https://github.com/nwesterhausen/valheim-discordconnector/blob/main/Metadata/CHANGELOG.md).

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
