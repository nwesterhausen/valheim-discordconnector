# Supported Bot Commands

A bit of API documentation for what the supported bot commands are, and what the results will be when they are sent.

## Discord Bot Endpoint

If the [`[Main Config:Enable Discord Bot Integration]`](/configuration-details/#main-config) setting is enabled, this plugin will open a port (defined by [`[Discord Bot Integration:Listening Port]`](/configuration-details/#discord-bot-integration)) and listen for incoming http POST messages at `http://{all IP addresses available to the server}:{Listening Port}/discord/`. Most of the commands require an Authorization header, which is randomly generated when the server starts up, but you can also set your own in the config at [`[Discord Bot Integration:Authorization Header]`](/configuration-details/#discord-bot-integration)