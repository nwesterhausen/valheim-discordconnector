# Discord Bot Integration Overview

A bit of API documentation for what the supported bot commands are, and what the results will be when they are sent.

## Discord Bot Endpoint

If the [`[Main Config:Enable Discord Bot Integration]`](/configuration-details/#main-config) setting is enabled, this plugin will open a port (defined by [`[Discord Bot Integration:Listening Port]`](/configuration-details/#discord-bot-integration)) and listen for incoming HTTP POST messages at `http://0.0.0.0:{Listening Port}/discord/`. Most of the commands require an Authorization header, which is randomly generated when the server starts up, but you can also set your own in the config at [`[Discord Bot Integration:Authorization Header]`](/configuration-details/#discord-bot-integration)

## Connection Diagrams

### Discord Bot Command Sequence

A high level description of the steps taken on the plugin-side when receiving a command.

<div class="mermaid">
%%{init: {'theme': 'base', 'themeVariables': {'noteBkgColor':'#FFF176', 'noteTextColor':'#78909C', 'primaryColor': '#3949AB','lineColor': '#4287ff', 'primaryTextColor': '#78909C'}}}%%
sequenceDiagram
    Discord Bot->>Server Plugin: Here's a command.
    Note over Server Plugin: Verifies Authorization Header
    Note over Server Plugin: Validates the body is parsable/complete.
    Note over Server Plugin: Validates Command Syntax
    Note over Server Plugin: Verifies the Configuration Permits the Command
    Note over Server Plugin: Performs any actions required by the command.
    Server Plugin->>Discord Bot: Here's the data from the command.
</div>

### Discord User Command Sequence

How a possible interaction may go to force the server to save the game.

<div class="mermaid">
%%{init: {'theme': 'base', 'themeVariables': {'noteBkgColor':'#FFF176', 'noteTextColor':'#78909C', 'primaryColor': '#3949AB','lineColor': '#4287ff', 'primaryTextColor': '#78909C'}}}%%
%%{init: {'theme': 'base', 'themeVariables': {'noteBkgColor':'#FFF176', 'noteTextColor':'#78909C', 'primaryColor': '#3949AB','lineColor': '#4287ff', 'primaryTextColor': '#78909C'}}}%%
sequenceDiagram
    Participant Discord User
    Participant Discord Bot
    Participant Server Plugin
    Discord User->>Discord Bot: /save-game
    Discord Bot ->>Server Plugin: Command "save the game"
    Discord Bot ->>Discord User: "Issuing save command."
    Note over Server Plugin: Saves the game.
    Server Plugin ->>Discord Bot: Result "the game was saved"
    Discord Bot ->> Discord User: "The game has been saved."
</div>