
# Webhook Event Definitions

Here is a list of every accepted event for inclusion in the 'webhook event' settings. These correspond pretty directly to the configurations in the messages config.

::: warning Specifying Events ≠ Enabling Messages
Adding an event to be send for a webhook does not enable Discord Connector to send that event. It only tells Discord Connector *where* to send the event message.

Messages for these events have to be enabled in the toggles config file (as they were previously). Any of these messages which are enabled by default are notated as such in the 'Enabled by Default' column.

Take note that by default, the webhooks are set to 'ALL' which takes any messages which happen and sends them.
:::

## General Events

These include player, server, and random events.

:::tip Special Case Events
Take note that some of these events are subsets of the [special cases](/config/webhook.events.html#special-case-events)
:::

| Event Code       | Corresponding Trigger                         | Enabled by Default |
| ---------------- | --------------------------------------------- | ------------------ |
| serverLaunch     | Server begins starting up                     | Yes                |
| serverStart      | World has been loaded                         | Yes                |
| serverStop       | Server beings stopping                        | Yes                |
| serverShutdown   | Server has stopped                            | Yes                |
| serverSave       | The world has been saved                      | Yes                |
| serverNewDay     | A new day begins on the server                | Yes                |
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

## Leaderboard Events

These are events which represent sending the leaderboards

| Event Code    | Corresponding Trigger          | Enabled by Default |
| ------------- | ------------------------------ | ------------------ |
| activePlayers | The active players leaderboard | No                 |
| leaderboard1  | The custom leaderboard 1       | No                 |
| leaderboard2  | The custom leaderboard 2       | No                 |
| leaderboard3  | The custom leaderboard 3       | No                 |
| leaderboard4  | The custom leaderboard 4       | No                 |
| leaderboard5  | The custom leaderboard 5       | No                 |

## 3rd Party Events

It's possible for other mods to hook into Discord Connector to send messages to Discord. As more get added to this list,
they will be able to be specifically attached to a certain webhook.

| Event Code | Corresponding Trigger |
| ---------- | --------------------- |
| cronjob    | Output from cronjob   |

## Special Case Events

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
