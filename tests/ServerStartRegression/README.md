# ServerStart regression harness

From the repository root, with .NET SDK 8 or newer and the .NET 8 runtime installed:

```sh
dotnet run --project tests/ServerStartRegression/ServerStartRegression.csproj
```

Links the actual production `ZNetPatches.cs`; no test packages or network calls.
Game, Unity clock, Harmony attributes, formatter and Discord sender are stubbed.
The harness invokes the attributed prefixes/postfixes in lifecycle order; it does
not validate runtime Harmony weaving, Unity object semantics or game callbacks.
It rejects multiple target attributes on one class rather than incorrectly
dispatching them as independent patches (Harmony merges their metadata).
Message/embed/config implementations are unchanged and outside this harness.

Before the production fix, the first assertion failed with:
`crossplay LoadWorld sent before active registration (count=1, message='Ready ')`.
