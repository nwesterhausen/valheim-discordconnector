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
| Announce Player Firsts       | true    | Disable this setting to disable all extra announcements the first time each player does something. (Overwrites any individual setting.)                                                                                                                                                                                                                                                                                                                  |

## Messages

All of the message options support having multiple messages defined in a semicolon (`;`) separated list. If you have multiple messages defined for these settings, one gets chosen at random when DiscordConnector decides to send the corresponding message.

If you wanted to have a couple different messages for when a player dies (always chosen at random), you could simply set the config value like this:

```
Player Death Message = %PLAYER_NAME% has died a beautiful death!;%PLAYER_NAME% went to their end with honor!;%PLAYER_NAME% died.
```

### Messages.Server

| Option                  | Default                  | Description                                                                                                 |
| ----------------------- | ------------------------ | ----------------------------------------------------------------------------------------------------------- |
| Server Launch Message   | `Server is starting up.` | The message defined here is sent when the server is starting up.                                            |
| Server Loaded Message   | `Server has started!`    | The message defined here is sent when the server has finished loading the map and is ready for connections. |
| Server Stop Message     | `Server is stopping.`    | The message defined here is sent when the server is shutting down.                                          |
| Server Shutdown Message | `Server has stopped.`    | The message defined here is sent when the server finishes shutting down.                                    |

### Messages.Players

In the player messages, anywhere in the message you put `%PLAYER_NAME%`, when the message is sent it will be replaced with that player's name.

| Option               | Default                            | Description                                                                                                                                  |
| -------------------- | ---------------------------------- | -------------------------------------------------------------------------------------------------------------------------------------------- |
| Player Join Message  | `%PLAYER_NAME% has joined.`        | The message that will be sent when a player joins the server.                                                                                |
| Player Leave Message | `%PLAYER_NAME% has left.`          | The message that will be sent when a player leaves the server.                                                                               |
| Player Death Message | `%PLAYER_NAME% has died.`          | The message that will be sent when a player dies..                                                                                           |
| Player Ping Message  | `%PLAYER_NAME% pings the map`      | The message that will be sent when a player pings the map.                                                                                   |
| Player Shout Message | `%PLAYER_NAME% shout **%SHOUT%**.` | The message that will be sent when a player shouts in game. %SHOUT% must be somewhere in this message for what the player shouts to be sent. |

### Messages.PlayerFirsts

In the player messages, anywhere in the message you put `%PLAYER_NAME%`, when the message is sent it will be replaced with that player's name.

| Option                     | Default                                                       | Description                                                              |
| -------------------------- | ------------------------------------------------------------- | ------------------------------------------------------------------------ |
| Player First Join Message  | `Welcome %PLAYER_NAME%, it's their first time on the server!` | The message that will be sent the first time a player joins the server.  |
| Player First Leave Message | `%PLAYER_NAME% has left for the first time.`                  | The message that will be sent the first time a player leaves the server. |
| Player First Death Message | `%PLAYER_NAME% has died for the first time.`                  | The message that will be sent the first time a player dies..             |
| Player First Ping Message  | `%PLAYER_NAME% pings the map for the first time`              | The message that will be sent the first time a player pings the map.     |
| Player First Shout Message | `%PLAYER_NAME% shouts for the first time.`                    | The message that will be sent the first time a player shouts in game.    |

## Toggles

The toggle configuration is a collection of on/off switches for all the message types and all the extra data that can be sent with them. It's broken up into 3 sections, "Toggles.Messages" which turns on or off each type of message, "Toggles.Positions" which turns on or off sending player coordinates with messages, "Toggles.Stats" which turns on or off collection of individual stats and "Toggles.Leaderboards" which turns on or off what stats to send with the leaderboard updates

### Toggles.Messages

| Option                     | Default | Description                                                                                |
| -------------------------- | ------- | ------------------------------------------------------------------------------------------ |
| Send Launch Messages       | true    | If true, a message will be sent to Discord when the server launches                        |
| Send Loaded Messages       | true    | If true, a message will be sent to Discord when the server is online and ready for players |
| Send Shutdown Messages     | true    | If true, a message will be sent to Discord when the server shuts down                      |
| Send Player Join Messages  | true    | Set to true to send a message when a player joins the world                                |
| Send Player Leave Messages | true    | Set to true to send a message when a player leaves the world                               |
| Send Player Death Messages | true    | Set to true to send a message when a player dies                                           |
| Send Player Shout Messages | true    | Set to true to send a message when a player shouts                                         |
| Send Player Ping Messages  | true    | Set to true to send a message when a player pings the map                                  |

### Toggles.Position

| Option                           | Default | Description                                                          |
| -------------------------------- | ------- | -------------------------------------------------------------------- |
| Send Position with Player Joins  | false   | Set to true to send a player's coordinates when they join the world  |
| Send Position with Player Leaves | false   | Set to true to send a player's coordinates when they leave the world |
| Send Position with Player Pings  | true    | Set to true to send a player's coordinates when theyping on the map  |
| Send Position with Player Shouts | false   | Set to true to send a player's coordinates when they shout in game   |
| Send Position with Player Deaths | true    | Set to true to send a player's coordinates when they die             |

### Toggles.Stats

| Option                        | Default | Description                                                  |
| ----------------------------- | ------- | ------------------------------------------------------------ |
| Allow recording player joins  | true    | Set to false to never record players joining in records.json |
| Allow recording player leaves | true    | Set to false to never record players leaving in records.json |
| Allow recording player pings  | true    | Set to false to never record player pings in records.json    |
| Allow recording player shouts | true    | Set to false to never record player shouts in records.json   |
| Allow recording player deaths | true    | Set to false to never record player deaths in records.json   |

### Toggles.Leaderboard

| Option                    | Default | Description                                                                                             |
| ------------------------- | ------- | ------------------------------------------------------------------------------------------------------- |
| Send pings leaderboard    | false   | If enabled (and leaderboards are enabled), will send a leaderboard for player pings at the interval.    |
| Send deaths leaderboard   | true    | If enabled (and leaderboards are enabled), will send a leaderboard for player deaths at the interval.   |
| Send sessions leaderboard | false   | If enabled (and leaderboards are enabled), will send a leaderboard for player sessions at the interval. |
| Send shouts leaderboard   | false   | If enabled (and leaderboards are enabled), will send a leaderboard for player shouts at the interval.   |

### Toggles.PlayerFirsts

| Option                     | Default | Description                                                                                                                   |
| -------------------------- | ------- | ----------------------------------------------------------------------------------------------------------------------------- |
| Send Player Join Messages  | true    | If enabled (and player-first anouncements are enabled), will send an extra message on a player's first leave from the server. |
| Send Player Leave Messages | false   | If enabled (and player-first anouncements are enabled), will send an extra message on a player's first join to the server.    |
| Send Player Death Messages | true    | If enabled (and player-first anouncements are enabled), will send an extra message on a player's first death."                |
| Send Player Shout Messages | false   | If enabled (and player-first anouncements are enabled), will send an extra message on a player's first ping.                  |
| Send Player Ping Messages  | false   | If enabled (and player-first anouncements are enabled), will send an extra message on a player's first shout.                 |