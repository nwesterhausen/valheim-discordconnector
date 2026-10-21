using System.Reflection;
using DiscordConnector;
using DiscordConnector.Patches;
using HarmonyLib;

static object? Patch(Type target, string name, string stage, object instance, object? state = null)
{
    foreach (Type type in typeof(ZNetPatches).Assembly.GetTypes())
    {
        HarmonyPatch[] targets = type.GetCustomAttributes<HarmonyPatch>().ToArray();
        Assert(targets.Length <= 1, $"{type.Name} declares multiple Harmony targets; attributes merge instead of patching independent methods");
        if (!targets.Any(a => a.Type == target && a.Name == name)) continue;
        MethodInfo? method = type.GetMethod(stage, BindingFlags.Static | BindingFlags.NonPublic);
        if (method == null) continue;
        object?[] args = method.GetParameters().Select(p => p.Name == "__instance" ? instance : state).ToArray();
        method.Invoke(null, args);
        int stateIndex = Array.FindIndex(method.GetParameters(), p => p.Name == "__state");
        if (stateIndex >= 0) state = args[stateIndex];
    }
    return state;
}

static void Assert(bool condition, string message)
{
    if (!condition) throw new Exception(message);
}

static void Reset(bool crossplay = true, bool enabled = true)
{
    Patch(typeof(ZNet), nameof(ZNet.OnDestroy), "Prefix", ZNet.instance);
    ZNet.instance = new();
    ZNet.World = new();
    ZNet.m_onlineBackend = crossplay ? OnlineBackendType.PlayFab : OnlineBackendType.Steamworks;
    ZPlayFabMatchmaking.instance = new();
    ZPlayFabMatchmaking.JoinCode = "";
    UnityEngine.Time.realtimeSinceStartup = 0;
    DiscordConnectorPlugin.StaticConfig = new() { LoadedMessageEnabled = enabled };
    DiscordConnectorPlugin.StaticEventWatcher = new();
    DiscordConnectorPlugin.StaticLogger = new();
    DiscordApi.Sent.Clear();
}

static void Load() => Patch(typeof(ZNet), nameof(ZNet.LoadWorld), "Postfix", ZNet.instance);
static void Tick(float time = 1)
{
    UnityEngine.Time.realtimeSinceStartup = time;
    Patch(typeof(ZNet), nameof(ZNet.Update), "Postfix", ZNet.instance);
}
static void Register(ZPlayFabMatchmaking? matchmaking = null, string world = "world", bool deferred = false)
{
    matchmaking ??= ZPlayFabMatchmaking.instance;
    object? previous = Patch(typeof(ZPlayFabMatchmaking), nameof(ZPlayFabMatchmaking.RegisterServer), "Prefix", matchmaking);
    if (!deferred)
    {
        matchmaking.m_state = ZPlayFabMatchmaking.State.Uninitialized;
        matchmaking.m_serverData = new() { worldName = world, joinCode = "111111" };
        ZPlayFabMatchmaking.JoinCode = "111111";
    }
    Patch(typeof(ZPlayFabMatchmaking), nameof(ZPlayFabMatchmaking.RegisterServer), "Postfix", matchmaking, previous);
}
static void State(ZPlayFabMatchmaking.State state, string code, ZPlayFabMatchmaking? matchmaking = null)
{
    matchmaking ??= ZPlayFabMatchmaking.instance;
    matchmaking.m_state = state;
    matchmaking.m_serverData!.joinCode = code;
    matchmaking.m_serverData.lobbyId = "current-lobby";
    ZPlayFabMatchmaking.JoinCode = code;
    Patch(typeof(ZPlayFabMatchmaking), nameof(ZPlayFabMatchmaking.OnSessionUpdated), "Postfix", matchmaking);
}
static void SentFinal()
{
    Assert(DiscordApi.Sent.Count == 1, $"Expected one ServerStart, got {DiscordApi.Sent.Count}");
    Assert(DiscordApi.Sent[0] == (Webhook.Event.ServerStart, "Ready 222222"), "Wrong event/final formatted message");
}

var tests = new (string Name, Action Run)[]
{
    ("crossplay LoadWorld does not send an empty join code", () =>
    {
        Reset(); Load();
        Assert(DiscordApi.Sent.Count == 0, $"crossplay LoadWorld sent before active registration (count={DiscordApi.Sent.Count}, message='{DiscordApi.Sent.FirstOrDefault().Message}')");
        Assert(DiscordConnectorPlugin.StaticEventWatcher.Activations == 1, "EventWatcher activation was delayed");
    }),
    ("noncrossplay remains immediate; repeated load is deduplicated", () =>
    {
        Reset(crossplay: false); Load(); Load(); Tick(200);
        Assert(DiscordApi.Sent.Count == 1 && DiscordApi.Sent[0].Message == "Ready ", "Noncrossplay behavior changed");
        Assert(DiscordConnectorPlugin.StaticLogger.Warnings.Count == 0, "Noncrossplay timed out");
    }),
    ("disabled startup never sends or warns", () =>
    {
        Reset(enabled: false); Load(); Register(); State(ZPlayFabMatchmaking.State.Active, "222222"); Tick(200);
        Assert(DiscordApi.Sent.Count == 0 && DiscordConnectorPlugin.StaticLogger.Warnings.Count == 0, "Disabled startup sent/warned");
        Assert(DiscordConnectorPlugin.StaticEventWatcher.Activations == 1, "Disabled message stopped EventWatcher");
    }),
    ("intermediate/regenerated code waits for confirmed Active; duplicate events send once", () =>
    {
        Reset(); Load(); Register(); State(ZPlayFabMatchmaking.State.Creating, "111111"); Tick();
        State(ZPlayFabMatchmaking.State.RegenerateJoinCode, "222222"); Tick(); Load();
        Assert(DiscordApi.Sent.Count == 0, "An intermediate code was sent");
        State(ZPlayFabMatchmaking.State.Active, "222222");
        State(ZPlayFabMatchmaking.State.Active, "222222"); Load(); Tick();
        SentFinal();
    }),
    ("registration and activation before load are retained", () =>
    {
        Reset(); Register(); State(ZPlayFabMatchmaking.State.Active, "222222");
        Assert(DiscordApi.Sent.Count == 0, "Sent before world load");
        Load(); SentFinal();
    }),
    ("readiness is not gated on a literal join-code token", () =>
    {
        Reset(); DiscordConnectorPlugin.StaticConfig.LoadedMessage = "%CUSTOM_VARIABLE%";
        Load(); Register(); State(ZPlayFabMatchmaking.State.Creating, "111111");
        Assert(DiscordApi.Sent.Count == 0, "Raw token inspection bypassed readiness");
        State(ZPlayFabMatchmaking.State.Active, "222222");
        Assert(DiscordApi.Sent.Count == 1 && DiscordApi.Sent[0].Message == "%CUSTOM_VARIABLE%", "Configured message changed");
    }),
    ("foreign matchmaking and wrong-world registration cannot release startup", () =>
    {
        Reset(); Load();
        var foreign = new ZPlayFabMatchmaking();
        Register(foreign); State(ZPlayFabMatchmaking.State.Active, "222222", foreign); Tick();
        Register(world: "other-world"); State(ZPlayFabMatchmaking.State.Active, "222222"); Tick();
        Assert(DiscordApi.Sent.Count == 0, "Foreign session released startup");
        Register(); State(ZPlayFabMatchmaking.State.Active, "222222"); SentFinal();
    }),
    ("previous-world active session/static code and deferred registration are rejected", () =>
    {
        Reset(); Register(); State(ZPlayFabMatchmaking.State.Active, "222222");
        ZNet.instance = new(); // Same name/singleton, different world lifetime, no teardown callback yet.
        Load(); Register(deferred: true); Tick();
        Assert(DiscordApi.Sent.Count == 0, "Stale session/code released a new world");
        Register(); State(ZPlayFabMatchmaking.State.Active, "222222"); SentFinal();
    }),
    ("replaced session data must be registered for this world", () =>
    {
        Reset(); Load(); Register();
        ZPlayFabMatchmaking.instance.m_serverData = new();
        State(ZPlayFabMatchmaking.State.Active, "222222"); Tick();
        Assert(DiscordApi.Sent.Count == 0, "Uncorrelated data was accepted");
    }),
    ("Active still requires lobby and matching nonempty current code", () =>
    {
        Reset(); Load(); Register(); State(ZPlayFabMatchmaking.State.Active, ""); Tick();
        Assert(DiscordApi.Sent.Count == 0, "Active empty code sent");
        State(ZPlayFabMatchmaking.State.Active, "222222"); SentFinal();
        Reset(); Load(); Register(); State(ZPlayFabMatchmaking.State.Creating, "222222");
        ZPlayFabMatchmaking.instance.m_state = ZPlayFabMatchmaking.State.Active;
        ZPlayFabMatchmaking.JoinCode = "stale"; Tick();
        Assert(DiscordApi.Sent.Count == 0, "Mismatching static code sent");
        ZPlayFabMatchmaking.JoinCode = "222222";
        ZPlayFabMatchmaking.instance.m_serverData!.lobbyId = ""; Tick();
        Assert(DiscordApi.Sent.Count == 0, "Missing lobby sent");
        ZPlayFabMatchmaking.instance.m_serverData.lobbyId = "current-lobby"; Tick(); SentFinal();
    }),
    ("timeout warns once, permanently suppresses late activation and repeated load", () =>
    {
        Reset(); Load(); Register(); Tick(179); Load(); Tick(180); Tick(181);
        Assert(DiscordConnectorPlugin.StaticLogger.Warnings.Count == 1, "Timeout warning missing/duplicated");
        Assert(!DiscordConnectorPlugin.StaticLogger.Warnings[0].Contains("111111"), "Join code leaked in warning");
        State(ZPlayFabMatchmaking.State.Active, "222222"); Load(); Tick(200);
        Assert(DiscordApi.Sent.Count == 0, "Late activation escaped timeout suppression");
    }),
    ("active callback checks deadline even before the next Update", () =>
    {
        Reset(); Load(); Register(); UnityEngine.Time.realtimeSinceStartup = 180;
        State(ZPlayFabMatchmaking.State.Active, "222222");
        Assert(DiscordApi.Sent.Count == 0 && DiscordConnectorPlugin.StaticLogger.Warnings.Count == 1, "Late callback sent");
    }),
    ("StopAll and OnDestroy cancel pending work; new world can send", () =>
    {
        foreach (string method in new[] { nameof(ZNet.StopAll), nameof(ZNet.OnDestroy) })
        {
            Reset(); Load(); Register();
            Patch(typeof(ZNet), method, "Prefix", ZNet.instance);
            State(ZPlayFabMatchmaking.State.Active, "222222"); Tick(200);
            Assert(DiscordApi.Sent.Count == 0 && DiscordConnectorPlugin.StaticLogger.Warnings.Count == 0, $"{method} did not cancel");
            ZNet.instance = new(); Load(); Tick();
            Assert(DiscordApi.Sent.Count == 0, "New world inherited old session");
            Register(); State(ZPlayFabMatchmaking.State.Active, "222222"); SentFinal();
        }
    }),
    ("disabling while pending suppresses startup", () =>
    {
        Reset(); Load(); Register(); DiscordConnectorPlugin.StaticConfig.LoadedMessageEnabled = false;
        State(ZPlayFabMatchmaking.State.Active, "222222"); Tick(200);
        Assert(DiscordApi.Sent.Count == 0 && DiscordConnectorPlugin.StaticLogger.Warnings.Count == 0, "Disabled pending message escaped");
    })
};

int failed = 0;
foreach (var test in tests)
{
    try { test.Run(); Console.WriteLine($"PASS: {test.Name}"); }
    catch (Exception error) { failed++; Console.Error.WriteLine($"FAIL: {test.Name}: {error.Message}"); }
}
Console.WriteLine($"{tests.Length - failed}/{tests.Length} passed");
return failed == 0 ? 0 : 1;
