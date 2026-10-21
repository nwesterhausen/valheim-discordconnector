// Only the game/plugin surface used by the linked production patches; no network calls.
namespace HarmonyLib
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class HarmonyPatch(Type type, string name) : Attribute
    {
        public Type Type = type;
        public string Name = name;
    }
}

namespace UnityEngine
{
    public static class Time { public static float realtimeSinceStartup; }
}

public enum OnlineBackendType { Steamworks, PlayFab }
public class World { public string m_seedName = "world"; }
public class PlayFabMatchmakingServerData
{
    public string worldName = "world", joinCode = "", lobbyId = "";
}
public class ZPlayFabMatchmaking
{
    public enum State { Uninitialized, Creating, RegenerateJoinCode, Active }
    public static ZPlayFabMatchmaking instance = new();
    public static string JoinCode = "";
    public State m_state;
    public PlayFabMatchmakingServerData? m_serverData;
    public void RegisterServer() { }
    public void OnSessionUpdated() { }
}
public class ZNet
{
    public static ZNet instance = new();
    public static World World = new();
    public static OnlineBackendType m_onlineBackend;
    public bool m_haveStoped;
    public bool HaveStopped => m_haveStoped;
    public bool IsServer() => true;
    public void LoadWorld() { }
    public void SaveWorld() { }
    public void Update() { }
    public void StopAll() { }
    public void OnDestroy() { }
    public void RPC_CharacterID() { }
    public void RPC_Disconnect() { }
    public ZNetPeer GetPeer(ZRpc rpc) => null!;
}
public class ZRpc { }
public struct ZDOID { }
public class ZNetPeer { public long m_uid; }

namespace DiscordConnector
{
    public static class DiscordConnectorPlugin
    {
        public static Config StaticConfig = new();
        public static Watcher StaticEventWatcher = new();
        public static Logger StaticLogger = new();
        public static bool IsHeadless() => true;
    }
    public class Config
    {
        public bool LoadedMessageEnabled = true, WorldSaveMessageEnabled;
        public string LoadedMessage = "Ready {{joincode}}", SaveMessage = "Saved";
    }
    public class Watcher { public int Activations; public void Activate() => Activations++; }
    public class Logger
    {
        public List<string> Warnings = new();
        public void LogWarning(string message) => Warnings.Add(message);
    }
    public static class Webhook { public enum Event { ServerStart, ServerSave } }
    public static class DiscordApi
    {
        public static List<(Webhook.Event Event, string Message)> Sent = new();
        public static void SendMessage(Webhook.Event ev, string message) => Sent.Add((ev, message));
    }
    public static class MessageTransformer
    {
        public static string FormatServerMessage(string raw) => raw.Replace("{{joincode}}", ZPlayFabMatchmaking.JoinCode);
    }
    public static class Handlers
    {
        public static void Join(ZNetPeer peer) { }
        public static void Leave(ZNetPeer peer) { }
    }
}
