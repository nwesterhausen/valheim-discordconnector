# Variables.DynamicConfig

Some variables can be used to define the text formatting used. Specifically the positional information.

## POS Variable Formatting

Type: `String`, default value: `%X%, %Y%, %Z%`

Modify this to change how the `%POS%` variable gets displayed. You can use `%X%`, `%Y%`, and `%Z%` in this value to customize how the `%POS%` gets sent.

## Auto-Appended POS Format

Type: `String`, default value: `Coords: (%POS%)`

This defines how the automatic inclusion of the position data is included. This gets appended to the messages sent.

If you prefer to embed the POS inside the message instead of embedding it, you can modify the messages in the message config  to include the `%POS%` variable.

This POS message only gets appended on the message if no `%POS%` is in the message getting sent but you have sent position data enabled for that message.

::: info When does it get appended?
POS data gets auto-appended if you enable the `Send Position with ...` [toggles](/config/toggles.position.html).

If you manually include `%POS%` in your messages then those will not be affected by the "auto-append" format setting.
:::
