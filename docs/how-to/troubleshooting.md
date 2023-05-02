# Troubleshooting

Discord Connector provides a few ways to troubleshoot.

## Enable Debug Logging

It's possible to get debug messages from Discord Connector in 2 ways. You can enable the DEBUG log level in the BepInEx configuration. Or you can change the [Main Config - Log Debug Messages](/config/main#log-debug-messages) to true to have Discord Connector send its DEBUG level logs to the normal (INFO) channel. This setting will cause the extra logs from Discord Connector to show up in the console and in the `LogOutput.log` file.

## Check Configuration

At startup, Discord Connector will create a `config-dump.json` file in its configuration directory. This file is JSON formatted configuration keys and values, as Discord Connector was able to parse them. It can be a sanity check that the settings are what you expect them to be, and its also a single file to share instead of having to share the handful of different config files.

## Open the Database in LiteDB.Studio

The `records.db` database is a LiteDB database. It is used to store records of players joining, leaving, dying and anything else that you have enabled in the [Toggles.Stats](/config/toggles.stats) section.

The [LiteDB Studio](https://github.com/mbdavid/LiteDB.Studio/releases/latest) tool will let you open the `records.db` file and verify that it has data in it. In some cases, Discord Connector may be disqualifying data to write or have some other error which prevents it from updating the database.

:::info `records.db` and `records.db.log`
Take care to notice that there may be two similarly named files. This is because when LiteDB is operating (while the server is running) it will use the "log" file to keep track of what has happened. After the server is shut down, it should clean up and have an up-to-date `records.db` file and an empty log (if the log file is not removed on its own).

If you plan to look at the `records.db` file, you should shut down your server, and then open the `records.db` file (or you can copy it to another location and open from there).
:::
