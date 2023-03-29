# Toggles

Filename `discordconnector-toggles.cfg`

The toggle configuration is a collection of on/off switches for all the message types and all the extra data that can be sent with them. It's broken up into 3 sections, "Toggles.Messages" which turns on or off each type of message, "Toggles.Positions" which turns on or off sending player coordinates with messages, and "Toggles.Stats" which turns on or off collection of individual stats.

## Toggles.DebugMessages

### Debug Message for Every Event Check

Type: `Boolean`, default value: `false`

 If enabled, this will write a log message at the DEBUG level every time it checks for an event (every 1s).

### Debug Message for Every Event Player Location Check

Type: `Boolean`, default value: `false`

 If enabled, this will write a log message at the DEBUG level every time the EventWatcher checks players' locations.

### Debug Message for Every Event Change

Type: `Boolean`, default value: `false`

 If enabled, this will write a log message at the DEBUG level when a change in event status is detected.

### Debug Message for HTTP Request Responses

Type: `Boolean`, default value: `false`

 If enabled, this will write a log message at the DEBUG level with the content of HTTP request responses. Nearly all of these requests are when data is sent to the Discord Webhook.

## Toggles.Messages

### Debug Message for Database Methods

Type: `Boolean`, default value: `false`

 If enabled, this will write a log message at the DEBUG level with logs generated while executing database methods.

### Server Launch Notifications

Type: `Boolean`, default value: `true`

 If enabled, this will send a message to Discord when the server launches.

### Server Loaded Notifications

Type: `Boolean`, default value: `true`

 If enabled, this will send a message to Discord when the server has loaded the map and is ready for connections.

### Server Stopping Notifications

Type: `Boolean`, default value: `true`

 If enabled, this will send a message to Discord when the server begins shut down.

### Server Shutdown Notifications

Type: `Boolean`, default value: `true`

 If enabled, this will send a message to Discord when the server has shut down.

### Server World Save Notifications

Type: `Boolean`, default value: `true`

 If enabled, this will send a message to Discord when the server saves the world.

### Chat Shout Messages Notifications

Type: `Boolean`, default value: `true`

 If enabled, this will send a message to Discord when a player shouts on the server.

### Ping Notifications

Type: `Boolean`, default value: `true`

 If enabled, this will send a message to Discord when a player pings on the map.

### Player Join Notifications

Type: `Boolean`, default value: `true`

 If enabled, this will send a message to Discord when a player joins the server.

### Player Death Notifications

Type: `Boolean`, default value: `true`

 If enabled, this will send a message to Discord when a player dies on the server.

### Player Leave Notifications

Type: `Boolean`, default value: `true`

 If enabled, this will send a message to Discord when a player leaves the server.

### Event Start Notifications

Type: `Boolean`, default value: `true`

 If enabled, this will send a message to Discord when a random event starts on the server.

### Event Stop Notifications

Type: `Boolean`, default value: `true`

 If enabled, this will send a message to Discord when a random event stops on the server.

### Event Paused Notifications

Type: `Boolean`, default value: `true`

 If enabled, this will send a message to Discord when a random event is paused due to players leaving the area.

## Toggles.PlayerFirsts

### Event Resumed Notifications

Type: `Boolean`, default value: `true`

 If enabled, this will send a message to Discord when a random event is resumed.

### Send a Message for the First Death of a Player

Type: `Boolean`, default value: `true`

 If enabled, this will send an extra message on a player's first death.

### Send a Message for the First Join of a Player

Type: `Boolean`, default value: `true`

 If enabled, this will send an extra message on a player's first join to the server.

### Send a Message for the First Leave of a Player

Type: `Boolean`, default value: `false`

 If enabled, this will send an extra message on a player's first leave from the server.

### Send a Message for the First Ping of a Player

Type: `Boolean`, default value: `false`

 If enabled, this will send an extra message on a player's first ping.

## Toggles.Position

### Send a Message for the First Shout of a Player

Type: `Boolean`, default value: `false`

 If enabled, this will send an extra message on a player's first shout.

### Include POS With Player Join

Type: `Boolean`, default value: `false`

 If enabled, this will include the coordinates of the player when they join.

### Include POS With Player Leave

Type: `Boolean`, default value: `false`

 If enabled, this will include the coordinates of the player when they leave.

### Include POS With Player Death

Type: `Boolean`, default value: `true`

 If enabled, this will include the coordinates of the player when they die.

### Ping Notifications Include Position

Type: `Boolean`, default value: `true`

 If enabled, includes the coordinates of the ping.

### Chat Shout Messages Position Notifications

Type: `Boolean`, default value: `false`

 If enabled, this will include the coordinates of the player when they shout.

### Event Start Messages Position Notifications

Type: `Boolean`, default value: `true`

 If enabled, this will include the coordinates of the random event when the start message is sent.

### Event Stop Messages Position Notifications

Type: `Boolean`, default value: `true`

 If enabled, this will include the coordinates of the random event when the stop message is sent.

### Event Paused Messages Position Notifications

Type: `Boolean`, default value: `true`

 If enabled, this will include the coordinates of the random event when the paused message is sent.

## Toggles.Stats

### Event Resumed Messages Position Notifications

Type: `Boolean`, default value: `true`

 If enabled, this will include the coordinates of the random event when the resumed message is sent.

### Collect and Send Player Death Stats

Type: `Boolean`, default value: `true`

 If enabled, will allow collection of the number of times a player has died.

### Collect and Send Player Join Stats

Type: `Boolean`, default value: `true`

 If enabled, will allow collection of how many times a player has joined the game.

### Collect and Send Player Leave Stats

Type: `Boolean`, default value: `true`

 If enabled, will allow collection of how many times a player has left the game.

### Collect and Send Player Ping Stats

Type: `Boolean`, default value: `true`

 If enabled, will allow collection of the number of pings made by a player.

### Collect and Send Player Shout Stats

Type: `Boolean`, default value: `true`

 If enabled, will allow collection of the number of times a player has shouted.
