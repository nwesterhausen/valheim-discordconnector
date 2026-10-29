# Tests

DiscordConnector is tested **inside a real Valheim dedicated server**. A small test plugin
(`DiscordConnector.Tests`) loads next to the mod, makes pretend players, triggers joins,
deaths, shouts, leaderboards and so on, and checks what ends up in the records database and
what gets sent to the webhook.

The webhook points at `tools/WebhookCapture`, a tiny local server that stands in for Discord
and logs every payload. It can also forward everything to a real Discord channel so you can
see the messages.

## One-time setup (Windows)

1. Install the dedicated server with SteamCMD:
   `steamcmd +force_install_dir <server dir> +login anonymous +app_update 896660 validate +quit`
2. Copy the contents of
   [BepInExPack_Valheim](https://thunderstore.io/c/valheim/p/denikson/BepInExPack_Valheim/)'s
   `BepInExPack_Valheim` folder into the server folder.
3. Set `ValheimGamePath` in `Environment.props` to your Valheim **client** install, and add
   `<BepInExPath>` pointing at the server's `BepInEx` folder. The build needs the client's
   `valheim_Data` folder and a BepInEx install. No client? Build against the server instead: run
   `mklink /J valheim_Data valheim_server_Data` in the server folder and set `GamePath` to the server
   folder (this is how GitHub Actions runs the tests, see `.github/actions/setup-valheim`).
4. .NET SDK 8+ (the capture tool targets net10.0).

## Running

```powershell
.\tests\run-tests.ps1 -ServerPath C:\valheim-server
.\tests\run-tests.ps1 -ServerPath C:\valheim-server -Slow        # includes the ~1 minute timer test
.\tests\run-tests.ps1 -ServerPath C:\valheim-server -Embeds      # with "fancier" embed messages
.\tests\run-tests.ps1 -ServerPath C:\valheim-server -DiscordWebhook https://discord.com/api/webhooks/...
```

The script builds everything, starts a fresh world, runs the tests, shuts the server down and
prints a summary. The exit code is the number of failed checks. Logs and results are written to
`tests/out/`.

## What is and isn't covered

Covered: join/leave/death/shout/ping handling, the records database, date-range queries,
time online, leaderboards, server lifecycle messages, webhook delivery, and that every Harmony
patch applies to the installed game version.

Not covered: what a real game client sends. Pretend players skip the network, so a change in
what the client sends (for example when it sends the death RPC) is only caught by joining the
test server with a real client.
