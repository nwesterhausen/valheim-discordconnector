# Discord Connector

Connect your Valheim server (dedicated or served from the game itself) to a Discord Webhook. 
([How to get a webhook](https://github.com/nwesterhausen/valheim-discordconnector/blob/main/Metadata/HowtoGuide.md#how-to-get-a-discord-webhook) or [Short FAQ Guide for this mod](https://github.com/nwesterhausen/valheim-discordconnector/blob/main/Metadata/HowtoGuide.md)) There's also [configuration documentation](https://github.com/nwesterhausen/valheim-discordconnector/blob/main/Metadata/ConfigurationDetails.md).

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

- Fancier Discord messages
- Discord bot integration

## Changelog

Full changelog history available on the
[Github repository](https://github.com/nwesterhausen/valheim-discordconnector/blob/main/Metadata/CHANGELOG.md).

**Release 1.0.0+ is a breaking release** since the structure of the configuration files completely changes. When you update you will need to modify the config
to save your webhook again and to update any message customization you have done!

### Version 1.2.0

Features:

- `%PUBLICIP%` message variable available in the server messages

  There is no variable for what port the game is running on since I figured that had to be set manually in the first place (if not default),
  and you should be able to modify the message to be something like `Join the server on %PUBLICIP%:2457` or similar if you want to.

- Messages for events start/pause/stopping

  A feature that I wanted finally added. This was difficult to test in a server environment and I did the testing running on the client and then
  the client running as a server. In those instances I verified that the messages were fired when events started, ended, paused or resumed. The
  resume message is the same as the start message by default because I couldn't think of a way to word it that made sense.
