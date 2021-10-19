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

=== "200"

    Sent if you provide a valid Authorization header, and the server is ready to handle requests.

    ```json
    {
        "status": "accepted",
        "version": "games.nwest.valheim.discordconnector-2.0.0"
    }
    ```

=== "400"

    Sent if you send a malformed command, either with bad JSON formatting or missing some required part of the command object.

    ```json
    {
        "status": "error",
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
        "status": "unauthorized",
        "version": "games.nwest.valheim.discordconnector-2.0.0"
    }
    ```

=== "500"

    Sent if the command failed to execute

    ```json
    {
        "status": "error",
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
        "status": "error",
        "message": "command is not fully implemented yet",
        "version": "games.nwest.valheim.discordconnector-2.0.0"
    }
    ```

## Command Detail

### Leaderboard

You may request a leaderboard for various stats about the players on your server.

!!! Attention

    Not implemented yet.

!!! tldr "Request Body"

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
    | ranking     | 3                               |
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

!!! Attention

    Not implemented yet.

!!! Warning

    If you modified the config to disable the Discord Bot integration, this will turn off the listening API endpoint.

!!! tldr "Request Body"

    ```json
    {
        "command": "reload"
    }
    ```

### Save the Game

You may request that the game be saved.

!!! Attention

    Not implemented yet.

!!! tldr "Request Body"

    ```json
    {
        "command": "save"
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

!!! tldr "Request Body"

    ```json
    {
        "command": "status"
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
