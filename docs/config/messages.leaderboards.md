# Messages.LeaderBoards

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

## Leader Board Heading for Top N Players

Type: `String`, default value: `Top %N% Player Leader Boards:`

Set the message that is included as a heading when this leader board is sent. Include %N% to include the number of rankings returned (the configured number)

## Leader Board Heading for Bottom N Players

Type: `String`, default value: `Bottom %N% Player Leader Boards:`

Set the message that is included as a heading when this leader board is sent. Include %N% to include the number of rankings returned (the configured number)

## Leader Board Heading for Highest Player

Type: `String`, default value: `Top Performer`

Set the message that is included as a heading when this leader board is sent. Include %N% to include the number of rankings returned (the configured number)

## Leader Board Heading for Lowest Player

Type: `String`, default value: `Bottom Performer`

Set the message that is included as a heading when this leader board is sent. Include %N% to include the number of rankings returned (the configured number)
