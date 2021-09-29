# valheim-discordconnector

A plugin to connect a Valheim server to a discord webhook.

## Development

### BepInEx Packages

If you clone this and try to work on it, you may need to add a NuGet.config file
in the project directory which contains the following:

```
<?xml version="1.0" encoding="utf-8"?>
<configuration>
  <packageSources>
    <add key="BepInEx" value="https://nuget.bepinex.dev/v3/index.json" />
  </packageSources>
</configuration>
```

### Publicized Assemblies

(Not sure if needed atm, but publicizer part of
[Bepinex-Tools](https://github.com/MrPurple6411/Bepinex-Tools) was used in other
plugins which tied into the game.)
