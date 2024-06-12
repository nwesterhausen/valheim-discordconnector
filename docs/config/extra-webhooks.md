# Extra Webhooks

Filename `discordconnector.cfg`

This file provides configuration for 16 additional webhooks.

Each one supports the same settings as the primary and secondary that are in the [main config](./main).

## Webhook 1 - 16

What can be configured for each webhook

### Webhook URL

Type: `String`, default value: ``

Discord channel webhook URL. For instructions, reference the 'MAKING A WEBHOOK' section of [Discord's documentation](https://support.Discord.com/hc/en-us/articles/228383668-Intro-to-Webhook).

### Webhook Events

Type: `String`, default value: `ALL`

Specify a subset of possible events to send to this webhook. Format should be the keyword 'ALL' or a semi-colon separated list, e.g. `serverLifecycle;playerAll;playerFirstAll;leaderboardsAll;`

A full list of valid options for this are [here](https://discord-connector.valheim.games.nwest.one/config/webhook.events.html).

### Webhook Username Override

Type: `String`, default value: ``

Override the username of this webhook. If left blank, the webhook will first try to use the default username from the main configuration, then will rely on the name provided by Discord.

:::tip Default Username
A fallback default username to enforce can be set in the [main config](./main#default-webhook-username-override)

:::

### Webhook Avatar Override

Type: `String`, default value: ``

Override the avatar of this webhook. This should be a URL to an image. If left blank, the webhook will use the default avatar set in your Discord channel's "Integration" menu.
