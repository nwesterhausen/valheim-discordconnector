# Leader Board Definitions

Filename `discordconnector-leaderBoards.cfg`

Use this file to define custom leader board messages to be sent. Previous versions of DiscordConnector offered only a few options for leader boards, but the custom leader board configuration introduced in 2.1.0, up to 5 custom leader boards can be defined.

There is a section of this config for each  custom board that can be defined. Each is named `LeaderBoard.#`, where `#` is 1-5. By default, all the boards are disabled, so you must manually enable any you wish to use.

## ActivePlayers.Announcement

### Enabled (Active Players)

Type: `Boolean`, default value: `false`

 Enable or disable the active players announcement being sent to Discord

### Sending Period (Active Players)

Type: `Int32`, default value: `360`

 Set the number of minutes between a leader board announcement sent to discord. (Default period is 6 hours.)

### Include Currently Online Players

Type: `Boolean`, default value: `true`

 Enable or disable currently online players as part of the active players announcement

### Include Unique Players for Today

Type: `Boolean`, default value: `true`

 Enable or disable unique online players for today as part of the active players announcement

### Include Unique Players for the Past Week

Type: `Boolean`, default value: `true`

 Enable or disable unique online players for the past week (including today) as part of the active players announcement

## LeaderBoard.1 ... LeaderBoard.5

Each leader board lets you set these options. Each one is independent of the other.

### Include Unique Players from All Time

Type: `Boolean`, default value: `true`

 Enable or disable unique online players from all time as part of the active players announcement

### Enabled

Type: `Boolean`, default value: `false`

 Enable or disable this leader board.

### Leader Board Heading

Type: `String`, default value: `LeaderBoard.1 Statistic Leader Board`

 Define the heading message to display with this leader board. Include %N% to dynamically reference the value in "Number of Rankings"

:::info Available Predefined Variables (Leader Boards)
In the "Leader Board Heading," the following variables are available:

| Variable | Replaced with..                                   |
| -------- | ------------------------------------------------- |
| `%N`     | The value of "Number of Rankings" (by default: 3) |
:::

### Leader Board Time Range

Type: `TimeRange`, default value: `AllTime`

Acceptable values: `AllTime`, `Today`, `Yesterday`, `PastWeek`, `WeekSundayToSaturday`, `WeekMondayToSunday`

 A more restrictive filter of time can be applied to the leader board. This restricts it to tally up statistics within the range specified. AllTime: Apply no time restriction to the leader board, use all available records. Today: Restrict leader board to recorded events from today. Yesterday: Restrict leader board to recorded events from yesterday. PastWeek: Restrict leader board to recorded events from the past week (including today). WeekSundayToSaturday: Restrict leader board to recorded events from the current week, beginning on Sunday and ending Saturday. WeekMondayToSunday: Restrict leader board to recorded events from the current week, beginning on Monday and ending Sunday.

::: info
Leader Board Time Ranges

| Option                           | Description                                             |
| -------------------------------- | ------------------------------------------------------- |
| All Time                         | Include every record, as far back as the database goes. |
| Today                            | Include only records from today.                        |
| Yesterday                        | Include only records from yesterday.                    |
| Past 7 Days                      | Include only the past 7 days (inclusive of today)       |
| Current Week, Sunday to Saturday | Include all records from this week (inclusive of today) |
| Current Week, Monday to Sunday   | Include all records from this week (inclusive of today) |
:::

### Sending Period

Type: `Int32`, default value: `600`

 Set the number of minutes between a leader board announcement sent to discord. This timer starts when the server is started. Default is set to 10 hours (600 minutes).

### Type

Type: `Ordering`, default value: `Descending`

Acceptable values: `Descending`, `Ascending`

 Choose what type of leader board this should be. There are 2 options: Descending:"Number of Rankings" players (with at least 1 record) are listed in descending order Ascending:  "Number of Rankings" players (with at least 1 record) are listed in ascending order

### Number of Rankings

Type: `Int32`, default value: `3`

 Specify a number of places in the leader board. Setting this can help prevent a very long leader board in the case of active servers. Setting to 0 (zero) results in limiting to the hard-coded maximum of 16.

### Death Statistics

Type: `Boolean`, default value: `true`

 If enabled, player death statistics will be part of the leader board.

### Session Statistics

Type: `Boolean`, default value: `false`

 If enabled, player session statistics will be part of the leader board.

### Shout Statistics

Type: `Boolean`, default value: `false`

 If enabled, player shout statistics will be part of the leader board.

### Ping Statistics

Type: `Boolean`, default value: `false`

 If enabled, player ping statistics will be part of the leader board.
