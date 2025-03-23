using System;
using System.Collections;

using HarmonyLib;

namespace DiscordConnector.RPC;

public static class Client
{
    public static IEnumerator RPC_OnNewChatMessage(long sender, ZPackage pkg)
    {
        // In the client, we don't need to do anything with the chat message.
        yield break;
    }

}
