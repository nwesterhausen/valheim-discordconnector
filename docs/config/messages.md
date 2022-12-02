# Messages

Filename `discordconnector-messages.cfg`

All of the message options support having multiple messages defined in a semicolon (`;`) separated list. If you have multiple messages defined for these settings, one gets chosen at random when DiscordConnector decides to send the corresponding message.

If you wanted to have a couple different messages for when a player dies (always chosen at random), you could simply set the config value like this:

```toml
Player Death Message = %PLAYER_NAME% has died a beautiful death!;%PLAYER_NAME% went to their end with honor!;%PLAYER_NAME% died.
```

::: tip
Any of the variables in [Variable Definitions](/config/variables) from the variables config file can be referenced in any message.
:::

## Messages.Server

| Option                  | Default                  | Description                                                                                                 |
| ----------------------- | ------------------------ | ----------------------------------------------------------------------------------------------------------- |
| Server Launch Message   | `Server is starting up.` | The message defined here is sent when the server is starting up.                                            |
| Server Loaded Message   | `Server has started!`    | The message defined here is sent when the server has finished loading the map and is ready for connections. |
| Server Stop Message     | `Server is stopping.`    | The message defined here is sent when the server is shutting down.                                          |
| Server Shutdown Message | `Server has stopped.`    | The message defined here is sent when the server finishes shutting down.                                    |

### Available Predefined Variables (Server)

| Variable     | Replaced with..                                                        | Can be used in..    |
| ------------ | ---------------------------------------------------------------------- | ------------------- |
| `%PUBLICIP%` | Server's public IP (obtained from [ifconfig.me](https://ifconfig.me/)) | Any server messages |

## Messages.Players

| Option               | Default                            | Description                                                                                                                                  |
| -------------------- | ---------------------------------- | -------------------------------------------------------------------------------------------------------------------------------------------- |
| Player Join Message  | `%PLAYER_NAME% has joined.`        | The message that will be sent when a player joins the server.                                                                                |
| Player Leave Message | `%PLAYER_NAME% has left.`          | The message that will be sent when a player leaves the server.                                                                               |
| Player Death Message | `%PLAYER_NAME% has died.`          | The message that will be sent when a player dies..                                                                                           |
| Player Ping Message  | `%PLAYER_NAME% pings the map`      | The message that will be sent when a player pings the map.                                                                                   |
| Player Shout Message | `%PLAYER_NAME% shout **%SHOUT%**.` | The message that will be sent when a player shouts in game. %SHOUT% must be somewhere in this message for what the player shouts to be sent. |

### Available Predefined Variables (Players)

| Variable           | Replaced with..              | Can be used in..                             |
| ------------------ | ---------------------------- | -------------------------------------------- |
| `%PLAYER_NAME%`    | Player's character name      | Player join/leave/shout/ping/death messages. |
| `%PLAYER_STEAMID%` | Player's Platform ID         | Player join/leave/shout/ping/death messages. |
| `%PLAYER_ID%`      | Player's Platform ID         | Player join/leave/shout/ping/death messages. |
| `%SHOUT%`          | Text of the player's shout   | Player shout messages.                       |
| `%POS%`            | Player's coordinate position | Player join/leave/shout/ping/death messages. |

::: tip
When using the `%POS%` variable, in a Ping message it will reflect the location that was pinged, not the player's location. If you enabled the position toggle is enabled for these messages but you do not include the `%POS%` variable, the position will be appended to the message (the default behavior before the addition of `%POS%`).
:::

## Messages.PlayerFirsts

In the player messages, anywhere in the message you put `%PLAYER_NAME%`, when the message is sent it will be replaced with that player's name.

| Option                     | Default                                                       | Description                                                              |
| -------------------------- | ------------------------------------------------------------- | ------------------------------------------------------------------------ |
| Player First Join Message  | `Welcome %PLAYER_NAME%, it's their first time on the server!` | The message that will be sent the first time a player joins the server.  |
| Player First Leave Message | `%PLAYER_NAME% has left for the first time.`                  | The message that will be sent the first time a player leaves the server. |
| Player First Death Message | `%PLAYER_NAME% has died for the first time.`                  | The message that will be sent the first time a player dies..             |
| Player First Ping Message  | `%PLAYER_NAME% pings the map for the first time`              | The message that will be sent the first time a player pings the map.     |
| Player First Shout Message | `%PLAYER_NAME% shouts for the first time.`                    | The message that will be sent the first time a player shouts in game.    |

## Messages.Events

| Option                | Default                                                                                          | Description                                                               |
| --------------------- | ------------------------------------------------------------------------------------------------ | ------------------------------------------------------------------------- |
| Event Start Message   | `**Event**: %EVENT_MSG%`                                                                         | Message sent when a random event starts.                                  |
| Event Stop Message    | `**Event**: %EVENT_MSG%`                                                                         | Message sent when a random event stops or ends.                           |
| Event Paused Message  | `**Event**: %EVENT_END_MSG% -- for now! (Currently paused due to no players in the event area.)` | Message sent when a random event is paused because players left the area. |
| Event Resumed Message | `**Event**: %EVENT_START_MSG%`                                                                   | Message sent when a random event resumes after being paused.              |

### Available Predefined Variables (Events)

In the event messages, anywhere in the message you can use the string vars `%EVENT_START_MSG%` and `%EVENT_END_MSG%` (or `%EVENT_MSG%` which is uses the start message when the event starts and the end message when the event ends.) You can also use `%POS%` which will be replaced with their position if the position toggle is enabled.

| Variable            | Replaced with..                                          | Can be used in..         |
| ------------------- | -------------------------------------------------------- | ------------------------ |
| `%EVENT_START_MSG%` | The event start message (e.g. "The forest is moving...") | Event start message      |
| `%EVENT_END_MSG%`   | The event stop message (e.g. "The forest rests again")   | Event stop message       |
| `%EVENT_MSG%`       | The appropriate start/end message for the event          | Event start/stop message |

::: tip
If you enabled the position toggle is enabled for these messages but you do not include the `%POS%` variable, the position will be appended to the message (the default behavior before the addition of `%POS%`).
:::

::: warning "`%PLAYERS%` variable removed in 1.4.0"
Due to how the server keeps track of where players are (only if they are sharing location), the `%PLAYERS%` variable has been disabled indefinitely until a reliable way to gather player positions is developed.
:::
