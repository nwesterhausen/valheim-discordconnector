# Discord Connector

Connect your Valheim server to a Discord Webhook. ([How to get a webhook](https://github.com/nwesterhausen/valheim-discordconnector/blob/main/Metadata/HowtoGuide.md))

## Features

- Set your own webhook, lets you configure icon, title, and a target channel
- Enable or Disable any messages
- Set what text gets sent for most messages

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

### Version 0.4.0

Features:

- Player leave messages

Thanks to a contribution from Digitalroot, player join and leave messages are now
implemented. You can modify what is announced when players join and leave or toggle
them on or off.

This removes the PlayerArrival settings.
