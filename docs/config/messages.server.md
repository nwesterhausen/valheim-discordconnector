# Messages.Server

::: tip Available Dynamic Variables

| Variable            | Replaced with..                                                                           | Can be used in..    |
| ------------------- | ----------------------------------------------------------------------------------------- | ------------------- |
| `%VAR1%` - `VAR10%` | Custom variable value (defined in [Custom Variables](/config/variables.html) config file) | Any messages        |
| `%PUBLICIP%`        | Server's public IP (according to the server)                                              | Any server messages |
| `%DAY_NUMBER%`      | Current day number on server                                                              | Any messages        |
| `%WORLD_NAME%`      | World name of the world used on the server                                                | Any messages        |
| `%NUM_PLAYERS%`     | Number of currently online players                                                        | Any messages        |
| `%JOIN_CODE%`       | Server's join code (only if a join code exists, blank otherwise)                          | Any messages        |

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
