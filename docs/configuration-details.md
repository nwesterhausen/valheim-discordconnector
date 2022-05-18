# Configuration in Detail

The details on where configuration settings are and what they do: how to fine-tune your configuration.

DiscordConnector uses multiple configuration files to make find the setting you want to change faster, and hopefully easier. The configuration is divided into the following files:

| Configuration File                                   | Details                          | Purpose                                                                              |
| ---------------------------------------------------- | -------------------------------- | ------------------------------------------------------------------------------------ |
| `games.nwest.valheim.discordconnector.cfg`           | [Details](#main-config)          | Master settings, including the main webhook and turning settings on or off globally  |
| `games.nwest.valheim.discordconnector-messages.cfg`  | [Details](#messages)             | The messages used/chosen from when DiscordConnector sends messages to Discord        |
| `games.nwest.valheim.discordconnector-toggles.cfg`   | [Details](#toggles)              | Used to turn individual notifications and/or their included extra details on or off. |
| `games.nwest.valheim.discordconnector-variables.cfg` | [Details](#variable-definitions) | Used to assign strings to variables which can be referenced any messages             |

## Main Config

Filename `games.nwest.valheim.discordconnector.cfg`

| Option                                                  | Default | Description                                                                                                                                      |
| ------------------------------------------------------- | ------- | ------------------------------------------------------------------------------------------------------------------------------------------------ |
| Webhook URL                                             | (none)  | The main Discord webhook URL to send notifications/messages to.                                                                                  |
| Use fancier discord messages                            | false   | Set to true to enable using embeds in the Discord messages. If left false, all messages will remain plain strings (except for the leaderboard).  |
| Allow positions to be sent                              | true    | Set to false to prevent any positions/coordinates from being sent. If this is true, it can be overridden per message in the toggles config file. |
| Ignored players                                         | (none)  | List of playernames to never send a discord message for (they also won't be tracked in stats). This list should be semicolon (`;`) separated.    |
| Ignored players (Regex)                                 | (none)  | Regex which playernames are matched against to determine to not send a discord message for (they also won't be tracked in stats)                 |
| Collect stats                                           | true    | When this setting is enabled, DiscordConnector will record basic stats (leave, join, ping, shout, death) about players.                          |
| Send leaderboard updates                                | false   | If you set this to true, that will enable DiscordConnector to send a leaderboard for stats to Discord on the set interval                        |
| Leaderboard update interval                             | 600     | Time in minutes between each leaderboard update sent to Discord.                                                                                 |
| Announce Player Firsts                                  | true    | Disable this setting to disable all extra announcements the first time each player does something. (Overwrites any individual setting.)          |
| How many places to list in the top ranking leaderboards | 3       | Set how many places (1st, 2nd, 3rd by default) to display when sending the ranked leaderboard.                                                   |

!!! info "Stat Collection Details"

     Stat collection will create a file in the BepInEx config directory `games.nwest.valheim.discordconnector-records.db`, where it will record the number of times each player joins, leaves, dies, shouts or pings.
     If this is set to false, DiscordConnector will not keep a record of number of times each player does something it alerts to.
     If this is false, it takes precedent over the "Send leaderboard updates" setting and no leaderboards will get sent.

     The stat collection database uses the [LiteDB](https://www.litedb.org/) library and if you are so inclined they offer a database gui which you can use to view/modify this database. (Find the LiteDB Editor on their site.)

## Variable Definitions

Filename `games.nwest.valheim.discordconnector-variables.cfg`

You may assign strings to these variables to reference them in any messages.

| Option              | Default | Description                                                                        |
| ------------------- | ------- | ---------------------------------------------------------------------------------- |
| Defined Variable 1  | (none)  | This variable can be reference in any of the message content settings with %VAR1%  |
| Defined Variable 2  | (none)  | This variable can be reference in any of the message content settings with %VAR2%  |
| Defined Variable 3  | (none)  | This variable can be reference in any of the message content settings with %VAR3%  |
| Defined Variable 4  | (none)  | This variable can be reference in any of the message content settings with %VAR4%  |
| Defined Variable 5  | (none)  | This variable can be reference in any of the message content settings with %VAR5%  |
| Defined Variable 6  | (none)  | This variable can be reference in any of the message content settings with %VAR6%  |
| Defined Variable 7  | (none)  | This variable can be reference in any of the message content settings with %VAR7%  |
| Defined Variable 8  | (none)  | This variable can be reference in any of the message content settings with %VAR8%  |
| Defined Variable 9  | (none)  | This variable can be reference in any of the message content settings with %VAR9%  |
| Defined Variable 10 | (none)  | This variable can be reference in any of the message content settings with %VAR10% |

## Messages

Filename `games.nwest.valheim.discordconnector-messages.cfg`

All of the message options support having multiple messages defined in a semicolon (`;`) separated list. If you have multiple messages defined for these settings, one gets chosen at random when DiscordConnector decides to send the corresponding message.

If you wanted to have a couple different messages for when a player dies (always chosen at random), you could simply set the config value like this:

```
Player Death Message = %PLAYER_NAME% has died a beautiful death!;%PLAYER_NAME% went to their end with honor!;%PLAYER_NAME% died.
```

!!! Tip

    Any of the variables in [Variable Definitions](#variable-definitions) from the variables config file can be referenced in any message.

### Messages.Server

In the server messages, anywhere in the message you can use the special string `%PUBLICIP%` which will be replaced with the public IP address of the server when sent.

| Option                  | Default                  | Description                                                                                                 |
| ----------------------- | ------------------------ | ----------------------------------------------------------------------------------------------------------- |
| Server Launch Message   | `Server is starting up.` | The message defined here is sent when the server is starting up.                                            |
| Server Loaded Message   | `Server has started!`    | The message defined here is sent when the server has finished loading the map and is ready for connections. |
| Server Stop Message     | `Server is stopping.`    | The message defined here is sent when the server is shutting down.                                          |
| Server Shutdown Message | `Server has stopped.`    | The message defined here is sent when the server finishes shutting down.                                    |

### Messages.Players

In the player messages, anywhere in the message you put `%PLAYER_NAME%`, when the message is sent it will be replaced with that player's name. You can also use `%POS%` which will be replaced with their position if the position toggle is enabled.

!!! Tip

    When using the `%POS%` variable, in a Ping message it will reflect the location that was pinged, not the player's location. If you enabled the position toggle is enabled for these messages but you do not include the `%POS%` variable, the position will be appended to the message (the default behavior before the addition of `%POS%`).

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

### Messages.Events

In the event messages, anywhere in the message you can use the string vars `%EVENT_START_MSG%` and `%EVENT_END_MSG%` (or `%EVENT_MSG%` which is uses the start message when the event starts and the end message when the event ends.) You can also use `%POS%` which will be replaced with their position if the position toggle is enabled.

!!! Tip

    If you enabled the position toggle is enabled for these messages but you do not include the `%POS%` variable, the position will be appended to the message (the default behavior before the addition of `%POS%`).

!!! warning "`%PLAYERS%` variable removed in 1.4.0"

    Due to how the server keeps track of where players are (only if they are sharing location), the `%PLAYERS%` variable has been disabled indefinitely until a reliable way to gather player positions is developed.

| Option                | Default                                                                                          | Description                                                               |
| --------------------- | ------------------------------------------------------------------------------------------------ | ------------------------------------------------------------------------- |
| Event Start Message   | `**Event**: %EVENT_MSG%`                                                                         | Message sent when a random event starts.                                  |
| Event Stop Message    | `**Event**: %EVENT_MSG%`                                                                         | Message sent when a random event stops or ends.                           |
| Event Paused Message  | `**Event**: %EVENT_END_MSG% -- for now! (Currently paused due to no players in the event area.)` | Message sent when a random event is paused because players left the area. |
| Event Resumed Message | `**Event**: %EVENT_START_MSG%`                                                                   | Message sent when a random event resumes after being paused.              |

## Toggles

Filename `games.nwest.valheim.discordconnector-toggles.cfg`

The toggle configuration is a collection of on/off switches for all the message types and all the extra data that can be sent with them. It's broken up into 3 sections, "Toggles.Messages" which turns on or off each type of message, "Toggles.Positions" which turns on or off sending player coordinates with messages, "Toggles.Stats" which turns on or off collection of individual stats and "Toggles.Leaderboards" which turns on or off what stats to send with the leaderboard updates

### Toggles.Messages

| Option                      | Default | Description                                                                                |
| --------------------------- | ------- | ------------------------------------------------------------------------------------------ |
| Send Launch Messages        | true    | If true, a message will be sent to Discord when the server launches                        |
| Send Loaded Messages        | true    | If true, a message will be sent to Discord when the server is online and ready for players |
| Send Shutdown Messages      | true    | If true, a message will be sent to Discord when the server shuts down                      |
| Send Player Join Messages   | true    | Set to true to send a message when a player joins the world                                |
| Send Player Leave Messages  | true    | Set to true to send a message when a player leaves the world                               |
| Send Player Death Messages  | true    | Set to true to send a message when a player dies                                           |
| Send Player Shout Messages  | true    | Set to true to send a message when a player shouts                                         |
| Send Player Ping Messages   | true    | Set to true to send a message when a player pings the map                                  |
| Event Start Notifications   | true    | Set to true to send a message when and event starts                                        |
| Event Stop Notifications    | true    | Set to true to send a message when and event stops                                         |
| Event Paused Notifications  | true    | Set to true to send a message when and event is paused                                     |
| Event Resumed Notifications | true    | Set to true to send a message when and event is resumed                                    |

### Toggles.Position

| Option                                        | Default | Description                                                          |
| --------------------------------------------- | ------- | -------------------------------------------------------------------- |
| Send Position with Player Joins               | false   | Set to true to send a player's coordinates when they join the world  |
| Send Position with Player Leaves              | false   | Set to true to send a player's coordinates when they leave the world |
| Send Position with Player Pings               | true    | Set to true to send a player's coordinates when they ping on the map |
| Send Position with Player Shouts              | false   | Set to true to send a player's coordinates when they shout in game   |
| Send Position with Player Deaths              | true    | Set to true to send a player's coordinates when they die             |
| Event Start Messages Position Notifications   | true    | Set to true to send the event coordinates when the event starts      |
| Event Stop Messages Position Notifications    | true    | Set to true to send the event coordinates when the event stops       |
| Event Paused Messages Position Notifications  | true    | Set to true to send the event coordinates when the event is paused   |
| Event Resumed Messages Position Notifications | true    | Set to true to send the event coordinates when the event is resumed  |

### Toggles.Stats

| Option                        | Default | Description                                                  |
| ----------------------------- | ------- | ------------------------------------------------------------ |
| Allow recording player joins  | true    | Set to false to never record players joining in records.json |
| Allow recording player leaves | true    | Set to false to never record players leaving in records.json |
| Allow recording player pings  | true    | Set to false to never record player pings in records.json    |
| Allow recording player shouts | true    | Set to false to never record player shouts in records.json   |
| Allow recording player deaths | true    | Set to false to never record player deaths in records.json   |

### Toggles.Leaderboard

!!! Info
All leaderboard toggles are restricted by the `Send leaderboard updates` toggle in the [Main config](#main-config).

For the ranked leadboards, you choose how many ranks to calculate and display with the `How many places to list in the top ranking leaderboards` setting in the [Main Config](#main-config).

| Option                                        | Default | Description                                                                     |
| --------------------------------------------- | ------- | ------------------------------------------------------------------------------- |
| Send Periodic Leaderboard for Player Deaths   | true    | If enabled, will send a ranked leaderboard for player deaths at the interval.   |
| Send Periodic Leaderboard for Player Pings    | false   | If enabled, will send a ranked leaderboard for player pings at the interval.    |
| Send Periodic Leaderboard for Player Sessions | false   | If enabled, will send a ranked leaderboard for player sessions at the interval. |
| Send Periodic Leaderboard for Player Shouts   | false   | If enabled, will send a ranked leaderboard for player shouts at the interval.   |

### Toggles.Leaderboard.Lowest

| Option                                         | Default | Description                                                              |
| ---------------------------------------------- | ------- | ------------------------------------------------------------------------ |
| Include Least Deaths in Periodic Leaderboard   | true    | If enabled, will include player with the least deaths at the interval.   |
| Include Least Pings in Periodic Leaderboard    | false   | If enabled, will include player with the least pings at the interval.    |
| Include Least Sessions in Periodic Leaderboard | false   | If enabled, will include player with the least sessions at the interval. |
| Include Least Shouts in Periodic Leaderboard   | false   | If enabled, will include player with the least shouts at the interval.   |

### Toggles.Leaderboard.Highest

| Option                                        | Default | Description                                                             |
| --------------------------------------------- | ------- | ----------------------------------------------------------------------- |
| Include Most Deaths in Periodic Leaderboard   | true    | If enabled, will include player with the most deaths at the interval.   |
| Include Most Pings in Periodic Leaderboard    | false   | If enabled, will include player with the most pings at the interval.    |
| Include Most Sessions in Periodic Leaderboard | false   | If enabled, will include player with the most sessions at the interval. |
| Include Most Shouts in Periodic Leaderboard   | false   | If enabled, will include player with the most shouts at the interval.   |

### Toggles.PlayerFirsts

| Option                     | Default | Description                                                                                                                   |
| -------------------------- | ------- | ----------------------------------------------------------------------------------------------------------------------------- |
| Send Player Join Messages  | true    | If enabled (and player-first anouncements are enabled), will send an extra message on a player's first leave from the server. |
| Send Player Leave Messages | false   | If enabled (and player-first anouncements are enabled), will send an extra message on a player's first join to the server.    |
| Send Player Death Messages | true    | If enabled (and player-first anouncements are enabled), will send an extra message on a player's first death."                |
| Send Player Shout Messages | false   | If enabled (and player-first anouncements are enabled), will send an extra message on a player's first ping.                  |
| Send Player Ping Messages  | false   | If enabled (and player-first anouncements are enabled), will send an extra message on a player's first shout.                 |
