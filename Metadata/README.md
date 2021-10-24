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

### Version 1.4.2

Fixes:

- Least deaths leaderboard wasn't respecting the correct config entry. (THanks @thedefside)

### Version 1.4.1

Fixes:

- Removed the two debug logging calls for events -- sorry for the log spam!

### Version 1.4.0

Features:

- 10 user defined variables that can be used an any messages (%VAR1% thru %VAR10%). These are set in their own configuration file, 
`games.nwest.valheim.discordconnector-variables.cfg` which will get generated first time 1.4.0 is run.
- The position of where the player/ping/event coordinates are inserted into messages is configurable using the `%POS%` variable in
the messages config. It won't be replaced if the "send coordinates" toggle is off for that message. If you don't include a `%POS%`
variable, it will append the coordinates as happens with previous versions.

Fixes:

- Fixed an off-by-one error in the Top Players leaderboard (the default leaderboard) (Thanks @thedefside)
- Fixed configuration not referencing proper settings (Thanks @thedefside)
- Fixed event messages (now properly functioning on dedicated servers)

Breaking Changes:

- If you used `%PLAYERS%` in any of the event messages, you need to remove it. With the changes required for the event messages
functionality, it is not supportable at this time.

Full changelog history available on the
[Github repository](https://github.com/nwesterhausen/valheim-discordconnector/blob/main/Metadata/CHANGELOG.md)
or [discordconnector.valheim.nwest.games](https://discordconnector.valheim.nwest.games/changelog).