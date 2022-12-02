# Main Config

Filename `discordconnector.cfg`

| Option                                                  | Default | Description                                                                                                                                      |
| ------------------------------------------------------- | ------- | ------------------------------------------------------------------------------------------------------------------------------------------------ |
| Webhook URL                                             | (none)  | The main Discord webhook URL to send notifications/messages to.                                                                                  |
| Use fancier discord messages                            | false   | Set to true to enable using embeds in the Discord messages. If left false, all messages will remain plain strings (except for the leaderboard).  |
| Allow positions to be sent                              | true    | Set to false to prevent any positions/coordinates from being sent. If this is true, it can be overridden per message in the toggles config file. |
| Ignored players                                         | (none)  | List of player names to never send a discord message for (they also won't be tracked in stats). This list should be semicolon (`;`) separated.   |
| Ignored players (Regex)                                 | (none)  | Regex which player names are matched against to determine to not send a discord message for (they also won't be tracked in stats)                |
| Collect stats                                           | true    | When this setting is enabled, DiscordConnector will record basic stats (leave, join, ping, shout, death) about players.                          |
| Send leaderboard updates                                | false   | If you set this to true, that will enable DiscordConnector to send a leaderboard for stats to Discord on the set interval                        |
| Leaderboard update interval                             | 600     | Time in minutes between each leaderboard update sent to Discord.                                                                                 |
| Announce Player Firsts                                  | true    | Disable this setting to disable all extra announcements the first time each player does something. (Overwrites any individual setting.)          |
| How many places to list in the top ranking leaderboards | 3       | Set how many places (1st, 2nd, 3rd by default) to display when sending the ranked leaderboard.                                                   |
| Send Non-Player Shouts to Discord                       | false   | Enable this setting to have shouts which are performed by other mods/the server/non-players to be sent to Discord as well.                       |

!!! info "Stat Collection Details"

     Stat collection will create a file in the `discordconnector` config directory `records.db`, where it will record the number of times each player joins, leaves, dies, shouts or pings.
     If this is set to false, DiscordConnector will not keep a record of number of times each player does something it alerts to.
     If this is false, it takes precedent over the "Send leader board updates" setting and no leader boards will get sent.

     The stat collection database uses the [LiteDB](https://www.litedb.org/) library and if you are so inclined they offer a database gui which you can use to view/modify this database. (Find the LiteDB Editor on their site.)
