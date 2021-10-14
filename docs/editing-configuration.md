## Editing the config using R2Modman

With R2Modman open to the profile you are using for the server, 

1. Go to Config Editor
2. Click on the DiscordConnector config
3. Paste the webhook URL into the Webhook URL field

    ![config editor section](/img/howto-4.png)

4. Click Save

## Editing the config manually

1. Navigate to `$(GameDirectory)/BepInEx/config`
2. Edit the `games.nwest.valheim.discordconnector.cfg` file
3. Add the webhook url after the equals sign for the setting:

    ![webhook url setting](/img/howto-5.png)

4. Save the file