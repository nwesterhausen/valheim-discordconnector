# Messages.Events

::: tip Available Predefined Variables (Events)
In the event messages, anywhere in the message you can use the string vars `%EVENT_START_MSG%` and `%EVENT_END_MSG%` (or `%EVENT_MSG%` which is uses the start message when the event starts and the end message when the event ends.) You can also use `%POS%` which will be replaced with their position if the position toggle is enabled.

| Variable            | Replaced with..                                          | Can be used in..         |
| ------------------- | -------------------------------------------------------- | ------------------------ |
| `%EVENT_START_MSG%` | The event start message (e.g. "The forest is moving...") | Event start message      |
| `%EVENT_END_MSG%`   | The event stop message (e.g. "The forest rests again")   | Event stop message       |
| `%EVENT_MSG%`       | The appropriate start/end message for the event          | Event start/stop message |
:::

::: tip Available Dynamic Variables

| Variable            | Replaced with..                                                                           | Can be used in..    |
| ------------------- | ----------------------------------------------------------------------------------------- | ------------------- |
| `%VAR1%` - `VAR10%` | Custom variable value (defined in [Custom Variables](/config/variables.html) config file) | Any messages        |
| `%PUBLICIP%`        | Server's public IP (according to the server)                                              | Any server messages |
| `%DAY_NUMBER%`      | Current day number on server                                                              | Any messages        |
:::

:::details Random Messages
All of the message options support having multiple messages defined in a semicolon (`;`) separated list. If you have multiple messages defined for these settings, one gets chosen at random when DiscordConnector decides to send the corresponding message.

If you wanted to have a couple different messages for when a player dies (always chosen at random), you could simply set the config value like this:

```toml
Player Death Message = %PLAYER_NAME% has died a beautiful death!;%PLAYER_NAME% went to their end with honor!;%PLAYER_NAME% died.
```

:::

::: details Available Custom Variables
These are defined in the [Custom Variables](/config/variables.html) config file.

`%VAR1%`, `%VAR2%`, `%VAR3%`, `%VAR4%`, `%VAR5%`, `%VAR6%`, `%VAR7%`, `%VAR8%`, `%VAR9%`, `%VAR10%`
:::

::: warning Including the Event Position
If you enabled the position toggle is enabled for these messages but you do not include the `%POS%` variable, the position will be appended to the message (the default behavior before the addition of `%POS%`).
:::

## Event Start Message

Type: `String`, default value: `**Event**: %EVENT_MSG%`

Set the message that will be sent when a random event starts on the server. Sending the coordinates is enabled by default in the toggles config. If you want to have this choose from a variety of messages at random, separate each message with a semicolon ';' The special string %EVENT_MSG% will be replaced with the message that is displayed on the screen when the event starts.

## Event Stop Message

Type: `String`, default value: `**Event**: %EVENT_MSG%`

Set the message that will be sent when a random event stops on the server. Sending the coordinates is enabled by default in the toggles config. If you want to have this choose from a variety of messages at random, separate each message with a semicolon ';' The special string %EVENT_MSG% will be replaced with the message that is displayed on the screen when the event stops.

## Event Paused Message

Type: `String`, default value: `**Event**: %EVENT_END_MSG% — for now! (Currently paused due to no players in the event area.)`

Set the message that will be sent when a random event is paused due to players leaving the area. Sending the coordinates is enabled by default in the toggles config. If you want to have this choose from a variety of messages at random, separate each message with a semicolon ';' The special string %EVENT_START_MSG% will be replaced with the message that is displayed on the screen when the event starts. The special string %EVENT_END_MSG% will be replaced with the message that is displayed on the screen when the event ends.

## Event Resumed Message

Type: `String`, default value: `**Event**: %EVENT_START_MSG%`

Set the message that will be sent when a random event is resumed due to players re-entering the area. Sending the coordinates is enabled by default in the toggles config. If you want to have this choose from a variety of messages at random, separate each message with a semicolon ';' The special string %EVENT_START_MSG% will be replaced with the message that is displayed on the screen when the event starts. The special string %EVENT_END_MSG% will be replaced with the message that is displayed on the screen when the event ends.
