# Messages

Filename `discordconnector-messages.cfg`

:::tip Random Messages
All of the message options support having multiple messages defined in a semicolon (`;`) separated list. If you have multiple messages defined for these settings, one gets chosen at random when DiscordConnector decides to send the corresponding message.

If you wanted to have a couple different messages for when a player dies (always chosen at random), you could simply set the config value like this:

```toml
Player Death Message = %PLAYER_NAME% has died a beautiful death!;%PLAYER_NAME% went to their end with honor!;%PLAYER_NAME% died.
```

:::

::: tip Custom Variables
Any of the variables in [Variable Definitions](/config/variables) from the variables config file can be referenced in any message.

::: details List of Variables

- `%VAR1%`
- `%VAR2%`
- `%VAR3%`
- `%VAR4%`
- `%VAR5%`
- `%VAR6%`
- `%VAR7%`
- `%VAR8%`
- `%VAR9%`
- `%VAR10%`

:::
