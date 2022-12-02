
# Discord Connector Configuration

DiscordConnector uses multiple configuration files to make find the setting you want to change faster, and hopefully easier. Since 2.1.0, DiscordConnector puts all its config files into a single directory: `BePinEx/config/games.nwest.valheim.discordconnector` directory. The configuration is divided into the following files:

| Configuration File                  | Details                          | Purpose                                                                              |
| ----------------------------------- | -------------------------------- | ------------------------------------------------------------------------------------ |
| `discordconnector.cfg`              | [Details](/config/main)          | Master settings, including the main webhook and turning settings on or off globally  |
| `discordconnector-messages.cfg`     | [Details](/config/messages)      | The messages used/chosen from when DiscordConnector sends messages to Discord        |
| `discordconnector-toggles.cfg`      | [Details](/config/toggles)       | Used to turn individual notifications and/or their included extra details on or off. |
| `discordconnector-variables.cfg`    | [Details](/config/variables)     | Used to assign strings to variables which can be referenced any messages             |
| `discordconnector-leaderBoards.cfg` | [Details](/config/leader-boards) | Define custom leader boards to be periodically sent to Discord                       |

The directory also has these files:

| File                | Purpose                                                                                         |
| ------------------- | ----------------------------------------------------------------------------------------------- |
| `config-debug.json` | A dump of your DiscordConnector configuration as JSON (the webhook info is redacted)            |
| `records.db`        | The LiteDB database used by DiscordConnector to track players joining/leaving/dying/pinging/etc |
