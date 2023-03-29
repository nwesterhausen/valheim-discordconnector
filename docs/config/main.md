# Main Config

Filename `discordconnector.cfg`

## Main Settings

### Webhook URL

Type: `String`, default value: ``

 Discord channel webhook URL. For instructions, reference the 'MAKING A WEBHOOK' section of [Discord's documentation](https://support.Discord.com/hc/en-us/articles/228383668-Intro-to-Webhook).

### Webhook Events

Type: `String`, default value: `ALL`

 Specify a subset of possible events to send to the primary webhook. Previously all events would go to the primary webhook. Format should be the keyword 'ALL' or a semi-colon separated list, e.g. 'serverLifecycle;playerAll;playerFirstAll;leaderboardsAll;' Full list of valid options [here](https://discordconnector.valheim.nwest.games/config/main.html#webhook-event-descriptions).

### Secondary Webhook URL

Type: `String`, default value: ``

 Discord channel webhook URL. For instructions, reference the 'MAKING A WEBHOOK' section of [Discord's documentation](https://support.Discord.com/hc/en-us/articles/228383668-Intro-to-Webhook).

### Secondary Webhook Events

Type: `String`, default value: `ALL`

 Specify a subset of possible events to send to the primary webhook. Previously all events would go to the primary webhook. Format should be the keyword 'ALL' or a semi-colon separated list, e.g. 'serverLaunch;serverStart;serverSave;' Full list of valid options [here](https://discordconnector.valheim.nwest.games/config/main.html#webhook-event-descriptions).

### Use fancier discord messages

Type: `Boolean`, default value: `false`

 Enable this setting to use embeds in the messages sent to Discord. Currently this will affect the position details for the messages.

### Ignored Players

Type: `String`, default value: ``

 It may be that you have some players that you never want to send Discord messages for. Adding a player name to this list will ignore them. Format should be a semicolon-separated list: Stuart;John McJohnny;Weird-name1

### Ignored Players (Regex)

Type: `String`, default value: ``

 It may be that you have some players that you never want to send Discord messages for. This option lets you provide a regular expression to filter out players if their name matches. Format should be a valid string for a .NET Regex (reference: [docs.microsoft.com](https://docs.microsoft.com/en-us/dotnet/standard/base-types/regular-expression-language-quick-reference))

### Send Positions with Messages

Type: `Boolean`, default value: `true`

 Disable this setting to disable any positions/coordinates being sent with messages (e.g. players deaths or players joining/leaving). (Overwrites any individual setting.)

### Collect Player Stats

Type: `Boolean`, default value: `true`

 Disable this setting to disable all stat collection. (Overwrites any individual setting.)

::: info "Stat Collection Details"
Stat collection will create a file in the `discordconnector` config directory `records.db`, where it will record the number of times each player joins, leaves, dies, shouts or pings.

If this is set to false, DiscordConnector will not keep a record of number of times each player does something it alerts to.

If this is false, it takes precedent over the "Send leader board updates" setting and no leader boards will get sent.

The stat collection database uses the [LiteDB](https://www.litedb.org/) library and if you are so inclined they offer a database gui which you can use to view/modify this database. (Find the LiteDB Editor on their site.)
:::

### Announce Player Firsts

Type: `Boolean`, default value: `true`

 Disable this setting to disable all extra announcements the first time each player does something. (Overwrites any individual setting.)

### How to discern players in Record Retrieval

Type: `RetrievalDiscernmentMethods`, default value: `PlayerId`

Acceptable values: `PlayerId`, `Name`, `NameAndPlayerId`

 Choose a method for how players will be separated from the results of a record query (used for statistic leader boards). Name: Treat each CharacterName as a separate player PlayerId: Treat each PlayerId as a separate player NameAndPlayerId: Treat each [PlayerId:CharacterName] combo as a separate player

### Send Non-Player Shouts to Discord

Type: `Boolean`, default value: `false`

 Enable this setting to have shouts which are performed by other mods/the server/non-players to be sent to Discord as well. Note: These are still subject to censure by the muted player regex and list.

## Webhook Event Definitions

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

| Event Code | Corresponding Trigger |
| ---------- | --------------------- |
| cronjob    | Output from cronjob   |

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
