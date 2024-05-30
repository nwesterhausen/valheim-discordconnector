# Messages.LeaderBoards

::: tip Available Dynamic Variables

| Variable            | Replaced with..                                                                           | Can be used in..    |
| ------------------- | ----------------------------------------------------------------------------------------- | ------------------- |
| `%VAR1%` - `VAR10%` | Custom variable value (defined in [Custom Variables](/config/variables.html) config file) | Any messages        |
| `%PUBLICIP%`        | Server's public IP (according to the server)                                              | Any server messages |
| `%DAY_NUMBER%`      | Current day number on server                                                              | Any messages        |
:::

## Leader Board Heading for Top N Players

Type: `String`, default value: `Top %N% Player Leader Boards:`

Set the message that is included as a heading when this leader board is sent. Include %N% to include the number of rankings returned (the configured number)

## Leader Board Heading for Bottom N Players

Type: `String`, default value: `Bottom %N% Player Leader Boards:`

Set the message that is included as a heading when this leader board is sent. Include %N% to include the number of rankings returned (the configured number)

## Leader Board Heading for Highest Player

Type: `String`, default value: `Top Performer`

Set the message that is included as a heading when this leader board is sent. Include %N% to include the number of rankings returned (the configured number)

## Leader Board Heading for Lowest Player

Type: `String`, default value: `Bottom Performer`

Set the message that is included as a heading when this leader board is sent. Include %N% to include the number of rankings returned (the configured number)
