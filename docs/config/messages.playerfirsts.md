# Messages.PlayerFirsts

::: tip Available Predefined Variables (Players)
| Variable           | Replaced with..              | Can be used in..                             |
| ------------------ | ---------------------------- | -------------------------------------------- |
| `%PLAYER_NAME%`    | Player's character name      | Player join/leave/shout/ping/death messages. |
| `%PLAYER_STEAMID%` | Player's Platform ID         | Player join/leave/shout/ping/death messages. |
| `%PLAYER_ID%`      | Player's Platform ID         | Player join/leave/shout/ping/death messages. |
| `%SHOUT%`          | Text of the player's shout   | Player shout messages.                       |
| `%POS%`            | Player's coordinate position | Player join/leave/shout/ping/death messages. |

:::

:::details Always Available Variables

| Variable            | Replaced with..                                                                          | Can be used in..    |
| ------------------- | ---------------------------------------------------------------------------------------- | ------------------- |
| `%VAR1%` - `VAR10%` | Custom variable value (defined in [Custom Variables](./variables.custom.md) config file) | Any messages        |
| `%PUBLICIP%`        | Server's public IP (according to the server)                                             | Any server messages |
| `%DAY_NUMBER%`      | Current day number on server                                                             | Any messages        |
| `%WORLD_NAME%`      | World name of the world used on the server                                               | Any messages        |
| `%NUM_PLAYERS%`     | Number of currently online players                                                       | Any messages        |
| `%JOIN_CODE%`       | Server's join code (only if a join code exists, blank otherwise)                         | Any messages        |

:::

:::details Random Messages
All of the message options support having multiple messages defined in a semicolon (`;`) separated list. If you have multiple messages defined for these settings, one gets chosen at random when DiscordConnector decides to send the corresponding message.

If you wanted to have a couple different messages for when a player dies (always chosen at random), you could simply set the config value like this:

```toml
Player Death Message = %PLAYER_NAME% has died a beautiful death!;%PLAYER_NAME% went to their end with honor!;%PLAYER_NAME% died.
```

:::

## Player First Join Message

Type: `String`, default value: `Welcome %PLAYER_NAME%, it\'s their first time on the server!`

Set the message that will be sent when a player joins the server If you want to have this choose from a variety of messages at random, separate each message with a semicolon ';'

## Player First Death Message

Type: `String`, default value: `%PLAYER_NAME% has died for the first time.`

Set the message that will be sent when a player dies. If you want to have this choose from a variety of messages at random, separate each message with a semicolon ';'

## Player First Leave Message

Type: `String`, default value: `%PLAYER_NAME% has left for the first time.`

Set the message that will be sent when a player leaves the server. If you want to have this choose from a variety of messages at random, separate each message with a semicolon ';'

## Player First Ping Message

Type: `String`, default value: `%PLAYER_NAME% pings the map for the first time.`

Set the message that will be sent when a player pings the map. If you want to have this choose from a variety of messages at random, separate each message with a semicolon ';'

::: tip
When using the `%POS%` variable, in a Ping message it will reflect the location that was pinged, not the player's location. If you enabled the position toggle is enabled for these messages but you do not include the `%POS%` variable, the position will be appended to the message (the default behavior before the addition of `%POS%`).
:::

## Player First Shout Message

Type: `String`, default value: `%PLAYER_NAME% shouts for the first time.`

Set the message that will be sent when a player shouts on the server. %SHOUT% works in this message to include what was shouted. If you want to have this choose from a variety of messages at random, separate each message with a semicolon ';'
