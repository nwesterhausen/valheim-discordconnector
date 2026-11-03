# Discord Connector

Connect your Valheim server (dedicated or served from the game itself) to a Discord Webhook.
(Find mod documentation on [the official website](https://discord-connector.valheim.games.nwest.one/).)

## Features

- Set your own webhook, lets you configure icon, title, and a target channel
- Enable or Disable any messages
- Set what text gets sent for most messages
- Set more than one message for each type and have one randomly chosen!
- Record number of logins/deaths/pings and flavor the Discord messages
- Works with non-dedicated server (games opened to lan from the client)
- Configure custom leader boards to be sent periodically, listing rankings for any of the tracked stats

### Supported Message Notifications

- Server startup (server starting, loading the world)
- Server started (world loaded, ready to join)
- Server shutting down (server stopping)
- Player join
- Player leave
- Player shouting
- Player pinging
- Player death
- Random events start/pause/resume/end

## Abridged Changelog

## 3.2.0 (Valheim 1.0 fixes)

Built and tested against Valheim 1.0.20 (network version 41).

Fixes: server events

- "Server has started!" and raid/event messages were missing on new worlds and on worlds from before Valheim 1.0
- Players were counted as dying when they reconnected after a crash or respawned without dying (e.g. teleporting home) (#412, #271, #286)
- Leaving was only recorded on a clean quit; timeouts, crashes and kicks now count too
- "Server stopping/stopped" messages were often lost when the server shut down
- On servers hosted from the game (not dedicated), the host's joins, deaths, shouts and leaves weren't counted

Fixes: shouts and pings

- Shouts no longer reached Discord since Valheim 0.219; the server now sees them without needing the client plugin
  when two or more players are online (#338, #319)
- The automatic "I have arrived!" shout is no longer posted, in any game language
- Embedded shout messages ignored the configured message format (#353)
- `%POS%` was left in the text of shouts that didn't come from a player

Fixes: Discord messages

- Messages could arrive out of order; each webhook now sends one message at a time, in order
- Messages are retried when Discord rate-limits them instead of being lost
- Webhook event names in the config now ignore upper/lower case, accept `other` (#382), and accept commas,
  colons or spaces between names (#357)
- A new connection was opened for every message and never cleaned up

Fixes: stats and leaderboards

- Weekly and other date-range leaderboards dropped records when the range crossed a month or year boundary
- "Monday to Sunday" leaderboards covered 13 days when posted on a Sunday
- "Past 7 Days" leaderboards covered 8 days
- Leaderboard 5 posted leaderboard 4's contents
- Time online now follows the "How to discern players" setting, no longer lists players with 0 seconds,
  and correctly splits sessions that cross midnight
- Long leaderboards could go over Discord's 1024-character limit, which made Discord reject the whole post
- Tied players are listed in name order instead of a different order on each post
- The startup log showed leaderboard periods of a day or more without the days (3 days was logged as `00h:00m`)
- Characters whose names end in a space were treated as new players on every join (#190)
- Leaderboards and stats no longer crash when the stats database failed to open (#357)
- Stats are saved with each world save, so less is lost if the server is killed (#214)
- Closing the stats database could throw an error at shutdown (#326)
- Stored positions always read back as (0, 0, 0)
- Stats were written on background threads, which could lose events and hide errors
- Errors in leaderboard and active-player announcements are now logged instead of silently stopping them (#319, #356)
- `vdc.log` could fail with "Sharing violation" when two messages were logged at once

Development

- New in-server test suite (`tests/run-tests.ps1`) with a fake Discord webhook server: 46 tests covering joins,
  deaths, shouts, webhooks, the stats database and leaderboards

The client plugin is on [Thunderstore](https://thunderstore.io/c/valheim/p/nwesterhausen/DiscordConnector_Client/) or can
be downloaded from the [GitHub releases](https://github.com/nwesterhausen/valheim-discordconnector/releases).
