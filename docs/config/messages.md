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

## Messages.Events

::: tip
Available Predefined Variables (Events)

In the event messages, anywhere in the message you can use the string vars `%EVENT_START_MSG%` and `%EVENT_END_MSG%` (or `%EVENT_MSG%` which is uses the start message when the event starts and the end message when the event ends.) You can also use `%POS%` which will be replaced with their position if the position toggle is enabled.

| Variable            | Replaced with..                                          | Can be used in..         |
| ------------------- | -------------------------------------------------------- | ------------------------ |
| `%EVENT_START_MSG%` | The event start message (e.g. "The forest is moving...") | Event start message      |
| `%EVENT_END_MSG%`   | The event stop message (e.g. "The forest rests again")   | Event stop message       |
| `%EVENT_MSG%`       | The appropriate start/end message for the event          | Event start/stop message |
:::

::: Head's Up!
If you enabled the position toggle is enabled for these messages but you do not include the `%POS%` variable, the position will be appended to the message (the default behavior before the addition of `%POS%`).
:::

### Event Start Message

Type: `String`, default value: `**Event**: %EVENT_MSG%`

 Set the message that will be sent when a random event starts on the server. Sending the coordinates is enabled by default in the toggles config. If you want to have this choose from a variety of messages at random, separate each message with a semicolon ';' The special string %EVENT_MSG% will be replaced with the message that is displayed on the screen when the event starts.

### Event Stop Message

Type: `String`, default value: `**Event**: %EVENT_MSG%`

 Set the message that will be sent when a random event stops on the server. Sending the coordinates is enabled by default in the toggles config. If you want to have this choose from a variety of messages at random, separate each message with a semicolon ';' The special string %EVENT_MSG% will be replaced with the message that is displayed on the screen when the event stops.

### Event Paused Message

Type: `String`, default value: `**Event**: %EVENT_END_MSG% — for now! (Currently paused due to no players in the event area.)`

 Set the message that will be sent when a random event is paused due to players leaving the area. Sending the coordinates is enabled by default in the toggles config. If you want to have this choose from a variety of messages at random, separate each message with a semicolon ';' The special string %EVENT_START_MSG% will be replaced with the message that is displayed on the screen when the event starts. The special string %EVENT_END_MSG% will be replaced with the message that is displayed on the screen when the event ends.

## Messages.LeaderBoards

### Event Resumed Message

Type: `String`, default value: `**Event**: %EVENT_START_MSG%`

 Set the message that will be sent when a random event is resumed due to players re-entering the area. Sending the coordinates is enabled by default in the toggles config. If you want to have this choose from a variety of messages at random, separate each message with a semicolon ';' The special string %EVENT_START_MSG% will be replaced with the message that is displayed on the screen when the event starts. The special string %EVENT_END_MSG% will be replaced with the message that is displayed on the screen when the event ends.

### Leader Board Heading for Top N Players

Type: `String`, default value: `Top %N% Player Leader Boards:`

 Set the message that is included as a heading when this leader board is sent. Include %N% to include the number of rankings returned (the configured number)

### Leader Board Heading for Bottom N Players

Type: `String`, default value: `Bottom %N% Player Leader Boards:`

 Set the message that is included as a heading when this leader board is sent. Include %N% to include the number of rankings returned (the configured number)

### Leader Board Heading for Highest Player

Type: `String`, default value: `Top Performer`

 Set the message that is included as a heading when this leader board is sent. Include %N% to include the number of rankings returned (the configured number)

## Messages.Player

::: tip
Available Predefined Variables (Players)

| Variable           | Replaced with..              | Can be used in..                             |
| ------------------ | ---------------------------- | -------------------------------------------- |
| `%PLAYER_NAME%`    | Player's character name      | Player join/leave/shout/ping/death messages. |
| `%PLAYER_STEAMID%` | Player's Platform ID         | Player join/leave/shout/ping/death messages. |
| `%PLAYER_ID%`      | Player's Platform ID         | Player join/leave/shout/ping/death messages. |
| `%SHOUT%`          | Text of the player's shout   | Player shout messages.                       |
| `%POS%`            | Player's coordinate position | Player join/leave/shout/ping/death messages. |
:::

### Leader Board Heading for Lowest Player

Type: `String`, default value: `Bottom Performer`

 Set the message that is included as a heading when this leader board is sent. Include %N% to include the number of rankings returned (the configured number)

### Player Join Message

Type: `String`, default value: `%PLAYER_NAME% has joined.`

 Set the message that will be sent when a player joins the server If you want to have this choose from a variety of messages at random, separate each message with a semicolon ';' Random choice example: %PLAYER_NAME% has joined;%PLAYER_NAME% awakens;%PLAYER_NAME% arrives

### Player Death Message

Type: `String`, default value: `%PLAYER_NAME% has died.`

 Set the message that will be sent when a player dies. If you want to have this choose from a variety of messages at random, separate each message with a semicolon ';'

### Player Leave Message

Type: `String`, default value: `%PLAYER_NAME% has left.`

 Set the message that will be sent when a player leaves the server. If you want to have this choose from a variety of messages at random, separate each message with a semicolon ';'

### Player Ping Message

Type: `String`, default value: `%PLAYER_NAME% pings the map.`

 Set the message that will be sent when a player pings the map. If you want to have this choose from a variety of messages at random, separate each message with a semicolon ';'

::: tip
When using the `%POS%` variable, in a Ping message it will reflect the location that was pinged, not the player's location. If you enabled the position toggle is enabled for these messages but you do not include the `%POS%` variable, the position will be appended to the message (the default behavior before the addition of `%POS%`).
:::

## Messages.PlayerFirsts

::: tip
Available Predefined Variables (Players)

| Variable           | Replaced with..              | Can be used in..                             |
| ------------------ | ---------------------------- | -------------------------------------------- |
| `%PLAYER_NAME%`    | Player's character name      | Player join/leave/shout/ping/death messages. |
| `%PLAYER_STEAMID%` | Player's Platform ID         | Player join/leave/shout/ping/death messages. |
| `%PLAYER_ID%`      | Player's Platform ID         | Player join/leave/shout/ping/death messages. |
| `%SHOUT%`          | Text of the player's shout   | Player shout messages.                       |
| `%POS%`            | Player's coordinate position | Player join/leave/shout/ping/death messages. |
:::

### Player Shout Message

Type: `String`, default value: `%PLAYER_NAME% shouts **%SHOUT%**.`

 Set the message that will be sent when a player shouts on the server. You can put %SHOUT% anywhere you want the content of the shout to be. If you want to have this choose from a variety of messages at random, separate each message with a semicolon ';'

### Player First Join Message

Type: `String`, default value: `Welcome %PLAYER_NAME%, it\'s their first time on the server!`

 Set the message that will be sent when a player joins the server If you want to have this choose from a variety of messages at random, separate each message with a semicolon ';'

### Player First Death Message

Type: `String`, default value: `%PLAYER_NAME% has died for the first time.`

 Set the message that will be sent when a player dies. If you want to have this choose from a variety of messages at random, separate each message with a semicolon ';'

### Player First Leave Message

Type: `String`, default value: `%PLAYER_NAME% has left for the first time.`

 Set the message that will be sent when a player leaves the server. If you want to have this choose from a variety of messages at random, separate each message with a semicolon ';'

### Player First Ping Message

Type: `String`, default value: `%PLAYER_NAME% pings the map for the first time.`

 Set the message that will be sent when a player pings the map. If you want to have this choose from a variety of messages at random, separate each message with a semicolon ';'

::: tip
When using the `%POS%` variable, in a Ping message it will reflect the location that was pinged, not the player's location. If you enabled the position toggle is enabled for these messages but you do not include the `%POS%` variable, the position will be appended to the message (the default behavior before the addition of `%POS%`).
:::

## Messages.Server                              |

::: tip
Available Predefined Variables (Server)

| Variable     | Replaced with..                                                        | Can be used in..    |
| ------------ | ---------------------------------------------------------------------- | ------------------- |
| `%PUBLICIP%` | Server's public IP (obtained from [ifconfig.me](https://ifconfig.me/)) | Any server messages |
:::

### Player First Shout Message

Type: `String`, default value: `%PLAYER_NAME% shouts for the first time.`

 Set the message that will be sent when a player shouts on the server. %SHOUT% works in this message to include what was shouted. If you want to have this choose from a variety of messages at random, separate each message with a semicolon ';'

### Server Launch Message

Type: `String`, default value: `Server is starting up.`

 Set the message that will be sent when the server starts up. If you want to have this choose from a variety of messages at random, separate each message with a semicolon ';' Random choice example: Server is starting;Server beginning to load If you use %PUBLICIP% in this message, it will be replaced with the public IP address of the server.

### Server Started Message

Type: `String`, default value: `Server has started!`

 Set the message that will be sent when the server has loaded the map and is ready for connections. If you want to have this choose from a variety of messages at random, separate each message with a semicolon ';' If you use %PUBLICIP% in this message, it will be replaced with the public IP address of the server.

### Server Stop Message

Type: `String`, default value: `Server is stopping.`

 Set the message that will be sent when the server shuts down. If you want to have this choose from a variety of messages at random, separate each message with a semicolon ';' If you use %PUBLICIP% in this message, it will be replaced with the public IP address of the server.

### Server Shutdown Message

Type: `String`, default value: `Server has stopped!`

 Set the message that will be sent when the server finishes shutting down. If you want to have this choose from a variety of messages at random, separate each message with a semicolon ';' If you use %PUBLICIP% in this message, it will be replaced with the public IP address of the server.

### Server Saved Message

Type: `String`, default value: `The world has been saved.`

 Set the message that will be sent when the server saves the world data. If you want to have this choose from a variety of messages at random, separate each message with a semicolon ';' If you use %PUBLICIP% in this message, it will be replaced with the public IP address of the server.
