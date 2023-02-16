# Main Config

Filename `discordconnector.cfg`

| Option                                                  | Default | Description                                                                                                                                      |
| ------------------------------------------------------- | ------- | ------------------------------------------------------------------------------------------------------------------------------------------------ |
| Webhook URL                                             | (none)  | The main Discord webhook URL to send notifications/messages to.                                                                                  |
| Webhook Events                                          | ALL     | Semi-colon separated list of which [events](#webhook-events) to send to the primary webhook.                                                     |
| Secondary Webhook URL                                   | (none)  | The main Discord webhook URL to send notifications/messages to.                                                                                  |
| Secondary Webhook Events                                | (none)  | Semi-colon separated list of which [events](#webhook-events) to send to the primary webhook.                                                     |
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

::: info "Stat Collection Details"
Stat collection will create a file in the `discordconnector` config directory `records.db`, where it will record the number of times each player joins, leaves, dies, shouts or pings.

If this is set to false, DiscordConnector will not keep a record of number of times each player does something it alerts to.

If this is false, it takes precedent over the "Send leader board updates" setting and no leader boards will get sent.

The stat collection database uses the [LiteDB](https://www.litedb.org/) library and if you are so inclined they offer a database gui which you can use to view/modify this database. (Find the LiteDB Editor on their site.)
:::

## Webhook Events

Here is a list of every accepted event for inclusion in the 'webhook event' settings. These correspond pretty directly to the configurations in the messages config.

::: info "Specifying Events ≠ Enabling Messages"
Adding an event to be send for a webhook does not enable Discord Connector to send that event. It only tells Discord Connector *where* to send the event message.

Messages for these events have to be enabled in the toggles config file (as they were previously). Any of these messages which are enabled by default are notated as such in the 'Enabled by Default' column.

Take note that by default, the webhooks are set to 'ALL' which takes any messages which happen and sends them.
:::

| Event Code       | Corresponding Trigger                         | Enabled by Default |
| ---------------- | --------------------------------------------- | ------------------ |
| serverLaunch     | Server begins starting up                     | Yes                |
| serverStart      | World has been loaded                         | Yes                |
| serverStop       | Server beings stopping                        | Yes                |
| serverShutdown   | Server has stopped                            | Yes                |
| serverSave       | The world has been saved                      | Yes                |
| eventStart       | An event starts                               | Yes                |
| eventPaused      | An event pauses                               | Yes                |
| eventResumed     | An event resumes                              | Yes                |
| eventStop        | An event finishes                             | Yes                |
| playerJoin       | A player joins the server                     | Yes                |
| playerLeave      | A player joins the server                     | Yes                |
| playerShout      | A player joins the server                     | Yes                |
| playerPing       | A player joins the server                     | Yes                |
| playerDeath      | A player joins the server                     | Yes                |
| playerFirstJoin  | A new player joins the server                 | Yes                |
| playerFirstLeave | A player leaves the server for the first time | No                 |
| playerFirstShout | A player shouts in chat for the first time    | No                 |
| playerFirstPing  | A player pings the map for the first time     | No                 |
| playerFirstDeath | A player dies for the first time              | Yes                |

### Leaderboard Events

These are events which represent sending the leaderboards

| Event Code    | Corresponding Trigger          | Enabled by Default |
| ------------- | ------------------------------ | ------------------ |
| activePlayers | The active players leaderboard | No                 |
| leaderboard1  | The custom leaderboard 1       | No                 |
| leaderboard2  | The custom leaderboard 2       | No                 |
| leaderboard3  | The custom leaderboard 3       | No                 |
| leaderboard4  | The custom leaderboard 4       | No                 |
| leaderboard5  | The custom leaderboard 5       | No                 |

### 3rd Party Events

It's possible for other mods to hook into Discord Connector to send messages to Discord. As more get added to this list,
they will be able to be specifically attached to a certain webhook.


| Event Code    | Corresponding Trigger          |
| ------------- | ------------------------------ |
| cronjob       | Output from cronjob            |

### Special Case Events

These are shorthand to represent multiple events.

| Event Code      | Corresponding Trigger                                                                                              | Enabled by Default |
| --------------- | ------------------------------------------------------------------------------------------------------------------ | ------------------ |
| ALL             | All messages get sent to this webhook                                                                              | N/A                |
| serverLifecycle | Any server launch, start, stop, shutdown messages are sent to this webhook                                         | N/A                |
| eventLifecycle  | Any event start, pause, resume, or stop/end messages are sent to this webhook                                      | N/A                |
| playerAll       | Any messages about player's activity are sent to this webhook                                                      | N/A                |
| playerFirstAll  | Any message about a player's first X are sent to this webhook                                                      | N/A                |
| leaderboardsAll | Any message for a leaderboard are sent to this webhook                                                             | N/A                |
| other           | A few other mods/plugins utilize Discord Connector's webhook to send messages. Those get tagged as 'Other' events. | N/A                |
