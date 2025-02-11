using HarmonyLib;

namespace DiscordConnector.Patches;
internal class EnvManPatches
{
    [HarmonyPatch(typeof(EnvMan), nameof(EnvMan.UpdateTriggers))]
    internal class OnNewDay
    {
        private static void Postfix(float oldDayFraction, float newDayFraction)
        {
            if (EnvMan.instance == null) return;

            if (!DiscordConnectorPlugin.StaticConfig.NewDayNumberEnabled) return;

            if (oldDayFraction > 0.2f && oldDayFraction < 0.25f && newDayFraction > 0.25f && newDayFraction < 0.3f)
            {
                DiscordApi.SendMessage(
                    Webhook.Event.NewDayNumber,
                    MessageTransformer.FormatServerMessage(DiscordConnectorPlugin.StaticConfig.NewDayMessage)
                );
            }
        }
    }
}
