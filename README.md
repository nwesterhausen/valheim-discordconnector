# Discord Connector

Connect your Valheim server to a Discord Webhook. ([How to get a webhook](Metadata/HowtoGuide.md))

## Features

- Set your own webhook, lets you configure icon, title, and a target channel
- Enable or Disable any messages

### Supported Message Notificaitons

- Server startup (server starting, loading the world)
- Server started (world loaded, ready to join)
- Server shutting down (server stopping)

### Roadmap

- Messages on player join/leave
- Messages on player ping
- Message on player shouting
- Message when events start/end

### Changelog

See [the changelog](Metadata/CHANGELOG.md).
## Development

To contribute or modify for a PR etc, simply clone this repository. A good set of
steps to follow to prepare your machine for development are listed in the
[BepInEx docs](https://docs.bepinex.dev/master/articles/dev_guide/plugin_tutorial/1_setup.html)
(essentially, make sure you have the .NET SDK setup, an IDE of your choice like
vscode, and that you are prepared to test the compiled plugin after you build it).

### BepInEx Packages

If you are unable to build due to package errors, make sure you've done the setup listed on the
[BepInEx docs](https://docs.bepinex.dev/master/articles/dev_guide/plugin_tutorial/1_setup.html).
Then, you need to add a NuGet.config file in the project directory which contains the following:

```
<?xml version="1.0" encoding="utf-8"?>
<configuration>
  <packageSources>
    <add key="BepInEx" value="https://nuget.bepinex.dev/v3/index.json" />
  </packageSources>
</configuration>
```

This lets dotnet find the packages hosted on bepinex.dev. That file gets created as
part of the template process but is ignored by the .gitignore because that file
can point to local nuget mirrors (may not be present for others) or contain some
sensitive information used for publishing. It's good practice to keep it out of a
public repository.

### Building

Use dotnet to restore and build the project. Post build, the compiled library and its 
dependencies get copied into `bin/DiscordConnector` which enables you to simply copy 
that folder into `$(GameDir)/BepinEx/plugins` for testing.

### Dependencies

For JSON serialization, I chose to use the System.Text.Json library which is part of
the most recent .NET but can be used with .NET 4.8 which is used in this project.
