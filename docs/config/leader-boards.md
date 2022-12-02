# Leader Board Definitions

Filename `discordconnector-leaderBoards.cfg`

Use this file to define custom leader board messages to be sent. Previous versions of DiscordConnector offered only a few options for leader boards, but the custom leader board configuration introduced in 2.1.0, up to 5 custom leader boards can be defined.

There is a section of this config for each  custom board that can be defined. Each is named `LeaderBoard.#`, where `#` is 1-5. By default, all the boards are disabled, so you must manually enable any you wish to use.

## Leader Board Configuration

Each leader board lets you set these options:

| Option                  | Default                    | Description                                                                                                |
| ----------------------- | -------------------------- | ---------------------------------------------------------------------------------------------------------- |
| Enabled                 | false                      | Enable or disable this leader board                                                                        |
| Leader Board Time Range | All Time                   | Set the time restriction of this leader board to one of the [time range values](#leader-board-time-ranges) |
| Number of Rankings      | 3                          | How many rankings to include in the leader board.                                                          |
| Type                    | Most to Least (Descending) | Can be either "Most to Least (Descending)" or "Least to Most (Ascending)"                                  |
| Sending Period          | 600                        | How many minutes to wait before sending the leader board. 600 minutes is the old default of 10 hours.      |
| Death Statistics        | true                       | Include player death statistics in the leader board                                                        |
| Session Statistics      | true                       | Include player session statistics in the leader board (how many times they have played on the server)      |
| Ping Statistics         | true                       | Include player ping statistics in the leader board                                                         |
| Time Online Statistics  | true                       | Include player total play time in the leader board                                                         |
| Leader Board Heading    | LeaderBoard.N              | Title of the leader board which is displayed when sent to discord.                                         |

### Available Predefined Variables (Leader Boards)

In the "Leader Board Heading," the following variables are available:

| Variable | Replaced with..                                   |
| -------- | ------------------------------------------------- |
| `%N`     | The value of "Number of Rankings" (by default: 3) |

### Leader Board Time Ranges

| Option                           | Description                                             |
| -------------------------------- | ------------------------------------------------------- |
| All Time                         | Include every record, as far back as the database goes. |
| Today                            | Include only records from today.                        |
| Yesterday                        | Include only records from yesterday.                    |
| Past 7 Days                      | Include only the past 7 days (inclusive of today)       |
| Current Week, Sunday to Saturday | Include all records from this week (inclusive of today) |
| Current Week, Monday to Sunday   | Include all records from this week (inclusive of today) |

## Active Player Announcement Configuration

These options are available:

| Option                                   | Default | Description                                                                                                            |
| ---------------------------------------- | ------- | ---------------------------------------------------------------------------------------------------------------------- |
| Enabled                                  | false   | Enable or disable this leader board                                                                                    |
| Sending Period                           | 600     | How many minutes to wait before sending the leader board. 600 minutes is the old default of 10 hours.                  |
| Include Currently Online Players         | true    | Enable or disable currently online players as part of the active players announcement                                  |
| Include Unique Players for Today         | true    | Enable or disable unique online players for today as part of the active players announcement                           |
| Include Unique Players for the Past Week | true    | Enable or disable unique online players for the past week (including today) as part of the active players announcement |
| Include Unique Players from All Time     | true    | Enable or disable unique online players from all time as part of the active players announcement                       |  |
