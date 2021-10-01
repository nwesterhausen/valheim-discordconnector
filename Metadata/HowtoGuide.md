# How-to Guides

- [How to get a Discord webhook](#how-to-get-a-discord-webhook)

Configuration editing (make sure the game has run once with the plugin enabled,
otherwise the config file won't be generated.)

- [Edit the config with R2ModMan](#editing-the-config-using-r2modman)
- [Edit the config manually](#editing-the-config-manually)

## How to get a Discord webhook

1. Make sure you have admin permission on the Discord server
2. Find or create the text channel you want to have messages sent to
3. Click the settings button for the channel

    ![channel settings button](../Images/howto-0.png)

4. Go to the "Integrations" section

    ![integrations section](../Images/howto-1.png)

5. Click "Create Webhook" next to the Webhooks heading. (This will create the webhook and take you into a settings page for this channel's webhooks)
6. By default, it is named "Spidey Bot" but you should modify the name and avatar for the webhook to your liking.

    ![edit the webhook settings](../Images/howto-2.png)

7. After modifying the webhook name and avatar, make sure to click save to save your changes.
8. Click on "Copy Webhook URL" to copy the webhook url which needs to go into the config file for DiscordConnector.

    ![copy webhook after saving changes](../Images/howto-3.png)

## Editing the config using R2Modman

With R2Modman open to the profile you are using for the server, 

1. Go to Config Editor
2. Click on the DiscordConnector config
3. Paste the webhook URL into the Webhook URL field

    ![config editor section](../Images/howto-4.png)

4. Click Save

## Editing the config manually

1. Navigate to `$(GameDirectory)/BepInEx/config`
2. Edit the `games.nwest.valheim.discordconnector.cfg` file
3. Add the webhook url after the equals sign for the setting:

    ![webhook url setting](../Images/howto-5.png)

4. Save the file