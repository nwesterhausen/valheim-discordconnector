# Debug Messages

:::danger Verbosity
Many of these settings enable many more messages to be added to the log. Make sure to only enable them with good reason!
:::

## Debug Message for Every Event Check

Type: `Boolean`, default value: `false`

If enabled, this will write a log message at the `DEBUG` level every time it checks for an event (every 1s).

## Debug Message for Every Event Player Location Check

Type: `Boolean`, default value: `false`

If enabled, this will write a log message at the `DEBUG` level every time the `EventWatcher` checks players' locations.

## Debug Message for Every Event Change

Type: `Boolean`, default value: `false`

If enabled, this will write a log message at the `DEBUG` level when a change in event status is detected.

## Debug Message for HTTP Request Responses

Type: `Boolean`, default value: `false`

If enabled, this will write a log message at the `DEBUG` level with the content of HTTP request responses.

All of these requests are when data is sent to the Discord Webhook.
