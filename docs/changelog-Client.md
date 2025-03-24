# Client Changelog

Here will be the record of changes for the client plugin.

## Version 1.0.1

### Fixes

- Issue where clients were sending all messages "heard" on the client to the server, resulting in duplication of messages
- Issue where the log file was not being created in the correct location (and causing the plugin to fail on clients)

## Version 1.0.0

Initial release of the client plugin.

The client plugin has only two parts, an RPC registration which lets it talk to the server plugin, and a patch to the
chat message method which sends a copy of the message to the server plugin.