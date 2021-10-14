## Randomized Messages

To use the random message feature, when editing the config file, separate each message with a semicolon. This does mean you cannot use a semicolon as part of your message. Raise an issue on the github if you have a better resolution.

### Using r2modman to edit

Use r2modman to modify the config value

1. Go to "Config Editor"
2. Expand the list item for `BepInEx\config\games.nwest.valheim.discordconnector.cfg`
3. Click "Edit Config"
4. Modify the message that you want to have as a multi-message (where one is randomly chosen each time)

![example modification](/img/howto-6.png)

### Manually Editing the Config File

In the `games.nwest.valheim.discordconnector.cfg` file, a multi-message string would look like this:

```ini
[Notification Content Settings]

## Set the message that will be sent when the server starts up.
## If you want to have this choose from a variety of messages at random, separate each message with a semicolon ';'
## Random choice example: 'Server is starting;Server beginning to load'
# Setting type: String
# Default value: Server is starting up.
Server Launch Message = Server is starting up.;Server Launching!;Server taking off.
```