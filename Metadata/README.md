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

Full changelog history available on the
[Github repository](https://github.com/nwesterhausen/valheim-discordconnector/blob/main/Metadata/CHANGELOG.md).

**Release 1.0.0+ is a breaking release** since the structure of the configuration files completely changes. When you update you will need to modify the config
to save your webhook again and to update any message customization you have done!

**Release 1.2.0 affected the records.json file** so if you update and notice that your recorded stats aren't changing, it's a simple fix.

records.json pre 1.2.0:

```json
[{"Category":"death","Values":[{"PlayerName":"Xithyr","Value":13} ...
```

records.json 1.2.0+ (PlayerName changed to Key)

```json
[{"Category":"death","Values":[{"Key":"Xithyr","Value":13} ...
```

### Version 1.3.0

Features:

- Additional leaderboard options. The existing leaerboard option will now default to sending top 3 players for what is enabled. 
You can enable a highest and lowest leaderboard for each tracked stat now. All leaderboards get sent on the same interval.