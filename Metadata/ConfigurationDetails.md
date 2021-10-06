# Configuration Details

The details on where configuration settings are and what they do: how to fine-tune your configuration.

DiscordConnector uses multiple configuration files to make find the setting you want to change faster, and hopefully easier. The configuration is divided into the following files:

| Configuration File                                    | Details                 | Purpose                                                                                                            |
| ----------------------------------------------------- | ----------------------- | ------------------------------------------------------------------------------------------------------------------ |
| `games.nwest.valheim.discordconnector.cfg`            | [Details](#main-config) | Master settings, including the main webhook and turning settings on or off globally                                |
| `games.nwest.valheim.discordconnector-messages.cfg`   | [Details](#messages)    | The messages used/chosen from when DiscordConnector sends messages to Discord                                      |
| `games.nwest.valheim.discordconnector-toggles.cfg`    | [Details](#toggles)     | Used to turn individual notifications and/or their included extra details on or off.                               |
| `games.nwest.valheim.discordconnecotr-moderation.cfg` | [Details](#moderation)  | Settings to help enable moderation are here, such as secondary webhooks and their settings. (Not yet implemented!) |

## Main Config

| Option                       | Default | Description                                                                                                                                                                                                                                                                                                                                                                                                                                              |
| ---------------------------- | ------- | -------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| Webhook URL                  | (none)  | The main Discord webhook URL to send notifications/messages to.                                                                                                                                                                                                                                                                                                                                                                                          |
| Use fancier discord messages | false   | Set to true to enable using embeds in the Discord messages. If left false, all messages will remain plain strings (except for the leaderboard).                                                                                                                                                                                                                                                                                                          |
| Allow positions to be sent   | true    | Set to false to prevent any positions/coordinates from being sent, even if you have enabled some in toggles. If this is true, it can be overridden per message in the toggles config file.                                                                                                                                                                                                                                                               |
| Ignored players              | (none)  | List of playernames to never send a discord message for (they also won't be tracked in stats). This list should be semicolon (`;`) separated.                                                                                                                                                                                                                                                                                                            |
| Collect stats                | true    | When this setting is enabled, DiscordConnector will create a file in the game root directory "records.json" where it will record the number of times each player joins, leaves, dies, shouts or pings. If this is set to false, DiscordConnector will not keep a record of number of times each player does something it alerts to. If this is false, it takes precendent over the "Send leaderboard updates" setting and no leaderboards will get sent. |
| Send leaderboard updates     | false   | If you set this to true, that will enable DiscordConnector to send a leaderboard for stats to Discord on the set interval                                                                                                                                                                                                                                                                                                                                |
| Leaderboard update interval  | 600     | Time in minutes between each leaderboard update sent to Discord.                                                                                                                                                                                                                                                                                                                                                                                         |

## Messages

All of the message options support having multiple messages defined in a semicolon (`;`) separated list. If you have multiple messages defined for these settings, one gets chosen at random when DiscordConnector decides to send the corresponding message.

If you wanted to have a couple different messages for when a player dies (always chosen at random), you could simply set the config value like this:

```
Player Death Message = has died a beautiful death!;went to their end with honor!;died.
```

As of version 0.10.0, all the player messages are constructed like this:

```
{{player name}} {{chosen message from config}}
```

So keep that in mind as your write messages. In a future update, this will likely change to allow you to specify in the config itself where to inject the player's name.

| Option                | Default                | Description                                                                                                 |
| --------------------- | ---------------------- | ----------------------------------------------------------------------------------------------------------- |
| Server Launch Message | Server is starting up. | The message defined here is sent when the server is starting up.                                            |
| Server Loaded Message | Server has started!    | The message defined here is sent when the server has finished loading the map and is ready for connections. |
| Server Stop Message   | Server is stopping.    | The message defined here is sent when the server is shutting down.                                          |
| Player Join Message   | has joined.            | The message that will be sent when a player joins the server.                                               |
| Player Leave Message  | has left.              | The message that will be sent when a player leaves the server.                                              |
| Player Death Message  | has died.              | The message that will be sent when a player dies..                                                          |

## Toggles

The toggle configuration is a collection of on/off switches for all the message types and all the extra data that can be sent with them. It's broken up into 3 sections, "Toggles.Messages" which turns on or off each type of message, "Toggles.Positions" which turns on or off sending player coordinates with messages, "Toggles.Stats" which turns on or off collection of individual stats and "Toggles.Leaderboards" which turns on or off what stats to send with the leaderboard updates

### Message.Toggles

| Option                     | Default | Description                                                  |
| -------------------------- | ------- | ------------------------------------------------------------ |
| Send Launch Messages       | true    | If true, a message will be sent to Discord when the server launches |
| Send Loaded Messages       | true    | If true, a message will be sent to Discord when the server is online and ready for players |
| Send Shutdown Messages     | true    | If true, a message will be sent to Discord when the server shuts down |
| Send Player Join Messages  | true    | Set to true to send a message when a player joins the world  |
| Send Player Leave Messages | true    | Set to true to send a message when a player leaves the world |
| Send Player Death Messages | true    | Set to true to send a message when a player dies             |
| Send Player Shout Messages | true    | Set to true to send a message when a player shouts           |
| Send Player Ping Messages  | true    | Set to true to send a message when a player pings the map    |

### Position.Toggles

| Option                           | Default | Description                                                  |
| -------------------------------- | ------- | ------------------------------------------------------------ |
| Send Position with Player Joins  | false   | Set to true to send a player's coordinates when they join the world |
| Send Position with Player Leaves | false   | Set to true to send a player's coordinates when they leave the world |
| Send Position with Player Pings  | true    | Set to true to send a player's coordinates when theyping on the map |
| Send Position with Player Shouts | false   | Set to true to send a player's coordinates when they shout in game |
| Send Position with Player Deaths | true    | Set to true to send a player's coordinates when they die     |

### Stats.Toggles

| Option                        | Default | Description                                                  |
| ----------------------------- | ------- | ------------------------------------------------------------ |
| Allow recording player joins  | true    | Set to false to never record players joining in records.json |
| Allow recording player leaves | true    | Set to false to never record players leaving in records.json |
| Allow recording player pings  | true    | Set to false to never record player pings in records.json    |
| Allow recording player shouts | true    | Set to false to never record player shouts in records.json   |
| Allow recording player deaths | true    | Set to false to never record player deaths in records.json   |

### Leaderboard.Toggles

| Option                    | Default | Description                                                  |
| ------------------------- | ------- | ------------------------------------------------------------ |
| Send pings leaderboard    | false   | Send a leaderboard (at the interval) for top-pinging players |
| Send deaths leaderboard   | true    | Send a leaderboard (at the interval) for what players have the most deaths |
| Send sessions leaderboard | false   | Send a leaderboard (at the interval) for players with the most joins/leaves |
| Send shouts leaderboard   | false   | Send a leaderboard (at the interval) for players with the most shouts sent |
