# Embed Configuration

## Allowed User Mentions

Type: `String`, default value: ``

 A semicolon-separated list of user IDs that are allowed to be mentioned in messages sent to Discord. These are just a number (no carets), e.g. `123;234` Note: This setting is overshadowed if 'Allow @user mentions` is enabled, and only when that is disabled will these users still be allowed to be mentioned.

## Show Embed Title

Type: `Boolean`, default value: `true`

 Enable this setting to show the title field in Discord embeds.

## Show Embed Description

Type: `Boolean`, default value: `true`

 Enable this setting to show the description field in Discord embeds.

## Show Embed Author

Type: `Boolean`, default value: `true`

 Enable this setting to show the author field in Discord embeds. This typically displays the server or player name.

## Show Embed Thumbnail

Type: `Boolean`, default value: `true`

 Enable this setting to show a thumbnail image in Discord embeds. This appears in the top-right of the embed.

## Show Embed Footer

Type: `Boolean`, default value: `true`

 Enable this setting to show the footer text in Discord embeds.

## Show Embed Timestamp

Type: `Boolean`, default value: `true`

 Enable this setting to show a timestamp in Discord embeds.

## Author Icon URL

Type: `String`, default value: `https://discord-connector.valheim.games.nwest.one/embed/author_icon.png`

 The URL for the small icon (32x32px) that appears next to the author name in Discord embeds.

## Thumbnail URL

Type: `String`, default value: `https://discord-connector.valheim.games.nwest.one/embed/thumbnail.png`

 The URL for the larger thumbnail image (ideally 256x256px) that appears in the top-right of Discord embeds.

## Footer Text

Type: `String`, default value: `Valheim Server | {worldName}`

 The text to display in the embed footer. You can use variables like {worldName}, {serverName}, and {timestamp}.

## Field Display Order

Type: `String`, default value: `position;event;player;details`

 The order in which to display embed fields. Format should be a semicolon-separated list of field identifiers.

## Embed URL Template

Type: `String`, default value: ``

 Optional URL template for the embed title. When set, the title becomes a clickable link. You can use variables like {worldName}, {serverName}, {playerName}.