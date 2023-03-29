# Variable Definitions

Filename `discordconnector-variables.cfg`

You may assign strings to these variables to reference them in any messages.

## Variable Definition

### Defined Variable 1

Type: `String`, default value: ``

 This variable can be reference in any of the message content settings with `%VAR1%`

### Defined Variable 2

Type: `String`, default value: ``

 This variable can be reference in any of the message content settings with `%VAR2%`

### Defined Variable 3

Type: `String`, default value: ``

 This variable can be reference in any of the message content settings with `%VAR3%`

### Defined Variable 4

Type: `String`, default value: ``

 This variable can be reference in any of the message content settings with `%VAR4%`

### Defined Variable 5

Type: `String`, default value: ``

 This variable can be reference in any of the message content settings with `%VAR5%`

### Defined Variable 6

Type: `String`, default value: ``

 This variable can be reference in any of the message content settings with `%VAR6%`

### Defined Variable 7

Type: `String`, default value: ``

 This variable can be reference in any of the message content settings with `%VAR7%`

### Defined Variable 8

Type: `String`, default value: ``

 This variable can be reference in any of the message content settings with `%VAR8%`

### Defined Variable 9

Type: `String`, default value: ``

 This variable can be reference in any of the message content settings with `%VAR9%`

### Defined Variable 10

Type: `String`, default value: ``

 This variable can be reference in any of the message content settings with `%VAR10%`

## Variables.DynamicConfig

Some variables can be used to define the text formatting used. Specifically the positional information.

### POS Variable Formatting

Type: `String`, default value: `%X%, %Y%, %Z%`

 Modify this to change how the %POS% variable gets displayed. You can use %X%, %Y%, and %Z% in this value to customize how the %POS% gets sent.

### Auto-Appended POS Format

Type: `String`, default value: `Coords: (%POS%)`

 This defines how the automatic inclusion of the position data is included. This gets appended to the messages sent. If you prefer to embed the POS inside the message instead of embedding it, you can modify the messages in the message config  to include the %POS% variable. This POS message only gets appended on the message if no %POS% is in the message getting sent  but you have sent position data enabled for that message.

::: info
POS data gets auto-appended if you enable the `Send Position with ...` toggles.

If you manually include `%POS%` in your messages then those will not be affected by the "auto-append" format setting.
:::
