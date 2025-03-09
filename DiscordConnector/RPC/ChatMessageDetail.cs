using System;

using UnityEngine;

namespace DiscordConnector.RPC;

public class ChatMessageDetail(Vector3 pos, Talker.Type type, string userName, string platformId, string text)
{
    public Vector3 Pos { get; private set; } = pos;
    public Talker.Type TalkerType { get; private set; } = type;
    public string UserName { get; private set; } = userName;
    public string PlatformId { get; private set; } = platformId;
    public string Text { get; private set; } = text;

    public ZPackage ToZPackage()
    {
        ZPackage pkg = new ZPackage();
        pkg.Write(Pos);
        pkg.Write((int)TalkerType);
        pkg.Write(UserName);
        pkg.Write(PlatformId);
        pkg.Write(Text);
        return pkg;
    }

    public static ChatMessageDetail FromZPackage(ZPackage pkg)
    {
        DiscordConnectorPlugin.StaticLogger.LogDebug($"Reading ZPackage with {pkg.ReadNumItems()} items");
        DiscordConnectorPlugin.StaticLogger.LogDebug(pkg.ToString());
        try
        {
            var pos = pkg.ReadVector3();
            var talkerType = (Talker.Type)pkg.ReadInt();
            var userName = pkg.ReadString();
            var platformId = pkg.ReadString();
            var text = pkg.ReadString();
            
            return new ChatMessageDetail(pos, talkerType, userName, platformId, text);
        }
        catch (Exception e)
        {
            DiscordConnectorPlugin.StaticLogger.LogError(e.Message);
        }
        
        // Return a default ChatMessageDetail if we failed to read the package
        return new ChatMessageDetail(Vector3.zero, Talker.Type.Normal, "unknown/error", "unknown/error", "##N/A##");
    }

    public override string ToString()
    {
        return
            $"ChatMessageDetail: Pos={Pos}, Type={TalkerType}, UserName={UserName}, PlatformId={PlatformId}, Text={Text}";
    }
}
