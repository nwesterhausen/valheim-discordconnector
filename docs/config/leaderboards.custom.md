# Custom Leaderboards

Each leader board lets you set these options. Each one is independent of the other.

Even though they are listed under headings `Leaderboard.1`, `Leaderboard.2`, etc., you can set what heading appears in the message sent to discord by modifying the [leader board heading](/config/leaderboards.custom.html#leader-board-heading) setting.

## Enabled

Type: `Boolean`, default value: `false`

Enable or disable this leader board.

## Leader Board Heading

Type: `String`, default value: `LeaderBoard.1 Statistic Leader Board`

Define the heading message to display with this leader board.

:::tip Available Predefined Variables (Leader Boards)
In the "Leader Board Heading," the following variables are available:

| Variable | Replaced with..                                   |
| -------- | ------------------------------------------------- |
| `%N%`    | The value of "Number of Rankings" (by default: 3) |
:::

## Leader Board Time Range

Type: `TimeRange`, default value: `AllTime`

Acceptable values: `AllTime`, `Today`, `Yesterday`, `PastWeek`, `WeekSundayToSaturday`, `WeekMondayToSunday`

A more restrictive filter of time can be applied to the leader board. This restricts it to tally up statistics within the range specified.

::: info Leader Board Time Ranges
| Option                           | Description                                             |
| -------------------------------- | ------------------------------------------------------- |
| All Time                         | Include every record, as far back as the database goes. |
| Today                            | Include only records from today.                        |
| Yesterday                        | Include only records from yesterday.                    |
| Past 7 Days                      | Include only the past 7 days (inclusive of today)       |
| Current Week, Sunday to Saturday | Include all records from this week (inclusive of today) |
| Current Week, Monday to Sunday   | Include all records from this week (inclusive of today) |
:::

## Sending Period

Type: `Int32`, default value: `600`

Set the number of minutes between a leader board announcement sent to discord. This timer starts when the server is started. Default is set to 10 hours (600 minutes).

## Type

Type: `Ordering`, default value: `Descending`

Acceptable values: `Descending`, `Ascending`

Choose what type of leader board this should be.

:::info Ordering Options
| Option     | Description                                                                          |
| ---------- | ------------------------------------------------------------------------------------ |
| Descending | "Number of Rankings" players (with at least 1 record) are listed in descending order |
| Ascending  | "Number of Rankings" players (with at least 1 record) are listed in ascending order  |
:::

## Number of Rankings

Type: `Int32`, default value: `3`

Specify a number of places in the leader board. Setting this can help prevent a very long leader board in the case of active servers.

:::warning
Setting to 0 (zero) results in limiting to the hard-coded maximum of 16.
:::

## Death Statistics

Type: `Boolean`, default value: `true`

If enabled, player death statistics will be part of the leader board.

## Session Statistics

Type: `Boolean`, default value: `false`

If enabled, player session statistics will be part of the leader board.

## Shout Statistics

Type: `Boolean`, default value: `false`

If enabled, player shout statistics will be part of the leader board.

## Ping Statistics

Type: `Boolean`, default value: `false`

If enabled, player ping statistics will be part of the leader board.
