# Discord Connector

Connect your Valheim server (dedicated or served from the game itself) to a Discord Webhook.
(Find mod documentation on [the official website](https://discordconnector.valheim.nwest.games/).)

## Features

- Set your own webhook, lets you configure icon, title, and a target channel
- Enable or Disable any messages
- Set what text gets sent for most messages
- Set more than one message for each type and have one randomly chosen!
- Record number of logins/deaths/pings and flavor the Discord messages
- Works with non-dedicated server (games opened to lan from the client)

### Supported Message Notificaitons

- Server startup (server starting, loading the world)
- Server started (world loaded, ready to join)
- Server shutting down (server stopping)
- Player join
- Player leave
- Player shouting
- Player pinging
- Player death
- Random events start/pause/resume/end

### Roadmap

See the [current roadmap](https://github.com/nwesterhausen/valheim-discordconnector/projects/1) as a Github project.

- Fancier Discord messages
- Discord bot integration
- Multiple webhook support
- More statistics able to be sent

## Changelog

### Version 1.5.3

Fixes:

- Leaderboard interval was half of what was configured (now is properly minutes)

### Version 1.5.2

Fixes:

- Highest and Lowest leaderboards were not checking the correct tables
- Configurable retrieval strategy for all records (either SteamID, PLayer Name, or both) -- always returns player names

Due to how records.json recorded stats and the LiteDB, you will not be able to use the old records with strategies
involving the SteamID because prior to 1.5.0 we were not recording the SteamID with the record.

### Version 1.5.1

Fixes:

- Toggles for the bottom n players leaderboards (inverse ranked leaderboards)

### Version 1.5.0

Features:

- Using LiteDB for record storage.

Because of how unreliable storing the records in a "roll-your-own"
database with a JSON file was, and because of the increased flexibility
in what could be stored, I've changed the storage system for the
recorded player stats to use LiteDB. Currently this means records for
join/leave/death/shout/ping will be timestamped, include the position of
the event, have the player name, and the player's steamid. Hopefully
adding this additional information will allow for more customization
options for the users of this mod.

It is set up to do a migration on first load of the updated plugin, the
steps it follows for that is:

	1. check if records.json (or configured name) exists
	2. read all records from the file
	3. parse the records
	4. loop through all the records and add them to the database

		Records added this way will have position of zero and a
		steamid of 1.

	5. move the records.json file to records.json.migrated

If you don't want to have it auto-migrate the records, rename your
records.json or delete it. If the name does not match exactly it will
not migrate the data.

For the migration steps, it will be outputting log information (at INFO
level) with how many records were migrated and which steps completed.

- Ranked Lowest Player Leaderbaord

Added an inverse of the Top Player leaderboard.

- Custom leaderboard heading messages

Added configuration for the messages sent at the top of the leaderboard
messages.

- The variable `%PUBLICIP%` can be used in _any_ message configuration
  now.

Full changelog history available on the
[Github repository](https://github.com/nwesterhausen/valheim-discordconnector/blob/main/Metadata/CHANGELOG.md)
or [discordconnector.valheim.nwest.games](https://discordconnector.valheim.nwest.games/changelog).