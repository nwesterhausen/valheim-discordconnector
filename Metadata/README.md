# Discord Connector

Connect your Valheim server to a Discord Webhook. ([How to get a webhook](https://github.com/nwesterhausen/valheim-discordconnector/blob/main/Metadata/HowtoGuide.md))

## Features

- Set your own webhook, lets you configure icon, title, and a target channel
- Enable or Disable any messages

### Supported Message Notificaitons

- Server startup (server starting, loading the world)
- Server started (world loaded, ready to join)
- Server shutting down (server stopping)

### Roadmap

- Messages on player join/leave
- Messages on player ping
- Message on player shouting
- Message when events start/end

## Changelog

### Version 0.1.1

Initial release. Configuration and sends messages on server startup and shutdown.
Essentially a minimally viable product.

- Configuration file with webhook and enable disable for each notification
- Ability to send messages to a Discord webhook
- Detection and message sent for:
    - server starting
    - server started
    - server stopping