# Messages

Filename `discordconnector-messages.cfg`

## Random Messages

All of the message options support having multiple messages defined in a semicolon (`;`) separated list. If you have multiple messages defined for these settings, one gets chosen at random when DiscordConnector decides to send the corresponding message.

If you wanted to have a couple different messages for when a player dies (always chosen at random), you could simply set the config value like this:

```toml
Player Death Message = %PLAYER_NAME% has died a beautiful death!;%PLAYER_NAME% went to their end with honor!;%PLAYER_NAME% died.
```

## Variables

Every message supports the following variables:

| Variable        | Replaced with                                                              | Notes                                                                                                                                                                                |
| --------------- | -------------------------------------------------------------------------- | ------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------ |
| `%PUBLICIP%`    | Server's public IP (from a request to [ifconfig.me](https://ifconfig.me/)) | This is set up once at server launch.                                                                                                                                                |
| `%DAY_NUMBER%`  | Current day number on server                                               | This will be 0 until the world actually gets loaded.                                                                                                                                 |
| `%WORLD_NAME%`  | World name of the world used on the server                                 | This isn't available until the [serverLoaded](./messages.server.md#server-started-message) event (can't use it in [serverStart](./messages.server.md#server-launch-message) message) |
| `%NUM_PLAYERS%` | Number of currently online players                                         | If used in a [playerLeave](./messages.player.md#player-leave-message) message, it will be auto decremented                                                                           |
| `%JOIN_CODE%`   | Server's join code (only if a join code exists, blank otherwise)           | This won't be available until the [serverLoaded](./messages.server.md#server-started-message) eventmessages                                                                          |
| `%VAR1%`        | Value specified in the [custom variable config](./variables.custom.md)     | Custom variables can contain other variables, which will be replaced if the type of message supports it.                                                                             |
| `%VAR2%`        | Value specified in the [custom variable config](./variables.custom.md)     | Custom variables can contain other variables, which will be replaced if the type of message supports it.                                                                             |
| `%VAR3%`        | Value specified in the [custom variable config](./variables.custom.md)     | Custom variables can contain other variables, which will be replaced if the type of message supports it.                                                                             |
| `%VAR4%`        | Value specified in the [custom variable config](./variables.custom.md)     | Custom variables can contain other variables, which will be replaced if the type of message supports it.                                                                             |
| `%VAR5%`        | Value specified in the [custom variable config](./variables.custom.md)     | Custom variables can contain other variables, which will be replaced if the type of message supports it.                                                                             |
| `%VAR6%`        | Value specified in the [custom variable config](./variables.custom.md)     | Custom variables can contain other variables, which will be replaced if the type of message supports it.                                                                             |
| `%VAR7%`        | Value specified in the [custom variable config](./variables.custom.md)     | Custom variables can contain other variables, which will be replaced if the type of message supports it.                                                                             |
| `%VAR8%`        | Value specified in the [custom variable config](./variables.custom.md)     | Custom variables can contain other variables, which will be replaced if the type of message supports it.                                                                             |
| `%VAR9%`        | Value specified in the [custom variable config](./variables.custom.md)     | Custom variables can contain other variables, which will be replaced if the type of message supports it.                                                                             |
| `%VAR10%`       | Value specified in the [custom variable config](./variables.custom.md)     | Custom variables can contain other variables, which will be replaced if the type of message supports it.                                                                             |

Some messages support additional variables, which are mentioned specifically on their config page; e.g., messages involving player actions support a position and other player information variables.
