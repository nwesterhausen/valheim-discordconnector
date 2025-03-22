using HarmonyLib;

namespace DiscordConnector.Patches;

internal class EnvManPatches
{
    [HarmonyPatch(typeof(EnvMan), nameof(EnvMan.UpdateTriggers))]
    internal class OnNewDay
    {
        private static void Postfix(float oldDayFraction, float newDayFraction)
        {
            if (EnvMan.instance == null)
            {
                return;
            }

            if (!DiscordConnectorPlugin.StaticConfig.NewDayNumberEnabled)
            {
                return;
            }

            if (oldDayFraction > 0.2f && oldDayFraction < 0.25f && newDayFraction > 0.25f && newDayFraction < 0.3f)
            {
                // Use the embed system for day number events
                if (DiscordConnectorPlugin.StaticConfig.DiscordEmbedsEnabled)
                {
                    string worldName = ZNet.instance != null ? ZNet.instance.GetWorldName() : "Unknown World";
                    string dayNumber = EnvMan.instance.GetCurrentDay().ToString();
                    var embed = EmbedTemplates.WorldEvent(Webhook.Event.NewDayNumber, 
                        MessageTransformer.FormatServerMessage(DiscordConnectorPlugin.StaticConfig.NewDayMessage),
                        $"Day Number {dayNumber}", worldName);
                    DiscordApi.SendEmbed(Webhook.Event.NewDayNumber, embed);
                }
                else
                {
                    DiscordApi.SendMessage(
                        Webhook.Event.NewDayNumber,
                        MessageTransformer.FormatServerMessage(DiscordConnectorPlugin.StaticConfig.NewDayMessage)
                    );
                }
            }
        }
    }
}
