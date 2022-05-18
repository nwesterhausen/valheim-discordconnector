using BepInEx.Configuration;

namespace DiscordConnector.Config
{
    internal class VariableConfig
    {
        private static ConfigFile config;

        public static string ConfigExtension = "variables";

        // config header strings
        private const string VARIABLE_SETTINGS = "Variable Definition";

        // Variable Definition
        private ConfigEntry<string> userVar;
        private ConfigEntry<string> userVar1;
        private ConfigEntry<string> userVar2;
        private ConfigEntry<string> userVar3;
        private ConfigEntry<string> userVar4;
        private ConfigEntry<string> userVar5;
        private ConfigEntry<string> userVar6;
        private ConfigEntry<string> userVar7;
        private ConfigEntry<string> userVar8;
        private ConfigEntry<string> userVar9;

        public VariableConfig(ConfigFile configFile)
        {
            config = configFile;
            LoadConfig();
        }

        public void ReloadConfig()
        {
            config.Reload();
            config.Save();
        }
        private void LoadConfig()
        {

            // User Variable Settings
            userVar = config.Bind<string>(VARIABLE_SETTINGS,
                "Defined Variable 1",
                "",
                "This variable can be reference in any of the message content settings with %VAR1%");
            userVar1 = config.Bind<string>(VARIABLE_SETTINGS,
                "Defined Variable 2",
                "",
                "This variable can be reference in any of the message content settings with %VAR2%");
            userVar2 = config.Bind<string>(VARIABLE_SETTINGS,
                "Defined Variable 3",
                "",
                "This variable can be reference in any of the message content settings with %VAR3%");
            userVar3 = config.Bind<string>(VARIABLE_SETTINGS,
                "Defined Variable 4",
                "",
                "This variable can be reference in any of the message content settings with %VAR4%");
            userVar4 = config.Bind<string>(VARIABLE_SETTINGS,
                "Defined Variable 5",
                "",
                "This variable can be reference in any of the message content settings with %VAR5%");
            userVar5 = config.Bind<string>(VARIABLE_SETTINGS,
                "Defined Variable 6",
                "",
                "This variable can be reference in any of the message content settings with %VAR6%");
            userVar6 = config.Bind<string>(VARIABLE_SETTINGS,
                "Defined Variable 7",
                "",
                "This variable can be reference in any of the message content settings with %VAR7%");
            userVar7 = config.Bind<string>(VARIABLE_SETTINGS,
                "Defined Variable 8",
                "",
                "This variable can be reference in any of the message content settings with %VAR8%");
            userVar8 = config.Bind<string>(VARIABLE_SETTINGS,
                "Defined Variable 9",
                "",
                "This variable can be reference in any of the message content settings with %VAR9%");
            userVar9 = config.Bind<string>(VARIABLE_SETTINGS,
                "Defined Variable 10",
                "",
                "This variable can be reference in any of the message content settings with %VAR10%");

            config.Save();
        }

        public string ConfigAsJson()
        {
            string jsonString = "{";
            jsonString += $"\"userVar\":\"{UserVariable}\",";
            jsonString += $"\"userVar1\":\"{UserVariable1}\",";
            jsonString += $"\"userVar2\":\"{UserVariable2}\",";
            jsonString += $"\"userVar3\":\"{UserVariable3}\",";
            jsonString += $"\"userVar4\":\"{UserVariable4}\",";
            jsonString += $"\"userVar5\":\"{UserVariable5}\",";
            jsonString += $"\"userVar6\":\"{UserVariable6}\",";
            jsonString += $"\"userVar7\":\"{UserVariable7}\",";
            jsonString += $"\"userVar8\":\"{UserVariable8}\",";
            jsonString += $"\"userVar9\":\"{UserVariable9}\"";
            jsonString += "}";
            return jsonString;
        }
        // Variables
        public string UserVariable => userVar.Value;
        public string UserVariable1 => userVar1.Value;
        public string UserVariable2 => userVar2.Value;
        public string UserVariable3 => userVar3.Value;
        public string UserVariable4 => userVar4.Value;
        public string UserVariable5 => userVar5.Value;
        public string UserVariable6 => userVar6.Value;
        public string UserVariable7 => userVar7.Value;
        public string UserVariable8 => userVar8.Value;
        public string UserVariable9 => userVar9.Value;
    }
}
