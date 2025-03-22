using System;
using System.Collections;

using HarmonyLib;

namespace DiscordConnector.RPC;

public static class Client
{
    internal static IEnumerator RPC_OnNewChatMessage(long sender, ZPackage pkg)
    {
        DiscordConnectorPlugin.StaticLogger.LogDebug($"Client  Received {Common.RPC_OnNewChatMessage} from {sender}");
        
        // In the client, we don't need to do anything with the chat message.
        yield break;
    }

}
