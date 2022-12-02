# Variable Definitions

Filename `discordconnector-variables.cfg`

You may assign strings to these variables to reference them in any messages.

| Option              | Default | Description                                                                        |
| ------------------- | ------- | ---------------------------------------------------------------------------------- |
| Defined Variable 1  | (none)  | This variable can be reference in any of the message content settings with %VAR1%  |
| Defined Variable 2  | (none)  | This variable can be reference in any of the message content settings with %VAR2%  |
| Defined Variable 3  | (none)  | This variable can be reference in any of the message content settings with %VAR3%  |
| Defined Variable 4  | (none)  | This variable can be reference in any of the message content settings with %VAR4%  |
| Defined Variable 5  | (none)  | This variable can be reference in any of the message content settings with %VAR5%  |
| Defined Variable 6  | (none)  | This variable can be reference in any of the message content settings with %VAR6%  |
| Defined Variable 7  | (none)  | This variable can be reference in any of the message content settings with %VAR7%  |
| Defined Variable 8  | (none)  | This variable can be reference in any of the message content settings with %VAR8%  |
| Defined Variable 9  | (none)  | This variable can be reference in any of the message content settings with %VAR9%  |
| Defined Variable 10 | (none)  | This variable can be reference in any of the message content settings with %VAR10% |

## Variable Configurations

Some variables can be configured. Mainly the positional information.

| Option                   | Default           | Description                                                                     |
| ------------------------ | ----------------- | ------------------------------------------------------------------------------- |
| POS Variable Formatting  | `%X%, %Y%, %Z%`   | Change how the %POS% variable is formatted.                                     |
| Auto-Appended POS Format | `Coords: (%POS%)` | Change this to modify how Discord Connector automatically appends the POS data. |

::: info
POS data gets auto-appended if you enable the `Send Position with ...` toggles.

If you manually include `%POS%` in your messages then those will not be affected by the "auto-append" format setting.
:::
