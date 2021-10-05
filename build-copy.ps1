dotnet format
dotnet build

Copy-Item `
    -Path "$env:userprofile\source\repos\DiscordConnector\bin\DiscordConnector\plugins\DiscordConnector.dll" `
    "$env:userprofile\scoop\persist\steam\steamapps\common\Valheim dedicated server\BepInEx\plugins\nwesterhausen-DiscordConnector" 