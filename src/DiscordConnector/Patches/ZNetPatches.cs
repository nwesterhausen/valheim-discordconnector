using HarmonyLib;
using JetBrains.Annotations;

namespace DiscordConnectorLite.Patches
{
    internal class ZNetPatches
    {

        [HarmonyPatch(typeof(ZNet), nameof(ZNet.LoadWorld))]
        internal class LoadWorld
        {
            [UsedImplicitly]
            private static void Postfix()
            {
                if (Plugin.StaticConfig.LoadedMessageEnabled)
                {
                    DiscordApi.SendMessage(
                        MessageTransformer.FormatServerMessage(Plugin.StaticConfig.LoadedMessage)
                    );
                }

                Plugin.ServerState = "running";
                Plugin.ServerWorld = ZNet.instance.GetWorldName();
            }
        }

        [HarmonyPatch(typeof(ZNet), nameof(ZNet.SaveWorld))]
        internal class SaveWorld
        {
            [UsedImplicitly]
            private static void Postfix(ref bool sync)
            {
                if (Plugin.StaticConfig.WorldSaveMessageEnabled)
                {
                    DiscordApi.SendMessage(
                        MessageTransformer.FormatServerMessage(Plugin.StaticConfig.SaveMessage)
                    );
                }
            }
        }
    }
}
