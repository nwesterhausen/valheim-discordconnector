# Messages.Server

:::details Always Available Variables

| Variable            | Replaced with..                                                                          | Can be used in..                                                                                |
| ------------------- | ---------------------------------------------------------------------------------------- | ----------------------------------------------------------------------------------------------- |
| `%VAR1%` - `VAR10%` | Custom variable value (defined in [Custom Variables](./variables.custom.md) config file) | Any messages                                                                                    |
| `%PUBLICIP%`        | Server's public IP (according to the server)                                             | Any server messages                                                                             |
| `%DAY_NUMBER%`      | Current day number on server                                                             | Any messages                                                                                    |
| `%WORLD_NAME%`      | World name of the world used on the server                                               | Any messages                                                                                    |
| `%NUM_PLAYERS%`     | Number of currently online players                                                       | Any messages                                                                                    |
| `%JOIN_CODE%`       | Server's join code (only if a join code exists, blank otherwise)                         | Any messages                                                                                    |
| `%TIMESTAMP%`       | `<t:UNIX_TIMESTAMP>`                                                                     | Replaced with a Discord timestamp that will be converted to the user's local time.              |
| `%TIMESINCE%`       | `<t:UNIX_TIMESTAMP:R>`                                                                   | Replaced with a Discord timestamp that will be converted to a relative time (e.g. 2 hours ago). |
| `%UNIX_TIMESTAMP%`  | UNIX timestamp of the event (e.g 12039232)                                               | This can be used to create a custom timestamp format in the message.                            |

:::

:::details Random Messages
All of the message options support having multiple messages defined in a semicolon (`;`) separated list. If you have multiple messages defined for these settings, one gets chosen at random when DiscordConnector decides to send the corresponding message.

If you wanted to have a couple different messages for when a player dies (always chosen at random), you could simply set the config value like this:

```toml
Player Death Message = %PLAYER_NAME% has died a beautiful death!;%PLAYER_NAME% went to their end with honor!;%PLAYER_NAME% died.
```

:::

## Server Launch Message

Type: `String`, default value: `Server is starting up.`

Set the message that will be sent when the server starts up. If you want to have this choose from a variety of messages at random, separate each message with a semicolon ';' Random choice example: Server is starting;Server beginning to load If you use %PUBLICIP% in this message, it will be replaced with the public IP address of the server.

## Server Started Message

Type: `String`, default value: `Server has started!`

Set the message that will be sent when the server has loaded the map and is ready for connections. If you want to have this choose from a variety of messages at random, separate each message with a semicolon ';' If you use %PUBLICIP% in this message, it will be replaced with the public IP address of the server.

## Server Stop Message

Type: `String`, default value: `Server is stopping.`

Set the message that will be sent when the server shuts down. If you want to have this choose from a variety of messages at random, separate each message with a semicolon ';' If you use %PUBLICIP% in this message, it will be replaced with the public IP address of the server.

## Server Shutdown Message

Type: `String`, default value: `Server has stopped!`

Set the message that will be sent when the server finishes shutting down. If you want to have this choose from a variety of messages at random, separate each message with a semicolon ';' If you use %PUBLICIP% in this message, it will be replaced with the public IP address of the server.

## Server Saved Message

Type: `String`, default value: `The world has been saved.`

Set the message that will be sent when the server saves the world data. If you want to have this choose from a variety of messages at random, separate each message with a semicolon ';' If you use %PUBLICIP% in this message, it will be replaced with the public IP address of the server.

## Server New Day Message

Type: `String`, default value: `Day Number %DAY_NUMBER%`

Set the message that will be sent when a new day starts. The `%DAY_NUMBER%` variable gets replaced with the new day number (e.g. "Day Number 34").
