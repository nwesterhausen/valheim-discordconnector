# Main Config

Filename `discordconnector.cfg`

## Default Webhook Username Override

Type: `String`, default value: ``

Override the username of all webhooks for this instance of Discord Connector. If left blank, the webhook will use the default name (assigned by Discord in the Integration menu).

This setting will be used for all webhooks unless overridden by a specific webhook username override setting.

## Webhook URL

Type: `String`, default value: ``

Discord channel webhook URL. For instructions, reference the 'MAKING A WEBHOOK' section of [Discord's documentation](https://support.Discord.com/hc/en-us/articles/228383668-Intro-to-Webhook).

## Webhook Events

Type: `String`, default value: `ALL`

Specify a subset of possible events to send to the primary webhook. Format should be the keyword 'ALL' or a semi-colon separated list, e.g. `serverLifecycle;playerAll;playerFirstAll;leaderboardsAll;`

A full list of valid options for this are [here](https://discord-connector.valheim.games.nwest.one/config/webhook.events.html).

## Webhook Username Override

Type: `String`, default value: ``

Override the username of this webhook. If left blank, the webhook will use the default username set in your Discord channel's "Integration" menu.

## Webhook Avatar Override

Type: `String`, default value: ``

Override the avatar of this webhook. This should be a URL to an image. If left blank, the webhook will use the default avatar set in your Discord channel's "Integration" menu.

## Secondary Webhook URL

Type: `String`, default value: ``

Discord channel webhook URL. For instructions, reference the 'MAKING A WEBHOOK' section of [Discord's documentation](https://support.Discord.com/hc/en-us/articles/228383668-Intro-to-Webhook).

## Secondary Webhook Events

Type: `String`, default value: `ALL`

Specify a subset of possible events to send to the secondary webhook. Format should be the keyword 'ALL' or a semi-colon separated list, e.g. `serverLifecycle;playerAll;playerFirstAll;leaderboardsAll;`

A full list of valid options for this are [here](https://discord-connector.valheim.games.nwest.one/config/webhook.events.html).

## Secondary Webhook Username Override

Type: `String`, default value: ``

Override the username of this webhook. If left blank, the webhook will use the default username set in your Discord channel's "Integration" menu.

## Secondary Webhook Avatar Override

Type: `String`, default value: ``

Override the avatar of this webhook. This should be a URL to an image. If left blank, the webhook will use the default avatar set in your Discord channel's "Integration" menu.

## Log Debug Messages

Type: `Boolean`, default value: `false`

Enable this setting to listen to debug messages from the mod. This will help with troubleshooting issues.

## Use fancier discord messages

Type: `Boolean`, default value: `false`

Enable this setting to use embeds in the messages sent to Discord. Currently this will affect the position details for the messages.

## Ignored Players

Type: `String`, default value: ``

It may be that you have some players that you never want to send Discord messages for. Adding a player name to this list will ignore them.

Format should be a semicolon-separated list: `Stuart;John McJohnny;Weird-name1`

## Ignored Players (Regex)

Type: `String`, default value: ``

It may be that you have some players that you never want to send Discord messages for. This option lets you provide a regular expression to filter out players if their name matches.

Format should be a valid string for a .NET Regex (reference: [docs.microsoft.com](https://docs.microsoft.com/en-us/dotnet/standard/base-types/regular-expression-language-quick-reference))

## Send Positions with Messages

Type: `Boolean`, default value: `true`

Disable this setting to disable any positions/coordinates being sent with messages (e.g. players deaths or players joining/leaving). (Overwrites any individual setting.)

## Collect Player Stats

Type: `Boolean`, default value: `true`

Disable this setting to disable all stat collection. (Overwrites any individual setting.)

::: details Stat Collection Details
Stat collection will create a file in the `discordconnector` config directory `records.db`, where it will record the number of times each player joins, leaves, dies, shouts or pings.

If this is set to false, DiscordConnector will not keep a record of number of times each player does something it alerts to.

If this is false, it takes precedent over the "Send leader board updates" setting and no leader boards will get sent.

The stat collection database uses the [LiteDB](https://www.litedb.org/) library and if you are so inclined they offer a database gui which you can use to view/modify this database. (Find the LiteDB Editor on their site.)
:::

## Announce Player Firsts

Type: `Boolean`, default value: `true`

Disable this setting to disable all extra announcements the first time each player does something. (Overwrites any individual setting.)

## How to discern players in Record Retrieval

Type: `RetrievalDiscernmentMethods`, default value: `PlayerId`

Acceptable values: `PlayerId`, `Name`, `NameAndPlayerId`

Choose a method for how players will be separated from the results of a record query (used for statistic leader boards).

:::info Discernment Methods
| Options         | Description                                                    |
| --------------- | -------------------------------------------------------------- |
| Name            | Treat each CharacterName as a separate player                  |
| PlayerId        | Treat each PlayerId as a separate player                       |
| NameAndPlayerId | Treat each [PlayerId:CharacterName] combo as a separate player |

:::

:::warning What is the PlayerID?
The player ID is the player's hostname, i.e. for a player connecting using the Steam version of the game, their PlayerId would be something like `STEAM_120390101034` (`STEAM_<steam id>`). Players connecting from other platforms will have different formatting for this.

Examples of other platforms are the game pass and xbox version of Valheim.
:::

## Send Non-Player Shouts to Discord

Type: `Boolean`, default value: `false`

Enable this setting to have shouts which are performed by other mods/the server/non-players to be sent to Discord as well.

:::warning Muted Players
These are still subject to censure by the muted player regex and list.
:::

## Allow Mentions for @here and @everyone

Type: `Boolean`, default value: `false`

Enable this setting to allow the use of @here and @everyone in messages sent to Discord.

:::warning
This setting is disabled by default to prevent accidental mentions of everyone in the Discord channel. There is no filtering to prevent players from using these mentions in their shouts or names.
:::

## Allow Role Mentions

Type: `Boolean`, default value: `true`

Enable this setting to allow the use of role mentions in messages sent to Discord. Role mentions are in the format `<@&role_id>`, where `role_id` is the ID of the role you want to mention.

:::warning
There is no filtering to prevent players from using these mentions in their shouts or names.
:::

## Allow User Mentions

Type: `Boolean`, default value: `true`

Enable this setting to allow the use of user mentions in messages sent to Discord. User mentions are in the format `<@user_id>`, where `user_id` is the ID of the user you want to mention.

:::warning
There is no filtering to prevent players from using these mentions in their shouts or names.
:::

## Specifically Allowed Role Mentions

Type: `String`, default value: ``

Example value: `123452834;123452835`

If you want to allow only certain roles to be mentioned, you can specify them here. This is a semicolon-separated list of role IDs where `role_id` is the ID of the role you want to allow mentions for.

This setting will take precedence over the `Allow Role Mentions` setting (of course if all roles are allowed, this setting is redundant).

:::tip How to get a Role ID
Send a message in Discord with a backslash before the role mention, e.g. `\@role_name`. This will display the role mention in the format `<@&role_id>`, where `role_id` is the ID of the role you want to mention.

:::

## Specifically Allowed User Mentions

Type: `String`, default value: ``

Example value: `123452834;123452835`

If you want to allow only certain users to be mentioned, you can specify them here. This is a semicolon-separated list of user IDs where `user_id` is the ID of the user you want to allow mentions for.

This setting will take precedence over the `Allow User Mentions` setting (of course if all users are allowed, this setting is redundant).

:::tip How to get a User ID
Send a message in Discord with a backslash before the user mention, e.g. `\@user_name`. This will display the user mention in the format `<@user_id>`, where `user_id` is the ID of the user you want to mention.

:::
