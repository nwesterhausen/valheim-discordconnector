# Discord Bot Endpoint

Valheim Discord Connector has an API which may allow Discord Bots to interact with the Valheim server.

!!! tip "Beta-version download"

    The bot integration is not in the live release version of Valheim Discord Connector.

    <a href="/DiscordConnector.zip" class="md-button md-button--primary">Download Beta Plugin</a>

    Built on: 2:17pm Oct 19th, 2021

## Overview

### Endpoint/url

`http://{your-server-ip}:{configured-port}/discord`

### Request Parameters

You will find the authorization token in `games.nwest.valheim.discordconnector-bot.cfg`

| Parameter            | Value                                  |
| -------------------- | -------------------------------------- |
| Authorization Header | $myAuthToken                           |
| Content Type         | application/json                       |
| Request Type         | POST                                   |
| Body                 | JSON data with details of the command. |

### Possible Responses

For all commands, the following responses are possible. Every response will include the version of Valheim Discord Connector that responded to your command.

!!! important "Status Description"

    The status value returned gives some info for the status of the server. It tells you if the server is dedicated or not, its current state, and the loaded world.

    ```c#
    $"{(RunningHeadless ? "Dedicated Server" : "Client-run Server")}; {ServerState}; {ServerWorld}";
    ```

    | ServerState | Description |
    | -- | -- |
    | not started | Plugin has been loaded but game has not been started yet |
    | awake | Plugin has been called awake |
    | running | Load world has completed |

    If the server is not in the "running" state, it will only be able to handle the following requests:

    - config
    - status

    For all other requests before the server is running, it will return a 

=== "200"

    Sent if you provide a valid Authorization header, and the server is ready to handle requests.

    ```json
    {   
        "statusCode": 200,
        "status": "Dedicated Server; running; Dedicated",
        "version": "games.nwest.valheim.discordconnector-1.4.3"
    }
    ```

=== "400"

    Sent if you send a malformed command, either with bad JSON formatting or missing some required part of the command object.

    ```json
    {   
        "statusCode": 400,
        "status": "Dedicated Server; running; Dedicated",
        "message": "bad request",
        "version": "games.nwest.valheim.discordconnector-2.0.0"
    }
    ```

    !!! Info

        There will be a log message on the server relating to this error which provides more details on the exact reason for failure.

=== "401"

    Sent if you provide an invalid Authorization header.

    ```json
    {
        "statusCode": 401,
        "status": "check your authorization header",
        "version": "games.nwest.valheim.discordconnector-2.0.0"
    }
    ```

=== "405"

    Sent if the command is not available at this moment (e.g. before the server is running).

    ```json
    {
        "statusCode": 405,
        "status": "Dedicated Server; awake; ",
        "message": "command not available until the server is running",
        "version": "games.nwest.valheim.discordconnector-2.0.0"
    }
    ```

=== "500"

    Sent if the command failed to execute

    ```json
    {
        "statusCode": 500,
        "status": "Dedicated Server; running; Dedicated",
        "message": "command failed to execute",
        "version": "games.nwest.valheim.discordconnector-2.0.0"
    }
    ```

    !!! Info

        There will be a log message on the server relating to this error which provides more details on the exact reason for failure.

=== "501"

    Sent if the command is not implemented yet

    ```json
    {
        "statusCode": 501,
        "status": "Dedicated Server; running; Dedicated",
        "message": "command is not fully implemented yet",
        "version": "games.nwest.valheim.discordconnector-2.0.0"
    }
    ```

## Command Detail

### Config

You may request the current running configuration for Discord Connector on the server.

=== "Request Body"

    ```json
    {
        "command": "config"
    }
    ```

=== "Sample Response"

    ```json
    {
        "Config.Main": {
            "discord": {
                "webhook": "unset", # can also say REDACTED if it is set
                "fancierMessages": "False",
                "ignoredPlayers": "",
                "botWebhookEnabled": "True"
            },
            "periodicLeaderboardEnabled": "True",
            "periodicLeaderboardPeriodSeconds": 1,
            "colectStatsEnabled": "True",
            "sendPositionsEnabled": "True",
            "announcePlayerFirsts": "True",
            "numberRankingsListed": "3"
        },
        "Config.Messages": {
            "Messages.Server": {
                "launchMessage": "Server is starting up.",
                "startMessage": "Server has started!",
                "stopMessage": "Server is stopping.",
                "shutdownMessage": "Server has stopped!",
                "savedMessage": "The world has been saved."
            },
            "Messages.Player": {
                "joinMessage": "%PLAYER_NAME% has joined.",
                "deathMessage": "%PLAYER_NAME% has died.",
                "leaveMessage": "%PLAYER_NAME% has left.",
                "pingMessage": "%PLAYER_NAME% pings the map.",
                "shoutMessage": "%PLAYER_NAME% shouts **%SHOUT%**."
            },
            "Messages.PlayerFirsts": {
                "joinMessage": "%PLAYER_NAME% has joined.",
                "deathMessage": "%PLAYER_NAME% has died.",
                "leaveMessage": "%PLAYER_NAME% has left.",
                "pingMessage": "%PLAYER_NAME% pings the map.",
                "shoutMessage": "%PLAYER_NAME% shouts **%SHOUT%**."
            },
            "Messages.Events": {
                "eventStartMessage": "**Event**: %EVENT_MSG%",
                "eventPausedMessage": "**Event**: %EVENT_END_MSG% -- for now! (Currently paused due to no players in the event area.)",
                "eventResumedMessage": "**Event**: %EVENT_START_MSG%",
                "eventStopMessage": "**Event**: %EVENT_MSG%"
            }
        },
        "Config.Toggles": {
            "Toggles.Messages": {
                "launchMessageEnabled": "True",
                "loadedMessageEnabled": "True",
                "stopMessageEnabled": "True",
                "shutdownMessageEnabled": "True",
                "chatShoutEnabled": "True",
                "chatPingEnabled": "True",
                "playerJoinEnabled": "True",
                "playerLeaveEnabled": "True",
                "playerDeathEnabled": "True",
                "eventStartEnabled": "True",
                "eventPausedEnabled": "True",
                "eventStoppedEnabled": "True",
                "eventResumedEnabled": "True"
            },
            "Toggles.Position": {
                "chatShoutPosEnabled": "False",
                "chatPingPosEnabled": "True",
                "playerJoinPosEnabled": "False",
                "playerLeavePosEnabled": "False",
                "playerDeathPosEnabled": "True",
                "eventStartPosEnabled": "True",
                "eventStopPosEnabled": "True",
                "eventPausedPosEnabled": "True",
                "eventResumedPosEnabled": "True"
            },
            "Toggles.Stats": {
                "statsDeathEnabled": "True",
                "statsJoinEnabled": "True",
                "statsLeaveEnabled": "True",
                "statsPingEnabled": "True",
                "statsShoutEnabled": "True"
            },
            "Toggles.Leaderboard": {
                "leaderboardDeathEnabled": "True",
                "leaderboardPingEnabled": "False",
                "leaderboardShoutEnabled": "False",
                "leaderboardSessionEnabled": "False"
            },
            "Toggles.Leaderboard.Highest": {
                "sendMostSessionLeaderboard": "False",
                "sendMostPingLeaderboard": "False",
                "sendMostDeathLeaderboard": "True",
                "sendMostShoutLeaderboard": "False"
            },
            "Toggles.Leaderboard.Lowest": {
                "sendLeastSessionLeaderboard": "False",
                "sendLeastPingLeaderboard": "False",
                "sendLeastDeathLeaderboard": "True",
                "sendLeastShoutLeaderboard": "False"
            },
            "Toggles.PlayerFirsts": {
                "announceFirstDeathEnabled": "True",
                "announceFirstJoinEnabled": "True",
                "announceFirstLeaveEnabled": "False",
                "announceFirstPingEnabled": "False",
                "announceFirstShoutEnabled": "False"
            },
            "Toggles.DebugMessages": {
                "debugEveryPlayerPosCheck": "False",
                "debugEveryEventCheck": "False",
                "debugEventChanges": "False",
                "debugHttpRequestResponses": "False"
            }
        },
        "Config.Bot": {
            "discordBotAuthorization": "REDACTED",
            "discordBotPort": "20736"
        },
        "Config.Variables": {
            "userVar": "",
            "userVar1": "",
            "userVar2": "",
            "userVar3": "",
            "userVar4": "",
            "userVar5": "",
            "userVar6": "",
            "userVar7": "",
            "userVar8": "",
            "userVar9": ""
        }
    }
    ```

### Leaderboard

You may request a leaderboard for various stats about the players on your server.

!!! Attention

    Not implemented yet.

=== "Request Body"

    ```json
    {
        "command": "leaderboard",
        "data": {
            "type": "$type",
            "number": "$number",
            "category": "$category"
        }
    }
    ```

=== "Sample Response"

    (Not yet implemented).

    Probably will be some kind of json object depending on the requested type of leaderboard.

**Variable Definitions**

=== "$type"

    `Type: string`

    The type of leaderboard. The following are valid:

    | Type    | Explanation                                           |
    | ------- | ----------------------------------------------------- |
    | ranking | The top players for the specified $category           |
    | top     | The highest ranked player for the specified $category |
    | bottom  | The lowest ranked player for the specified $category  |

=== "$number"

    `Type: number (integer)`

    Used when requesting a leaderboard that has a requirement for a certain number of people.

    Default values:

    | Leaderboard | Default value or not applicable |
    | ----------- | ------------------------------- |
    | ranking     | (what is specified in config)                               |
    | top         | N/A                             |
    | bottom      | N/A                             |

=== "$category"

    `Type: string`

    The category for the leaderboard.

    | Category | Explanation                                  |
    | -------- | -------------------------------------------- |
    | session  | Number of play session (joins to the server) |
    | death    | Number of deaths on the server               |
    | ping     | Number of pings performed on the server      |
    | shout    | Number of shouts made on the server          |

### Reload Config

You may request that the Valheim Discord Connector config be reloaded.

!!! Warning

    If you modified the config to disable the Discord Bot integration, this will turn off the listening API endpoint.

=== "Request Body"

    ```json
    {
        "command": "reload"
    }
    ```
    
=== "Response"

    ```json
    {   
        "statusCode": 200,
        "status": "Dedicated Server; running; Dedicated",
        "version": "games.nwest.valheim.discordconnector-1.4.3",
        "message": "configuration reloaded"
    }
    ```

### Save the Game

You may request that the game be saved.

=== "Request Body"

    ```json
    {
        "command": "save"
    }
    ```
    
=== "Response"

    ```json
    {   
        "statusCode": 200,
        "status": "Dedicated Server; running; Dedicated",
        "version": "games.nwest.valheim.discordconnector-1.4.3",
        "message": "game saved"
    }
    ```

### Send Chat Message

You may send a message that will be shouted in the server.

!!! Attention

    Not implemented yet.

!!! tldr "Request Body"

    ```json
    {
    "command": "chat",
    "data": {
        "username": "$username",
        "content": "$message"
        }
    }
    ```

**Variable Definitions**

=== "$username"

    `Type: string`

    The name to be displayed alongside the message
    
=== "$message"

    `Type: string`

    The message to be shouted to all players

### Status

You may check to see if you are using the proper authorization header and if the server is listening and ready to respond.

This will return a short response with details on your request.

=== "Request Body"

    ```json
    {
        "command": "status"
    }
    ```
    
=== "Response"

    ```json
    {   
        "statusCode": 200,
        "status": "Dedicated Server; running; Dedicated",
        "version": "games.nwest.valheim.discordconnector-1.4.3"
    }
    ```

### Stop the Server

You may request to shutdown the server.

!!! Attention

    Not implemented yet.

!!! tldr "Request Body"

    ```json
    {
        "command": "shutdown"
    }
    ```
