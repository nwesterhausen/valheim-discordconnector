# Toggles

Filename `discordconnector-toggles.cfg`

The toggle configuration is a collection of on/off switches for all the message types and all the extra data that can be sent with them. It's broken up into 3 sections, "Toggles.Messages" which turns on or off each type of message, "Toggles.Positions" which turns on or off sending player coordinates with messages, and "Toggles.Stats" which turns on or off collection of individual stats.

## Toggles.Messages

| Option                      | Default | Description                                                                                |
| --------------------------- | ------- | ------------------------------------------------------------------------------------------ |
| Send Launch Messages        | true    | If true, a message will be sent to Discord when the server launches                        |
| Send Loaded Messages        | true    | If true, a message will be sent to Discord when the server is online and ready for players |
| Send Shutdown Messages      | true    | If true, a message will be sent to Discord when the server shuts down                      |
| Send Player Join Messages   | true    | Set to true to send a message when a player joins the world                                |
| Send Player Leave Messages  | true    | Set to true to send a message when a player leaves the world                               |
| Send Player Death Messages  | true    | Set to true to send a message when a player dies                                           |
| Send Player Shout Messages  | true    | Set to true to send a message when a player shouts                                         |
| Send Player Ping Messages   | true    | Set to true to send a message when a player pings the map                                  |
| Event Start Notifications   | true    | Set to true to send a message when and event starts                                        |
| Event Stop Notifications    | true    | Set to true to send a message when and event stops                                         |
| Event Paused Notifications  | true    | Set to true to send a message when and event is paused                                     |
| Event Resumed Notifications | true    | Set to true to send a message when and event is resumed                                    |

## Toggles.Position

| Option                                        | Default | Description                                                          |
| --------------------------------------------- | ------- | -------------------------------------------------------------------- |
| Send Position with Player Joins               | false   | Set to true to send a player's coordinates when they join the world  |
| Send Position with Player Leaves              | false   | Set to true to send a player's coordinates when they leave the world |
| Send Position with Player Pings               | true    | Set to true to send a player's coordinates when they ping on the map |
| Send Position with Player Shouts              | false   | Set to true to send a player's coordinates when they shout in game   |
| Send Position with Player Deaths              | true    | Set to true to send a player's coordinates when they die             |
| Event Start Messages Position Notifications   | true    | Set to true to send the event coordinates when the event starts      |
| Event Stop Messages Position Notifications    | true    | Set to true to send the event coordinates when the event stops       |
| Event Paused Messages Position Notifications  | true    | Set to true to send the event coordinates when the event is paused   |
| Event Resumed Messages Position Notifications | true    | Set to true to send the event coordinates when the event is resumed  |

## Toggles.Stats

| Option                        | Default | Description                                                  |
| ----------------------------- | ------- | ------------------------------------------------------------ |
| Allow recording player joins  | true    | Set to false to never record players joining in records.json |
| Allow recording player leaves | true    | Set to false to never record players leaving in records.json |
| Allow recording player pings  | true    | Set to false to never record player pings in records.json    |
| Allow recording player shouts | true    | Set to false to never record player shouts in records.json   |
| Allow recording player deaths | true    | Set to false to never record player deaths in records.json   |

## Toggles.PlayerFirsts

| Option                     | Default | Description                                                                                                                    |
| -------------------------- | ------- | ------------------------------------------------------------------------------------------------------------------------------ |
| Send Player Join Messages  | true    | If enabled (and player-first announcements are enabled), will send an extra message on a player's first leave from the server. |
| Send Player Leave Messages | false   | If enabled (and player-first announcements are enabled), will send an extra message on a player's first join to the server.    |
| Send Player Death Messages | true    | If enabled (and player-first announcements are enabled), will send an extra message on a player's first death."                |
| Send Player Shout Messages | false   | If enabled (and player-first announcements are enabled), will send an extra message on a player's first ping.                  |
| Send Player Ping Messages  | false   | If enabled (and player-first announcements are enabled), will send an extra message on a player's first shout.                 |
