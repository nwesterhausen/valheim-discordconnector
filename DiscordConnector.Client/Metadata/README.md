# Discord Connector Client

This is the client-side of the Discord Connector mod for Valheim. This mod basically handles sending messages to the
server-side mod, which then sends them to Discord.

## Features

- Enables letting the server know about shouts, pings, regular chat messages, and whispers.

## Abridged Changelog

### Version 1.0.0

Initial release of the client plugin.

The client plugin has only two parts, an RPC registration which lets it talk to the server plugin, and a patch to the
chat message method which sends a copy of the message to the server plugin.