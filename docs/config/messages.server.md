# Messages.Server

::: tip Available Predefined Variables (Server)
| Variable     | Replaced with..                                                        | Can be used in..    |
| ------------ | ---------------------------------------------------------------------- | ------------------- |
| `%PUBLICIP%` | Server's public IP (obtained from [ifconfig.me](https://ifconfig.me/)) | Any server messages |
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
